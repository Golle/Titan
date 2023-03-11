using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.DXGI;

[Guid("29038f61-3839-4626-91fd-086879011a05")]
public unsafe struct IDXGIAdapter1
{
    private void** _vtbl;

    //HRESULT(STDMETHODCALLTYPE* QueryInterface)(
    //     IDXGIAdapter1* This,
    //     /* [in] */ REFIID riid,
    //     /* [annotation][iid_is][out] */
    //     _COM_Outptr_  void** ppvObject);

    //ULONG(STDMETHODCALLTYPE* AddRef)(
    // IDXGIAdapter1* This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //    HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    //     IDXGIAdapter1* This,
    //     /* [annotation][in] */
    //     _In_ REFGUID Name,
    //     /* [in] */ UINT DataSize,
    //        /* [annotation][in] */ 
    //        _In_reads_bytes_(DataSize)  const void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    // IDXGIAdapter1* This,
    // /* [annotation][in] */
    // _In_ REFGUID Name,
    // /* [annotation][in] */
    // _In_opt_  const IUnknown* pUnknown);

    //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    // IDXGIAdapter1* This,
    // /* [annotation][in] */
    // _In_ REFGUID Name,
    // /* [annotation][out][in] */
    // _Inout_  UINT* pDataSize,
    // /* [annotation][out] */
    // _Out_writes_bytes_(*pDataSize)  void* pData);

    //HRESULT(STDMETHODCALLTYPE* GetParent)(
    // IDXGIAdapter1* This,
    // /* [annotation][in] */
    // _In_ REFIID riid,
    // /* [annotation][retval][out] */
    // _COM_Outptr_  void** ppParent);

    //HRESULT(STDMETHODCALLTYPE* EnumOutputs)(
    // IDXGIAdapter1* This,
    // /* [in] */ UINT Output,
    // /* [annotation][out][in] */
    // _COM_Outptr_ IDXGIOutput ** ppOutput);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetDesc(DXGI_ADAPTER_DESC* pDesc) => ((delegate* unmanaged[Stdcall]<void*, DXGI_ADAPTER_DESC*, HRESULT>)_vtbl[8])(Unsafe.AsPointer(ref this), pDesc);

    //HRESULT(STDMETHODCALLTYPE* CheckInterfaceSupport)(
    // IDXGIAdapter1* This,
    // /* [annotation][in] */
    // _In_ REFGUID InterfaceName,
    // /* [annotation][out] */
    // _Out_  LARGE_INTEGER* pUMDVersion);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetDesc1(DXGI_ADAPTER_DESC1* pDesc) => ((delegate* unmanaged[Stdcall]<void*, DXGI_ADAPTER_DESC1*, HRESULT>)_vtbl[10])(Unsafe.AsPointer(ref this), pDesc);
}
