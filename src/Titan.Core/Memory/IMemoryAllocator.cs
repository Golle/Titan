namespace Titan.Core.Memory;

public unsafe interface IMemoryAllocator<out TAllocator> where TAllocator : unmanaged, IMemoryAllocator<TAllocator>
{
    MemoryBlock GetBlock(uint size, bool initialize = false);
    MemoryBlock<T> GetBlock<T>(uint count, bool initialize = false) where T : unmanaged;
    T* GetPointer<T>(bool initialize) where T : unmanaged;
    T* GetPointer<T>(uint count, bool initialize) where T : unmanaged;
    void* GetPointer(uint size, bool initialize = false);
    ref T Get<T>(bool initialize) where T : unmanaged;
    static abstract TAllocator CreateAllocator(Allocator* allocator);
}
