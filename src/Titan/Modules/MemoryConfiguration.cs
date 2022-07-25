using Titan.Core;

namespace Titan.Modules;
/// <summary>
/// Set the <see cref="FixedSizeMemoryPool"/> property to have a fixed size memory pool instead of a dynamic one.
/// </summary>
public struct MemoryConfiguration : IDefault<MemoryConfiguration>
{
    private const uint DefaultTransientMemoryPool = 128 * 1024 * 1024;
    
    public uint TransientMemory;
    public uint FixedSizeMemoryPool;
    public static MemoryConfiguration Default =>
        new()
        {
            TransientMemory = DefaultTransientMemoryPool,
            FixedSizeMemoryPool = 0
        };
}
