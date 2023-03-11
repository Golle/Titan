using Titan.Core.Memory.Allocators;

namespace Titan.Core.Memory;

public unsafe interface IMemoryManager
{
    void* Alloc(int size, bool initialize = false);
    void* Alloc(uint size, bool initialize = false);
    T* Alloc<T>(uint count, bool initialize = false) where T : unmanaged;
    TitanBuffer AllocBuffer(uint size, bool initialize = false);
    TitanArray<T> AllocArray<T>(uint count, bool initialize = false) where T : unmanaged;
    TitanQueue<T> AllocQueue<T>(uint count) where T : unmanaged;
    bool TryAllocArray<T>(out TitanArray<T> array, uint count, bool initialize = false) where T : unmanaged;
    bool TryAllocQueue<T>(out TitanQueue<T> queue, uint count) where T : unmanaged;
    void Free(void* ptr);
    void Free(ref TitanBuffer buffer);
    void Free<T>(ref TitanArray<T> buffer) where T : unmanaged;
    bool TryCreatePoolAllocator<T>(AllocatorStrategy allocatorStrategy, uint maxCount, out IPoolAllocator<T> allocator) where T : unmanaged;
    bool TryCreateLinearAllocator(AllocatorStrategy allocatorStrategy, uint size, out ILinearAllocator allocator);
    bool TryCreateGeneralAllocator(uint minSize, out IGeneralAllocator allocator);
}
