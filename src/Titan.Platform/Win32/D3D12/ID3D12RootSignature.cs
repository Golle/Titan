using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

[Guid("c54a6b66-72df-4ee8-8be5-a946a1429214")]
public unsafe struct ID3D12RootSignature : INativeGuid
{
    public static Guid* Guid => IID.IID_ID3D12RootSignature;
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //DECLSPEC_XFGVIRT(ID3D12Object, GetPrivateData)
    //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    //ID3D12RootSignature* This,
    //_In_ REFGUID guid,
    //_Inout_  UINT* pDataSize,
    //_Out_writes_bytes_opt_( *pDataSize )  void* pData);

    //DECLSPEC_XFGVIRT(ID3D12Object, SetPrivateData)
    //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    //ID3D12RootSignature* This,
    //_In_ REFGUID guid,
    //_In_  UINT DataSize,
    //_In_reads_bytes_opt_( DataSize )  const void* pData);

    //DECLSPEC_XFGVIRT(ID3D12Object, SetPrivateDataInterface)
    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    //ID3D12RootSignature* This,
    //_In_ REFGUID guid,
    //_In_opt_  const IUnknown* pData);

    //DECLSPEC_XFGVIRT(ID3D12Object, SetName)
    //HRESULT(STDMETHODCALLTYPE* SetName)(
    //ID3D12RootSignature* This,
    //_In_z_ LPCWSTR Name);

    //DECLSPEC_XFGVIRT(ID3D12DeviceChild, GetDevice)
    //HRESULT(STDMETHODCALLTYPE* GetDevice)(
    //ID3D12RootSignature* This,
    //REFIID riid,
    //_COM_Outptr_opt_  void** ppvDevice);
}
