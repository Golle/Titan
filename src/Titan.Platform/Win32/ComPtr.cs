using System.Diagnostics;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Titan.Platform.Win32;

#if DEBUG
[DebuggerDisplay("{DebugString()}")]
#endif
public unsafe struct ComPtr<T> : IDisposable where T : unmanaged
{
    private T* _ptr;

    public ComPtr(in T* ptr)
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
    public T** ReleaseAndGetAddressOf()
    {
        InternalRelease();
        return GetAddressOf();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly T** GetAddressOf()
        => (T**)Unsafe.AsPointer(ref Unsafe.AsRef(in this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComPtr<T> Wrap(in T* ptr) => new() { _ptr = ptr }; // Use object initializer to avoid InternalAddRef

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => InternalRelease();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Reset() => InternalRelease();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void InternalAddRef() => ((IUnknown*)_ptr)->AddRef();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint InternalRelease()
    {
        var result = 0u;
        if (_ptr != null)
        {
            var unknown = (IUnknown*)_ptr;
            result = unknown->Release();
            _ptr = null;
        }
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator T*(in ComPtr<T> p) => p._ptr;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator IUnknown*(in ComPtr<T> p) => (IUnknown*)p._ptr;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ComPtr<T>(T* ptr) => new(ptr);

#if DEBUG
    private string DebugString()
        => $"Ptr: 0x{(nint)_ptr:x16}";
#endif
}
