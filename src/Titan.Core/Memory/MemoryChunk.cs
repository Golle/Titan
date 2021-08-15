using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Core.Memory
{
    public readonly unsafe struct MemoryChunk
    {
        private readonly void* _ptr;
        internal MemoryChunk(void* ptr) => _ptr = ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free()
        {
            if (_ptr != null)
            {
                NativeMemory.Free(_ptr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* AsPointer() => _ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void*(in MemoryChunk memory) => memory._ptr;
    }

    public readonly unsafe struct MemoryChunk<T> where T : unmanaged
    {
        private readonly T* _ptr;
        private readonly uint _size;
        private readonly uint _count;
        /// <summary>
        /// The number of elements in this memory chunk. This can be 0 in cases where the chunk has been created from a pointer
        /// </summary>
        public uint Count => _count;
        
        /// <summary>
        /// The size in bytes of the memory chunk
        /// </summary>
        public uint Size => _size;
        internal MemoryChunk(T* ptr, uint size, uint count)
        {
            _ptr = ptr;
            _size = size;
            _count = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* AsPointer() => _ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free()
        {
            if (_ptr != null)
            {
                NativeMemory.Free(_ptr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T*(in MemoryChunk<T> memory) => memory._ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void*(in MemoryChunk<T> memory) => memory._ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator MemoryChunk<T>(T* ptr) => new(ptr, 0u, 0u);

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _ptr[index];
        }

        public ref T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _ptr[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* GetPointer(int index) => &_ptr[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* GetPointer(uint index) => &_ptr[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan() => new (_ptr, (int) _size);
    }
}
