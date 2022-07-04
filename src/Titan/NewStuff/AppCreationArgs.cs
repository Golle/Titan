namespace Titan.NewStuff;

public record AppCreationArgs
{
    public const uint DefaultUnmanagedMemory = 1 * 1024 * 1024 * 1024;  //1Gb
    public const uint DefaultGlobalResourcesMemory = 32 * 1024 * 1024;  // 32Mb
    public const uint DefaultGlobalResourceTypes = 100;

    public uint UnmanagedMemory { get; init; }
    public uint GlobalResourcesMemory { get; init; }
    public uint GlobalResourcesTypes { get; init; }

    public static AppCreationArgs Default => new()
    {
        GlobalResourcesMemory = DefaultGlobalResourcesMemory,
        GlobalResourcesTypes = DefaultGlobalResourceTypes,
        UnmanagedMemory = DefaultUnmanagedMemory
    };
}
