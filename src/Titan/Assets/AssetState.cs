namespace Titan.Assets;

internal enum AssetState
{
    Unloaded,
    LoadRequested,
    ReadingFile,
    ReadFileCompleted,
    CreatingResource,
    ResourceCreationCompleted,
    ResourceRecreationCompleted,
    Loaded,
    /*
     * Add states for resolving dependencies
     */
    UnloadRequested,
    Unloading,
    UnloadCompleted,
    /// <summary>
    /// This is used when a resource is loaded synchronous.
    /// </summary>
    InlineLoading,
    ReloadRequested,
    Reloading,
    Error,
    
}
