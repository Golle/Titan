using System;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32
{
    public unsafe struct ComPtr<T> : IDisposable where T : unmanaged
    {
        private T* _ptr;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T* Get() => _ptr;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref readonly T* GetPinnableReference()
        {
            fixed (T** ptr = &_ptr)
            {
                return ref *ptr;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly T** GetAddressOf()
        {
            fixed (T** ptr = &_ptr)
            {
                return ptr;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            Release();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Release()
        {
            // TODO: handle AddRef and Release counts
            if (_ptr != null)
            {
                var unknown = ((IUnknown*)_ptr);
                var result = unknown->Release();
                _ptr = null;
            }
        }
    }
}
