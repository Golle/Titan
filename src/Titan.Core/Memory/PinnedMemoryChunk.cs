using System;
using System.Runtime.CompilerServices;

namespace Titan.Core.Memory
{
    public readonly unsafe struct PinnedMemoryChunk<T>
    {
        private readonly T[] _data;
        private readonly uint _size;
        public uint Size => _size;
        public PinnedMemoryChunk(in T[] data)
        {
            _data = data;
            _size = (uint) data.Length;
        }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _data[index];
        }

        public ref T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _data[index];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AsSpan() => _data.AsSpan();
    }
}
