using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32;

public unsafe struct IUnknown
{
    // TODO add the rest of the IUnkown methods when needed
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject) => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
}
