namespace Titan.Assets
{
    internal enum LoadState
    {
        LoadRequested,
        ReadingFile,
        FileReadCompleted,
        RequestMetadata,
        RequestDependencies,
        WaitingForDependencies,
        CreatingAsset,
        WaitingForAsset,
        CachingAsset,
        Loaded,
        UnloadRequested,
        Unloaded,
    }
}
