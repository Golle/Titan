using System.Diagnostics;
using System.Threading.Channels;

namespace Titan.Tools.Editor.Services.Metadata;

internal class AssetsBackgroundService
{
    private readonly AssetsFileWatcher _assetsFileWatcher;
    private readonly IAssetFileProcessor[] _processors;
    private readonly Channel<FileSystemEventArgs> _channel = Channel.CreateUnbounded<FileSystemEventArgs>();
    private Task? _backgroundTask;
    private CancellationTokenSource? _cancellationTokenSource;

    public AssetsBackgroundService(AssetsFileWatcher assetsFileWatcher, IEnumerable<IAssetFileProcessor> processors)
    {
        _assetsFileWatcher = assetsFileWatcher;
        _processors = processors.ToArray();
    }

    public bool Start(string assetPath)
    {
        var writer = _channel.Writer;
        _assetsFileWatcher.Start(assetPath, new AssetFileWatcherCallbacks
        {
            OnChanged = args => writer.TryWrite(args),
            OnCreated = args => writer.TryWrite(args),
            OnRenamed = args => writer.TryWrite(args),
            OnDeleted = args => writer.TryWrite(args)
        });
        _cancellationTokenSource = new CancellationTokenSource();
        _backgroundTask = Task.Run(Processor);
        return true;
    }

    private async Task Processor()
    {
        var token = _cancellationTokenSource!.Token;
        var reader = _channel.Reader;
        while (!token.IsCancellationRequested && await reader.WaitToReadAsync(token))
        {
            var result = await reader.ReadAsync(CancellationToken.None);
            switch (result.ChangeType)
            {

                case WatcherChangeTypes.Created:
                    await OnFileCreated(result);
                    break;
                case WatcherChangeTypes.Changed:
                    await OnFileChanged(result);
                    break;
                case WatcherChangeTypes.Deleted:
                    await OnFileDeteled(result);
                    break;
                case WatcherChangeTypes.Renamed:
                    await OnFileRenamed((RenamedEventArgs)result);
                    break;
            }
        }

        // add flush method or just ignore? we'll process all files at startup anyway.
    }

    private async Task OnFileCreated(FileSystemEventArgs args)
    {
        var info = new FileInfo(args.FullPath);
        var isDirectory = (info.Attributes & FileAttributes.Directory) != 0;
        var fileExtension = isDirectory ? null : Path.GetExtension(args.FullPath);
        var fileCreated = new AssetFileCreatedInfo(args.Name ?? string.Empty, args.FullPath, isDirectory, fileExtension, info.CreationTime);
        foreach (var processor in _processors)
        {
            try
            {
                await processor.OnFileCreated(fileCreated);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception {e.GetType().Name} ({e.Message}) occured when processing file {args.Name} in {processor.GetType().Name}.{nameof(IAssetFileProcessor.OnFileCreated)}");
            }
        }
    }

    private async Task OnFileDeteled(FileSystemEventArgs args)
    {
        var fileExtension = Path.GetExtension(args.FullPath);
        var fileDeleted = new AssetFileDeletedInfo(args.Name ?? string.Empty, args.FullPath, fileExtension.Length == 0 ? null : fileExtension);
        foreach (var processor in _processors)
        {
            try
            {
                await processor.OnFileDeleted(fileDeleted);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception {e.GetType().Name} ({e.Message}) occured when processing file {args.Name} in {processor.GetType().Name}.{nameof(IAssetFileProcessor.OnFileDeleted)}");
            }
        }
    }

    private async Task OnFileChanged(FileSystemEventArgs args)
    {
        var info = new FileInfo(args.FullPath);
        var isDirectory = (info.Attributes & FileAttributes.Directory) != 0;
        var fileExtension = isDirectory ? null : Path.GetExtension(args.FullPath);
        var fileSize = isDirectory ? 0 : info.Length;
        var assetFileChanged = new AssetFileInfo(args.Name ?? string.Empty, args.FullPath, isDirectory, fileExtension, info.LastWriteTime, fileSize);
        foreach (var processor in _processors)
        {
            try
            {
                await processor.OnFileChanged(assetFileChanged);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception {e.GetType().Name} ({e.Message}) occured when processing file {args.Name} in {processor.GetType().Name}.{nameof(IAssetFileProcessor.OnFileChanged)}");
            }
        }
    }

    private async Task OnFileRenamed(RenamedEventArgs args)
    {
        var info = new FileInfo(args.FullPath);
        var isDirectory = (info.Attributes & FileAttributes.Directory) != 0;
        var newFileExtension = isDirectory ? null : Path.GetExtension(args.FullPath);
        var oldFileExtension = isDirectory ? null : Path.GetExtension(args.OldFullPath);
        var assetFileRenameInfo = new AssetFileRenameInfo(args.OldName ?? string.Empty, args.Name ?? string.Empty, args.OldFullPath, args.FullPath, oldFileExtension, newFileExtension, isDirectory);
        foreach (var processor in _processors)
        {
            try
            {
                await processor.OnFileRename(assetFileRenameInfo);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception {e.GetType().Name} ({e.Message}) occured when processing file {args.Name} in {processor.GetType().Name}.{nameof(IAssetFileProcessor.OnFileRename)}");
            }
        }
    }
    public async Task Stop()
    {
        _assetsFileWatcher.Stop();
        if (_cancellationTokenSource != null)
        {
            await _cancellationTokenSource.CancelAsync();
        }
        if (_backgroundTask != null)
        {
            await _backgroundTask;
        }

        _cancellationTokenSource?.Dispose();
        _backgroundTask?.Dispose();
        _cancellationTokenSource = null;
        _backgroundTask = null;
    }
}
