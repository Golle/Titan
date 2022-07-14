namespace Titan.ECS.AnotherTry;

public record struct AppCreationArgs
{
    public const uint DefaultUnmanagedMemory = 1 * 1024 * 1024 * 1024;      // 1Gb
    public const uint DefaultResourcesMemory = 32 * 1024 * 1024;      // 32Mb
    public const uint DefaultMaxResourceTypes = 1_000;
    public uint UnmanagedMemory { get; init; }
    public uint ResourcesMemory { get; init; }
    public uint MaxResourceTypes { get; init; }


    public static AppCreationArgs Default => new()
    {
        ResourcesMemory = DefaultResourcesMemory,
        UnmanagedMemory = DefaultUnmanagedMemory,
        MaxResourceTypes = DefaultMaxResourceTypes
    };
}
