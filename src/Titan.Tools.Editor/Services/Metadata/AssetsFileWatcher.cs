namespace Titan.Tools.Editor.Services.Metadata;

internal record AssetFileWatcherCallbacks
{
    public required Action<FileSystemEventArgs> OnCreated { get; init; }
    public required Action<FileSystemEventArgs> OnChanged { get; init; }
    public required Action<FileSystemEventArgs> OnDeleted { get; init; }
    public required Action<RenamedEventArgs> OnRenamed { get; init; }
}

internal class AssetsFileWatcher
{
    private FileSystemWatcher? _watcher;
    
    public void Start(string path, AssetFileWatcherCallbacks callbacks)
    {
        if (_watcher != null)
        {
            throw new InvalidOperationException($"The {nameof(AssetsFileWatcher)} has already been initialized.");
        }

        _watcher = new FileSystemWatcher(path)
        {
            IncludeSubdirectories = true,
        };
        _watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite;
        _watcher.Changed += (_, args) => callbacks.OnChanged(args);
        _watcher.Created += (_, args) => callbacks.OnCreated(args);
        _watcher.Renamed += (_, args) => callbacks.OnRenamed(args);
        _watcher.Deleted += (_, args) => callbacks.OnDeleted(args);
        _watcher.EnableRaisingEvents = true;
    }

    public void Stop()
    {
        if (_watcher != null)
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _watcher = null;
        }
    }
}
