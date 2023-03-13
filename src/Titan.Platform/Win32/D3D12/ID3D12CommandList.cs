using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

[Guid("7116d91c-e7e4-47ce-b8c6-ec8168f437e5")]
public unsafe struct ID3D12CommandList
{
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    //ID3D12CommandList* This,
    //_In_ REFGUID guid,
    //_Inout_  UINT* pDataSize,
    //_Out_writes_bytes_opt_( *pDataSize )  void* pData);

    //DECLSPEC_XFGVIRT(ID3D12Object, SetPrivateData)
    //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    //ID3D12CommandList* This,
    //_In_ REFGUID guid,
    //_In_  UINT DataSize,
    //_In_reads_bytes_opt_( DataSize )  const void* pData);

    //DECLSPEC_XFGVIRT(ID3D12Object, SetPrivateDataInterface)
    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    //ID3D12CommandList* This,
    //_In_ REFGUID guid,
    //_In_opt_  const IUnknown* pData);

    //DECLSPEC_XFGVIRT(ID3D12Object, SetName)
    //HRESULT(STDMETHODCALLTYPE* SetName)(
    //ID3D12CommandList* This,
    //_In_z_ LPCWSTR Name);

    //DECLSPEC_XFGVIRT(ID3D12DeviceChild, GetDevice)
    //HRESULT(STDMETHODCALLTYPE* GetDevice)(
    //ID3D12CommandList* This,
    //REFIID riid,
    //_COM_Outptr_opt_  void** ppvDevice);

    //DECLSPEC_XFGVIRT(ID3D12CommandList, GetType)
    //D3D12_COMMAND_LIST_TYPE(STDMETHODCALLTYPE* GetType)(
    //    ID3D12CommandList* This);

}
