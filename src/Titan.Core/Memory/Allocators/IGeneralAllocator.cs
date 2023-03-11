namespace Titan.Core.Memory.Allocators;

public unsafe interface IGeneralAllocator
{
    TitanBuffer AllocateBuffer(uint size, bool initialize = false);
    TitanArray<T> AllocateArray<T>(uint count, bool initialize = false) where T : unmanaged;
    TitanQueue<T> AllocateQueue<T>(uint count) where T : unmanaged;

    bool TryAllocateQueue<T>(out TitanQueue<T> queue, uint count) where T : unmanaged;
    bool TryAllocateArray<T>(out TitanArray<T> array, uint count, bool initialize = false) where T : unmanaged;
    T* Allocate<T>(uint count, bool initialize = true) where T : unmanaged;
    T* Allocate<T>(bool initialize = true) where T : unmanaged;
    void* Allocate(uint size, bool initialize = true);
    void Free(ref TitanBuffer buffer);
    void Free<T>(ref TitanArray<T> array) where T : unmanaged;
    void Free(void* ptr);
    void Release();
}
