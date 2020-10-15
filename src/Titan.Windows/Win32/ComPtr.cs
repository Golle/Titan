using System;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32
{
    public unsafe struct ComPtr<T> : IDisposable where T : unmanaged
    {
        private T* _ptr;
        
        public ComPtr(T* ptr)
        {
            _ptr = ptr;
            InternalAddRef();
        }

        public ComPtr(in ComPtr<T> ptr)
        {
            _ptr = ptr._ptr;
            InternalAddRef();
        }

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
            InternalRelease();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalAddRef() => ((IUnknown*)_ptr)->AddRef();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalRelease()
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
