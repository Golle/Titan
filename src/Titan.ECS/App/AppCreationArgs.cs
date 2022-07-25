namespace Titan.ECS.App;

public record struct AppCreationArgs
{
    public const uint DefaultResourcesMemory = 32 * 1024 * 1024;      // 32Mb
    public const uint DefaultMaxResourceTypes = 1_000;
    public nuint UnmanagedMemory { get; init; }
    public uint ResourcesMemory { get; init; }
    public uint MaxResourceTypes { get; init; }


    public static AppCreationArgs Default => new()
    {
        ResourcesMemory = DefaultResourcesMemory,
        UnmanagedMemory = 0u,
        MaxResourceTypes = DefaultMaxResourceTypes
    };
}
