using Titan.Core;

namespace Titan.Modules;

public struct MemoryConfiguration : IDefault<MemoryConfiguration>
{
    private const uint DefaultTransientMemoryPool = 128 * 1024 * 1024;
    public uint TransientMemory;
    public static MemoryConfiguration Default =>
        new()
        {
            TransientMemory = DefaultTransientMemoryPool
        };
}
