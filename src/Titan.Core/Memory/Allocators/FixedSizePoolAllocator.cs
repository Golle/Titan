using System.Runtime.CompilerServices;

namespace Titan.Core.Memory.Allocators;

internal unsafe class FixedSizePoolAllocator<T> : IPoolAllocator<T> where T : unmanaged
{
    public static bool TryCreate(IMemoryManager memoryManager, uint count, out IPoolAllocator<T> allocator)
    {
        Unsafe.SkipInit(out allocator);
        throw new NotImplementedException("THis has not been implemented yet. Not needed at the moment.");
        //return true;
    }

    private struct Header
    {
        //public Header* Previous;
    }
}
