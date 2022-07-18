using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Core.Memory;

/// <summary>
/// This is just a pool of memory with no possibility for Dealloc/free.
/// Create allocators on top of this pool for different usages.
/// Also look into how we can use virtual address space instead of raw pointers to native memory (unless its the same thing on Windows?)
/// The problem we're trying to solve is to have a deterministic amount of memory usage during the applications lifecycle,
/// low fragmentation and the possiblity for dynamic allocs and frees. 
/// </summary>
public readonly unsafe struct MemoryPool : IDisposable
{
    // NOTE(Jens): align all allocations to 8 bytes. (We should measure this at some point)
    private const int Alignment = 8;

    private readonly byte* _mem;
    private readonly int _size;
    
    // Hack to allow it to be readonly.
    private readonly int* _next;

    private MemoryPool(byte* mem, int size)
    {
        _mem = mem;
        _size = size;
        // The first element in the pool is the counter
        _next = (int*)mem;
        // Set next to next aligned byte
        *_next = Alignment;
    }

    public static MemoryPool Create(uint bytes)
    {
        var mem = (byte*)NativeMemory.Alloc(bytes);
        if (mem == null)
        {
            throw new OutOfMemoryException($"Failed to allocate {bytes} bytes of memory.");
        }
        return new MemoryPool(mem, (int)bytes);
    }

    public T CreateAllocator<T>(uint size, bool initialize = false) where T : unmanaged, IMemoryAllocator<T>
    {
        var memory = GetPointer(size, initialize);
        var allocator = GetPointer<Allocator>();
        *allocator = new Allocator(memory, size);
        return T.CreateAllocator(allocator);
    }

    public T* GetPointer<T>(uint count = 1, bool initialize = false) where T : unmanaged
        => (T*)GetOffset((uint)sizeof(T) * count, initialize);

    public void* GetPointer(uint size, bool initialize = false)
        => GetOffset(size, initialize);

    private void* GetOffset(uint size, bool zeroMemory)
    {
        var location = _mem + Increment(size);
        if (zeroMemory)
        {
            Unsafe.InitBlock(location, 0, size);
        }
        return location;
    }

    // NOTE(Jens): this is a very naive solution to memory allocation. We might want to look at page size and allocate blocks instead of packing everything like this.
    private int Increment(uint size)
    {
        var current = *_next;
        // Add the aligned size
        // NOTE(Jens): this is not thread safe.
        *_next += (int)(size + Alignment - 1) & -Alignment;
        
        Debug.Assert(*_next <= _size, "The requested memory block is out of range.");

        // return current
        return current;
    }


    public void Dispose()
    {
        NativeMemory.Free(_mem);
    }
}
