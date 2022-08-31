using System.Diagnostics;
using System.Threading;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Threading2;
using Titan.Memory.Arenas;

namespace Titan.Assets.NewAssets;

internal unsafe struct LoaderUNDECIDEDNAME
{
    private BestFitFixedArena _allocator;
    private AssetRegistry* _registry;
    private JobApi* _jobApi;

    public static bool Create(AssetRegistry* registry, JobApi* jobApi, out LoaderUNDECIDEDNAME loader)
    {
        loader = default;
        if (!BestFitFixedArena.Create(4096, MemoryUtils.MegaBytes(64), out var allocator))
        {
            Logger.Error<LoaderUNDECIDEDNAME>($"Failed to create the {nameof(BestFitFixedArena)}.");
            return false;
        }

        loader = new()
        {
            _allocator = allocator,
            _registry = registry,
            _jobApi = jobApi
        };
        return true;
    }

    public void Load(in Handle<Asset> handle)
    {
        Debug.Assert(handle.IsValid());
        ref var asset = ref _registry->Get(handle);
        if (asset.State != AssetState.Unloaded && asset.State != AssetState.LoadRequested)
        {
            Logger.Warning<LoaderUNDECIDEDNAME>($"Trying to load an asset that's in state {asset.State}. (Expected: {AssetState.Unloaded} or {AssetState.LoadRequested})");
            return;
        }
        asset.State = AssetState.LoadRequested;
        Interlocked.Increment(ref asset.ReferenceCount);
    }

    public void Unload(in Handle<Asset> handle)
    {
        Debug.Assert(handle.IsValid());
        ref var asset = ref _registry->Get(handle);
        if (asset.State != AssetState.Loaded)
        {
            Logger.Warning<LoaderUNDECIDEDNAME>($"Trying to unload an asset that's in state {asset.State}. (Expected: {AssetState.Loaded})");
            return;
        }
        var currentRefCount = Interlocked.Increment(ref asset.ReferenceCount);
        if (currentRefCount <= 0)
        {
            asset.State = AssetState.UnloadRequested;
        }
    }

    public void Update()
    {
        foreach (ref var asset in _registry->GetAssets())
        {
            switch (asset.State)
            {
                case AssetState.Loaded:
                case AssetState.Unloaded:
                case AssetState.Loading:
                case AssetState.ReadingFile:
                case AssetState.Unloading:
                    //we don't care about these states
                    break;
                case AssetState.LoadRequested:
                    // start read file
                    asset.State = AssetState.ReadingFile;
                    asset.FileBuffer = _allocator.Allocate((nuint)asset.Descriptor.GetSize());
                    _jobApi->Enqueue(JobItem.Create(ref asset, &AsyncReadFile));
                    break;

                case AssetState.ReadFileCompleted:
                    if (asset.FileBuffer != null)
                    {
                        _allocator.Free(asset.FileBuffer);
                    }
                    break;
            }
        }

    }

    private static void AsyncReadFile(ref AssetContext asset)
    {

    }

}
