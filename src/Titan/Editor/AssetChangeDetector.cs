using Titan.Assets;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Tools.Core.Manifests;

namespace Titan.Editor;

internal class AssetChangeDetector
{
    private class InternalAsset
    {
        public string Path { get; init; }
        public AssetDescriptor Descriptor { get; init; }
        public DateTime LastWriteTime { get; set; }
    }
    public int ChangeCount
    {
        get
        {
            var lockTaken = false;
            _lock.Enter(ref lockTaken);
            var count = _changes.Count;
            _lock.Exit();
            return count;
        }
    }

    private SpinLock _lock;
    private readonly List<FileSystemWatcher> _watchers = new();
    private readonly List<InternalAsset> _assets = new();
    private readonly Queue<Handle<Asset>> _changes = new();

    public void GetHandles(Span<Handle<Asset>> outHandles)
    {
        var lockTaken = false;
        _lock.Enter(ref lockTaken);
        var min = Math.Min(_changes.Count, outHandles.Length);
        for (var i = 0; i < min; i++)
        {
            if (!_changes.TryDequeue(out outHandles[i]))
            {
                break;
            }
        }
        _lock.Exit();
    }

    public bool Init(AssetsConfiguration[] configs, AssetsRegistry registry)
    {
        foreach (var config in configs)
        {
            var basepath = Path.GetDirectoryName(config.ManifestFile)!;
            for (var i = 0; i < config.RawAssets.Length; ++i)
            {
                var path = (config.RawAssets[i] as ManifestItemWithPath)?.Path;
                if (path == null)
                {
                    // asset wihtout a path
                    continue;
                }
                var fullPath = Path.GetFullPath(Path.Combine(basepath, path));
                _assets.Add(new()
                {
                    Path = fullPath,
                    Descriptor = config.AssetDescriptors[i],
                    LastWriteTime = DateTime.Now
                });
            }

            var watcher = new FileSystemWatcher
            {
                Path = basepath!,
                IncludeSubdirectories = true,
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite,
            };
            watcher.Changed += (_, args) =>
            {
                var asset = _assets.FirstOrDefault(s => s.Path == args.FullPath);
                if (asset == null)
                {
                    Logger.Warning<AssetChangeDetector>($"File changed that is not in the asset list. {args.FullPath}");
                    return;
                }
                var lastWriteTime = File.GetLastWriteTime(args.FullPath);


                try
                {
                    var lockTaken = false;
                    _lock.Enter(ref lockTaken);
                    // we get multiple calls, just notify on the first one and record the date. sometimes we get multiple calls with just 1 ms difference, so we just notify every 100ms.
                    if (lastWriteTime - asset.LastWriteTime <= TimeSpan.FromMilliseconds(100))
                    {
                        return;
                    }

                    var handle = registry.GetHandleFromDescriptor(asset.Descriptor);
                    Logger.Trace<AssetChangeDetector>($"Asset {asset.Descriptor.Id} with handle {handle.Value} changed. ({asset.Path})");
                    asset.LastWriteTime = lastWriteTime;
                    // notify
                    _changes.Enqueue(handle);
                }
                finally
                {
                    _lock.Exit();
                }
            };
            _watchers.Add(watcher);
        }
        return true;
    }


    public void Shutdown()
    {
        foreach (var watcher in _watchers)
        {
            watcher.Dispose();
        }
        _watchers.Clear();
    }
}
