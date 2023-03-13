using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

[Guid("affaa4ca-63fe-4d8e-b8ad-159000af4304")]
public unsafe struct ID3D12Debug1 : INativeGuid
{
    public static Guid* Guid => IID.IID_ID3D12Debug1;
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject) => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnableDebugLayer() => ((delegate* unmanaged[Stdcall]<void*, void>)_vtbl[3])(Unsafe.AsPointer(ref this));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetEnableGPUBasedValidation(bool Enable) => ((delegate* unmanaged[Stdcall]<void*, int, void>)_vtbl[4])(Unsafe.AsPointer(ref this), Enable ? 1 : 0);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetEnableSynchronizedCommandQueueValidation(bool Enable) => ((delegate* unmanaged[Stdcall]<void*, int, void>)_vtbl[5])(Unsafe.AsPointer(ref this), Enable ? 1 : 0);
}
