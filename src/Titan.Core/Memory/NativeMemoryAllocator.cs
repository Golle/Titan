using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Titan.Core.Memory;

public readonly unsafe struct MemoryBlock<T> where T : unmanaged
{
    private readonly T* _ptr;
    private readonly uint _count;
    internal MemoryBlock(T* start, uint count)
    {
        _ptr = start;
        _count = count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsSpan() => new(_ptr, (int)_count);
}

public readonly unsafe struct MemoryBlock
{
    public uint Size => _size;
    private readonly void* _ptr;
    private readonly uint _size;

    internal MemoryBlock(void* start, uint size)
    {
        _ptr = start;
        _size = size;
    }

    public ReadOnlySpan<byte> AsSpan() => new(_ptr, (int)_size);

    public byte* AsPointer() => (byte*)_ptr;
}



public interface ITransientMemoryAllocator : IMemoryAllocator
{
    ref T Get<T>(bool initialize) where T : unmanaged;
    MemoryBlock<T> GetBlock<T>(uint count, bool zeroMemory = false) where T : unmanaged;
    void Reset();
}
public interface IPersistentMemoryAllocator : IMemoryAllocator
{
}


public interface IMemoryAllocator
{
    MemoryBlock GetBlock(uint size, bool zeroMemory = false);
}

public unsafe class NativeMemoryAllocator : IMemoryAllocator, IPersistentMemoryAllocator, ITransientMemoryAllocator, IDisposable
{
    private byte* _ptr;
    private readonly int _size;

    private volatile int _next = 0;
    public NativeMemoryAllocator(nuint bytes)
    {
        _size = (int)bytes;
        _ptr = (byte*)NativeMemory.Alloc(bytes);
        if (_ptr == null)
        {
            throw new OutOfMemoryException($"Failed to allocate {bytes} bytes of memory.");
        }
    }

    public ref T Get<T>(bool initialize) where T : unmanaged
    {
        var offset = (T*)GetOffset((uint)sizeof(T), initialize);
        return ref *offset;
    }

    public MemoryBlock GetBlock(uint size, bool zeroMemory = false) => new(GetOffset(size, zeroMemory), size);
    public MemoryBlock<T> GetBlock<T>(uint count, bool zeroMemory = false) where T : unmanaged => new((T*)GetOffset((uint)(sizeof(T) * count), zeroMemory), count);

    public void Reset() => _next = 0;

    private byte* GetOffset(uint size, bool zeroMemory)
    {
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
