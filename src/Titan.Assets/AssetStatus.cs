namespace Titan.Assets
{
    public enum AssetStatus
    {
        Unloaded,

        LoadRequested,
        WaitingForDependencies,
        DependenciesLoaded,
        ReadingFiles,
        FileReadComplete,
        CreatingAsset,
        AssetCreated,
        Loaded,
        UnloadRequested
    }
}
