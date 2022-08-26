using System.Runtime.CompilerServices;

namespace Titan.Core;

public unsafe struct TitanArray<T> where T : unmanaged
{
    private readonly T* _mem;
    private readonly nuint _size;
    internal TitanArray(void* mem, nuint size)
    {
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

        public readonly ref readonly T Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _array[(uint)_next];
        }
    }
}
