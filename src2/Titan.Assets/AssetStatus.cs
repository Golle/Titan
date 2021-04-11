namespace Titan.Assets
{
    public enum AssetStatus
    {
        Unloaded,
        LoadRequested,
        ReadingFile,
        FileReadComplete,
        RequestDependencies,
        WaitingForDependencies,
        CreatingAsset,
        AssetCreated,
        Loaded,
        UnloadRequested
    }
}
