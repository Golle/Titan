using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Core;

public unsafe struct TitanArray<T> where T : unmanaged
{
    private readonly T* _mem;
    private readonly int _size;
    public TitanArray(void* mem, uint size) 
        : this(mem, (int)size)
    {
    }

    public TitanArray(void* mem, int size)
    {
        Debug.Assert(size >= 0);
        _size = size;
        _mem = (T*)mem;
    }

    public readonly int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (int)_size;
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
    public Span<T> AsSpan() => new(_mem, (int)_size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T* GetPointer(uint index = 0u)
    {
        Debug.Assert(index < _size);
        return _mem + index;
    }

    public static implicit operator T*(in TitanArray<T> array) => array.GetPointer();
    public static implicit operator void*(in TitanArray<T> array) => array.GetPointer();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Enumerator GetEnumerator() => new(ref this);

    public ref struct Enumerator
    {
        private readonly ref TitanArray<T> _array;
        private int _next;
        internal Enumerator(ref TitanArray<T> array)
        {
            _array = ref array;
            _next = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_next == -1)
            {
                _next = 0;
                return _next < _array.Length;
            }
            _next++;
            return _next < _array.Length;
        }

        public readonly ref T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _array[(uint)_next];
        }
    }
}
