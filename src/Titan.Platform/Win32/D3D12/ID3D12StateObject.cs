using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

[Guid("47016943-fca8-4594-93ea-af258b55346d")]
public unsafe struct ID3D12StateObject : INativeGuid
{
    public static Guid* Guid => IID.IID_ID3D12StateObject;
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetPrivateData(Guid* guid, uint* pDataSize, void* pData)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, uint*, void*, HRESULT>)_vtbl[3])(Unsafe.AsPointer(ref this), guid, pDataSize, pData);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetPrivateData(Guid* guid, uint DataSize, void* pData)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, uint, void*, HRESULT>)_vtbl[4])(Unsafe.AsPointer(ref this), guid, DataSize, pData);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetPrivateDataInterface(Guid* guid, IUnknown* pData)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, IUnknown*, HRESULT>)_vtbl[5])(Unsafe.AsPointer(ref this), guid, pData);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetName(char* Name)
        => ((delegate* unmanaged[Stdcall]<void*, char*, HRESULT>)_vtbl[6])(Unsafe.AsPointer(ref this), Name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetDevice(Guid* riid, void** ppvDevice)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[7])(Unsafe.AsPointer(ref this), riid, ppvDevice);
}
