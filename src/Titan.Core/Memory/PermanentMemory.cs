namespace Titan.Core.Memory;

public readonly unsafe struct PermanentMemory : IMemoryAllocator<PermanentMemory>
{
    private readonly TransientMemory _memory;
    public PermanentMemory(Allocator* allocator)
        => _memory = new(allocator);
    public MemoryBlock GetBlock(uint size, bool initialize = false)
        => _memory.GetBlock(size, initialize);
    public MemoryBlock<T> GetBlock<T>(uint count, bool initialize = false) where T : unmanaged
        => _memory.GetBlock<T>(count, initialize);
    public T* GetPointer<T>(bool initialize = false) where T : unmanaged
        => _memory.GetPointer<T>(initialize);
    public void* GetPointer(uint size, bool initialize = false)
        => _memory.GetPointer(size, initialize);
    public ref T Get<T>(bool initialize) where T : unmanaged
        => ref _memory.Get<T>(initialize);

    public static PermanentMemory CreateAllocator(Allocator* allocator) 
        => new PermanentMemory(allocator);
}
