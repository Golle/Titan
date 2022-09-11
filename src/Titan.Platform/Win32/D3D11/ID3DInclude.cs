using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Titan.Platform.Win32.D3D11;

public unsafe struct ID3DInclude
{
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject) => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
}
