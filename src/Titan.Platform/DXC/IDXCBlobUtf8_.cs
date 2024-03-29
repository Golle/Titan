using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32;

namespace Titan.Platform.DXC;

[Guid("3DA636C9-BA71-4024-A301-30CBF125305B")]
public unsafe struct IDXCBlobUtf8 // : IDxcBlobEncoding
{
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //IDxcBlobEncoding
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* GetBufferPointer()
        => ((delegate* unmanaged[Stdcall]<void*, void*>)_vtbl[3])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nuint GetBufferSize()
        => ((delegate* unmanaged[Stdcall]<void*, nuint>)_vtbl[4])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetEncoding(int* pKnown, uint* pCodePage)
        => ((delegate* unmanaged[Stdcall]<void*, int*, uint*, HRESULT>)_vtbl[5])(Unsafe.AsPointer(ref this), pKnown, pCodePage);

    //IDxcBlobUtf8
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte* GetStringPointer()
        => ((delegate* unmanaged[Stdcall]<void*, byte*>)_vtbl[6])(Unsafe.AsPointer(ref this));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nuint GetStringLength()
        => ((delegate* unmanaged[Stdcall]<void*, nuint>)_vtbl[7])(Unsafe.AsPointer(ref this));
}
