using System.Runtime.CompilerServices;

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
    public Span<T> AsSpan() => new(_ptr, (int)_count);
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> AsSpan() => new(_ptr, (int)_size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte* AsPointer() => (byte*)_ptr;
}
