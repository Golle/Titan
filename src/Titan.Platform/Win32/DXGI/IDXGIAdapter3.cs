using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D12;

namespace Titan.Platform.Win32.DXGI;
[Guid("645967A4-1392-4310-A798-8053CE3E93FD")]
public unsafe struct IDXGIAdapter3 : INativeGuid
{
    public static Guid* Guid => IID.IID_IDXGIAdapter3;
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

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

    //DECLSPEC_XFGVIRT(IDXGIAdapter2, GetDesc2)
    //    HRESULT(STDMETHODCALLTYPE* GetDesc2)(
    //        IDXGIAdapter3* This,
    //        /* [annotation][out] */
    //        _Out_ DXGI_ADAPTER_DESC2 * pDesc);

    //DECLSPEC_XFGVIRT(IDXGIAdapter3, RegisterHardwareContentProtectionTeardownStatusEvent)
    //    HRESULT(STDMETHODCALLTYPE* RegisterHardwareContentProtectionTeardownStatusEvent)(
    //        IDXGIAdapter3* This,
    //        /* [annotation][in] */
    //        _In_ HANDLE hEvent,
    //        /* [annotation][out] */
    //        _Out_  DWORD* pdwCookie);

    //DECLSPEC_XFGVIRT(IDXGIAdapter3, UnregisterHardwareContentProtectionTeardownStatus)
    //    void (STDMETHODCALLTYPE* UnregisterHardwareContentProtectionTeardownStatus ) (
    //        IDXGIAdapter3* This,
    //        /* [annotation][in] */
    //        _In_ DWORD dwCookie);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryVideoMemoryInfo(uint NodeIndex, DXGI_MEMORY_SEGMENT_GROUP MemorySegmentGroup, DXGI_QUERY_VIDEO_MEMORY_INFO* pVideoMemoryInfo)
        => ((delegate* unmanaged[Stdcall]<void*, uint, DXGI_MEMORY_SEGMENT_GROUP, DXGI_QUERY_VIDEO_MEMORY_INFO*, HRESULT>)_vtbl[14])(Unsafe.AsPointer(ref this), NodeIndex, MemorySegmentGroup, pVideoMemoryInfo);

    //DECLSPEC_XFGVIRT(IDXGIAdapter3, SetVideoMemoryReservation)
    //    HRESULT(STDMETHODCALLTYPE* SetVideoMemoryReservation)(
    //        IDXGIAdapter3* This,
    //        /* [annotation][in] */
    //        _In_ UINT NodeIndex,
    //        /* [annotation][in] */
    //        _In_  DXGI_MEMORY_SEGMENT_GROUP MemorySegmentGroup,
    //        /* [annotation][in] */
    //        _In_  UINT64 Reservation);

    //DECLSPEC_XFGVIRT(IDXGIAdapter3, RegisterVideoMemoryBudgetChangeNotificationEvent)
    //    HRESULT(STDMETHODCALLTYPE* RegisterVideoMemoryBudgetChangeNotificationEvent)(
    //        IDXGIAdapter3* This,
    //        /* [annotation][in] */
    //        _In_ HANDLE hEvent,
    //        /* [annotation][out] */
    //        _Out_  DWORD* pdwCookie);

    //DECLSPEC_XFGVIRT(IDXGIAdapter3, UnregisterVideoMemoryBudgetChangeNotification)
    //    void (STDMETHODCALLTYPE* UnregisterVideoMemoryBudgetChangeNotification ) (
    //        IDXGIAdapter3* This,
    //        /* [annotation][in] */
    //        _In_ DWORD dwCookie);
    
}
