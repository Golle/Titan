namespace Titan.Core.Memory;

public readonly unsafe struct TransientMemory : IMemoryAllocator<TransientMemory>
{
    private readonly Allocator* _allocator;
    public uint Size => _allocator->Size;
    public uint Used => _allocator->Used;
    public TransientMemory(Allocator* allocator)
        => _allocator = allocator;

    public MemoryBlock GetBlock(uint size, bool initialize = false)
        => new(_allocator->GetPointer(size, initialize), size);

    public MemoryBlock<T> GetBlock<T>(uint count, bool initialize = false) where T : unmanaged
        => new(_allocator->GetPointer<T>(initialize), count);

    public T* GetPointer<T>(bool initialize) where T : unmanaged
        => _allocator->GetPointer<T>(initialize);

    public T* GetPointer<T>(uint count, bool initialize) where T : unmanaged 
        => (T*)GetPointer((uint)(sizeof(T) * count), initialize);

    public void* GetPointer(uint size, bool initialize)
        => _allocator->GetPointer(size, initialize);

    public ref T Get<T>(bool initialize) where T : unmanaged
        => ref *_allocator->GetPointer<T>(initialize);

    public T CreateSubAllocator<T>(uint size) where T : unmanaged, IMemoryAllocator<T>
    {
        var allocator = GetPointer<Allocator>(true);
        *allocator = new Allocator(GetPointer(size, false), size);
        return T.CreateAllocator(allocator);
    }

    public T As<T>() where T : unmanaged, IMemoryAllocator<T>
    {
        return T.CreateAllocator(_allocator);
    }

    public static TransientMemory CreateAllocator(Allocator* allocator) 
        => new(allocator);

    public void Reset()
        => _allocator->Reset();
}
