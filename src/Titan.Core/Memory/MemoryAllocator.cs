using System;

namespace Titan.Core.Memory;

/// <summary>
/// This memory allocator is only used for Transient memory at the moment. We should create an API that supports all 3 types that we want. Permanent, Per frame and buffers(over several frames, for example loading maps).
/// This struct should be moved to Titan.Core
/// </summary>
public readonly unsafe struct MemoryAllocator : IMemoryAllocator<MemoryAllocator>, IApi
{
    private readonly Allocator* _allocator;
    private uint Size => _allocator->Size;
    private uint Used => _allocator->Used;
    private MemoryAllocator(Allocator* allocator) 
        => _allocator = allocator;

    public MemoryBlock GetBlock(uint size, bool initialize = false)
        //=> new(_allocator.GetPointer(size, initialize), size);
        => throw new NotImplementedException();

    public MemoryBlock<T> GetBlock<T>(uint count, bool initialize = false) where T : unmanaged
        //=> new(_allocator.GetPointer<T>(initialize), count);
        => throw new NotImplementedException();

    public T* GetPointer<T>(bool initialize = false) where T : unmanaged
        => _allocator->GetPointer<T>(initialize);

    public T* GetPointer<T>(uint count, bool initialize = false) where T : unmanaged
        => (T*)GetPointer((uint)(sizeof(T) * count), initialize);

    public void* GetPointer(uint size, bool initialize = false)
        => _allocator->GetPointer(size, initialize);

    public ref T Get<T>(bool initialize = false) where T : unmanaged
        => ref *_allocator->GetPointer<T>(initialize);

    public static MemoryAllocator CreateAllocator(Allocator* allocator)
        => new(allocator);

    public void Reset(bool initialize = false) => _allocator->Reset(initialize);
}
