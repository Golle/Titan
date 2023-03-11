namespace Titan.Core.Memory.Allocators;
public unsafe interface ILinearAllocator
{
    void* Alloc(int size, bool initialize = false);
    void* Alloc(uint size, bool initialize = false);
    T* Alloc<T>(uint count = 1u, bool initialize = false) where T : unmanaged;
    TitanArray AllocArray(uint count, uint stride, bool initialize = false);
    TitanArray<T> AllocArray<T>(int count, bool initialize = false) where T : unmanaged;
    TitanArray<T> AllocArray<T>(uint count, bool initialize = false) where T : unmanaged;
    void Reset();
    void Destroy();
    uint GetBytesAllocated();
}
