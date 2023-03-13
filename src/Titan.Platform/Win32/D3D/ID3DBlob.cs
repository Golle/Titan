using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D12;

namespace Titan.Platform.Win32.D3D;

[Guid("8BA5FB08-5195-40e2-AC58-0D989C3A0102")]
public unsafe struct ID3DBlob : INativeGuid
{
    public static Guid* Guid => IID.IID_ID3DBlob;
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int QueryInterface(Guid* riid, void** ppvObject) => ((delegate* unmanaged<void*, Guid*, void**, int>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* GetBufferPointer() => ((delegate* unmanaged<void*, void*>)_vtbl[3])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nuint GetBufferSize() => ((delegate* unmanaged<void*, nuint>)_vtbl[4])(Unsafe.AsPointer(ref this));

}
