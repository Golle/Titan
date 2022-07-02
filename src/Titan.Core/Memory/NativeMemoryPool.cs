using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Titan.Core.Memory;

public unsafe class NativeMemoryPool : IDisposable
{
    // NOTE(Jens): align all allocations to 8 bytes. (We should measure this at some point)
    private const int Alignment = 8;

    private byte* _ptr;
    private readonly int _size;
    private volatile int _next = 0;
    public NativeMemoryPool(uint bytes)
    {
        _size = (int)bytes;
        _ptr = (byte*)NativeMemory.Alloc(bytes);
        if (_ptr == null)
        {
            throw new OutOfMemoryException($"Failed to allocate {bytes} bytes of memory.");
        }
    }

    public T CreateAllocator<T>(uint size, bool initialize = false) where T : unmanaged, IMemoryAllocator<T>
    {
        var allocator = GetPointer<Allocator>();
        *allocator = new Allocator(GetPointer(size, initialize), size);
        return T.CreateAllocator(allocator);
    }


    public T* GetPointer<T>(uint count = 1, bool initialize = false) where T : unmanaged
        => (T*)GetOffset((uint)sizeof(T), initialize);

    public void* GetPointer(uint size, bool initialize = false)
        => GetOffset(size, initialize);

    private void* GetOffset(uint size, bool zeroMemory)
    {
        size = (uint)((size + Alignment - 1) & -Alignment);

        // NOTE(Jens): this is a very naive solution to memory allocation. We might want to look at page size and allocate blocks instead of packing everything like this.
        var blockEnd = Interlocked.Add(ref _next, (int)size);
        Debug.Assert(blockEnd <= _size, "The requested memory block is out of range.");
        var location = _ptr + blockEnd - size;
        if (zeroMemory)
        {
            Unsafe.InitBlock(location, 0, size);
        }
        return location;
    }

    public void Dispose()
    {
        if (_ptr != null)
        {
            NativeMemory.Free(_ptr);
            _ptr = null;
        }
    }
}
