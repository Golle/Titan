namespace Titan.Assets.NewAssets;

public enum AssetState
{
    Unloaded,
    LoadRequested,
    ReadingFile,
    Loading,
    Loaded,
    /*
     * Add states for resolving dependencies
     */
    UnloadRequested
}
