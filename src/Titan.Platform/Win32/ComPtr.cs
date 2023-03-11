using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32;

public unsafe struct ComPtr<T> : IDisposable where T : unmanaged
{
    private T* _ptr;
    public Guid UUID => typeof(T).GUID;

    public ComPtr(T* ptr)
    {
        _ptr = ptr;
    }

    public ComPtr(in ComPtr<T> ptr)
    {
        _ptr = ptr._ptr;
        InternalAddRef();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T* Get() => _ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T** GetAddressOf()
    {
        fixed (T** addressOf = &_ptr)
        {
            return addressOf;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InternalAddRef()
    {
        Debug.Assert(_ptr != null);
        ((IUnknown*)_ptr)->AddRef();
    }

    public void Reset()
    {
        if (_ptr != null)
        {
            var result = ((IUnknown*)_ptr)->Release();
            if (result == 0)
            {
                _ptr = null;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T*(in ComPtr<T> ptr) => ptr._ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ComPtr<T>(T* ptr) => new(ptr);
    public void Dispose() => Reset();
}
