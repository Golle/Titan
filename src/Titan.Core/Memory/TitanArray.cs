using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core.Memory;

public readonly unsafe struct TitanArray
{
    private readonly byte* _mem;
    private readonly uint _stride;
    private readonly int _length;

    public TitanArray(void* mem, uint count, uint stride)
    {
        _mem = (byte*)mem;
        _length = (int)(count * stride);
        _stride = stride;
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* GetPointer(int index)
    {
        Debug.Assert(index >= 0);
        var offset = index * _stride;
        Debug.Assert(offset + _stride < _length);
        return _mem + offset;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* GetPointer(uint index = 0)
    {
        var offset = index * _stride;
        Debug.Assert(offset + _stride < _length);
        return _mem + offset;
    }
}



//NOTE(Jens): This does not align memory properly.
//NOTE(Jens): Add support for Stride (basically aligned size)
public readonly unsafe struct TitanArray<T> where T : unmanaged
{
    private readonly T* _mem;
    private readonly int _length;
    public bool IsValid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _mem != null;
    }

    public TitanArray(void* mem, uint length)
        : this(mem, (int)length)
    {
    }

    public TitanArray(void* mem, int length)
    {
        Debug.Assert(length >= 0);
        _length = length;
        _mem = (T*)mem;
    }

    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _length;
    }

    public ref T this[uint index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Debug.Assert(index < _length);
            return ref _mem[index];
        }
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            Debug.Assert(index >= 0 && index < _length);
            return ref _mem[index];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => new(_mem, _length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsReadOnlySpan() => new(_mem, _length);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TitanArray<T> Slice(int start, int length)
    {
        Debug.Assert(start >= 0);
        Debug.Assert(start + length <= _length);
        return new(_mem + start, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* GetPointer(int index)
    {
        Debug.Assert(index >= 0);
        return GetPointer((uint)index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T* GetPointer(uint index = 0u)
    {
        Debug.Assert(index < _length);
        return _mem + index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T*(in TitanArray<T> array) => array.GetPointer();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator void*(in TitanArray<T> array) => array.GetPointer();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<T>(in TitanArray<T> array) => new(array._mem, array._length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(in TitanArray<T> array) => new(array._mem, array._length);
}
