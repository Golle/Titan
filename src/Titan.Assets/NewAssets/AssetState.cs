namespace Titan.Assets.NewAssets;

internal enum AssetState
{
    Unloaded,
    LoadRequested,
    ReadingFile,
    ReadFileCompleted,
    Loading,
    LoadingCompleted,
    Loaded,
    /*
     * Add states for resolving dependencies
     */
    UnloadRequested,
    Unloading,
    UnloadCompleted,
    Error
}
