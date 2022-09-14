using Titan.Core.Memory;

namespace Titan.ECS.App;

public record struct AppCreationArgs
{
    public static readonly uint DefaultResourcesMemory = MemoryUtils.MegaBytes(64);
    public static readonly uint DefaultMaxReservedMemory = MemoryUtils.GigaBytes(2);
    public static readonly uint DefaultMaxGeneralPurposeMemory = MemoryUtils.MegaBytes(512);
    public static readonly uint DefaultMaxResourceTypes = 1_000;
    public nuint MaxReservedMemory { get; init; }
    public nuint MaxGeneralPurposeMemory { get; init; }
    
    // do we need these?
    public uint MaxResourcesMemory { get; init; }
    public uint MaxResourceTypes { get; init; }

    public static AppCreationArgs Default => new()
    {
        MaxResourcesMemory = DefaultResourcesMemory,
        MaxReservedMemory = DefaultMaxReservedMemory,
        MaxResourceTypes = DefaultMaxResourceTypes,
        MaxGeneralPurposeMemory = DefaultMaxGeneralPurposeMemory
    };
}
