namespace Titan.Assets.NewAssets;

public enum AssetState
{
    Unloaded,
    LoadRequested,
    ReadingFile,
    ReadFileCompleted,
    Loading,
    Loaded,
    /*
     * Add states for resolving dependencies
     */
    UnloadRequested,
    Unloading
}
