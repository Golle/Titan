using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core;

public readonly unsafe struct TitanArray<T> where T : unmanaged
{
    private readonly T* _mem;
    private readonly int _length;
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
        get => ref _mem[index];
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref _mem[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan() => new(_mem, _length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> AsReadOnlySpan() => new(_mem, _length);

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
