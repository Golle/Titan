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
                Marshal.FreeHGlobal((nint)_ptr);
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
        internal MemoryChunk(T* ptr) => _ptr = ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* AsPointer() => _ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free()
        {
            if (_ptr != null)
            {
                Marshal.FreeHGlobal((nint)_ptr);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T*(in MemoryChunk<T> memory) => memory._ptr;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator void*(in MemoryChunk<T> memory) => memory._ptr;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _ptr[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* GetPointer(int index) => &_ptr[index];
    }
}
