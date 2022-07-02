namespace Titan.Core.Memory;

public readonly unsafe struct TransientMemory : IMemoryAllocator<TransientMemory>
{
    private readonly Allocator* _allocator;
    public TransientMemory(Allocator* allocator)
        => _allocator = allocator;

    public MemoryBlock GetBlock(uint size, bool initialize = false)
        => new(_allocator->GetPointer(size, initialize), size);

    public MemoryBlock<T> GetBlock<T>(uint count, bool initialize = false) where T : unmanaged
        => new(_allocator->GetPointer<T>(initialize), count);

    public T* GetPointer<T>(bool initialize) where T : unmanaged
        => _allocator->GetPointer<T>(initialize);

    public void* GetPointer(uint size, bool initialize)
        => _allocator->GetPointer(size, initialize);

    public ref T Get<T>(bool initialize) where T : unmanaged
        => ref *_allocator->GetPointer<T>(initialize);

    public static TransientMemory CreateAllocator(Allocator* allocator) 
        => new(allocator);

    internal void Reset()
        => _allocator->Reset();
}
