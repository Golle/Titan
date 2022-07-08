namespace Titan.NewStuff;

public record AppCreationArgs
{
    public const uint DefaultUnmanagedMemory = 1 * 1024 * 1024 * 1024;      // 1Gb
    public const uint DefaultGlobalResourcesMemory = 32 * 1024 * 1024;      // 32Mb
    public const uint DefaultGlobalSystemTypes = 1000;


    // These should be used somwewhere else
    public const uint DefaultWorldMemory = 512 * 1024 * 1024;               // 512Mb
    public const uint DefaultWorldTransientMemory = 128 * 1024 * 1024;      // 128Mb
    public const uint DefaultGlobalTransientMemory = 128 * 1024 * 1024;     // 128Mb
    public const uint DefaultGlobalResourceTypes = 200;

    public uint UnmanagedMemory { get; init; }
    public uint GlobalResourcesMemory { get; init; }
    public uint GlobalSystemTypes { get; init; }


    // These should be used somwewhere else
    public uint GlobalTransientMemory { get; init; }
    public uint GlobalResourcesTypes { get; init; }
    public uint WorldMemory { get; init; }
    public uint WorldTransientMemory { get; init; }

    public static AppCreationArgs Default => new()
    {
        GlobalResourcesMemory = DefaultGlobalResourcesMemory,
        GlobalResourcesTypes = DefaultGlobalResourceTypes,
        UnmanagedMemory = DefaultUnmanagedMemory,
        GlobalTransientMemory = DefaultGlobalTransientMemory,
        WorldMemory = DefaultWorldMemory,
        WorldTransientMemory = DefaultWorldTransientMemory,
        GlobalSystemTypes = DefaultGlobalSystemTypes
    };

    
}
