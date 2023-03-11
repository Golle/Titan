using System.Runtime.CompilerServices;


namespace Titan.Platform.Win32.DXGI;

public unsafe struct IDXGIFactory2
{
    public static readonly Guid UUID = new("50c83a1c-e072-4c48-87b0-3630fa36a6d0");
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(in Guid riid, void** ppvObject) => ((delegate* unmanaged[Stdcall]<void*, in Guid, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //    HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    //     IDXGIFactory1* This,
    //     /* [annotation][in] */
    //     _In_ REFGUID Name,
    //     /* [in] */ UINT DataSize,
    //        /* [annotation][in] */ 
    //        _In_reads_bytes_(DataSize)  const void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    // IDXGIFactory1* This,
    // /* [annotation][in] */
    // _In_ REFGUID Name,
    // /* [annotation][in] */
    // _In_opt_  const IUnknown* pUnknown);

    //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    // IDXGIFactory1* This,
    // /* [annotation][in] */
    // _In_ REFGUID Name,
    // /* [annotation][out][in] */
    // _Inout_  UINT* pDataSize,
    // /* [annotation][out] */
    // _Out_writes_bytes_(*pDataSize)  void* pData);

    //HRESULT(STDMETHODCALLTYPE* GetParent)(
    // IDXGIFactory1* This,
    // /* [annotation][in] */
    // _In_ REFIID riid,
    // /* [annotation][retval][out] */
    // _COM_Outptr_  void** ppParent);

    //HRESULT(STDMETHODCALLTYPE* EnumAdapters)(
    // IDXGIFactory1* This,
    // /* [in] */ UINT Adapter,
    // /* [annotation][out] */
    // _COM_Outptr_ IDXGIAdapter ** ppAdapter);

    //HRESULT(STDMETHODCALLTYPE* MakeWindowAssociation)(
    // IDXGIFactory1* This,
    // HWND WindowHandle,
    // UINT Flags);

    //    HRESULT(STDMETHODCALLTYPE* GetWindowAssociation)(
    //     IDXGIFactory1* This,
    //     /* [annotation][out] */
    //     _Out_ HWND * pWindowHandle);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateSwapChain(IUnknown* pDevice, DXGI_SWAP_CHAIN_DESC* pDesc, IDXGISwapChain** ppSwapChain) =>
        ((delegate* unmanaged[Stdcall]<void*, IUnknown*, DXGI_SWAP_CHAIN_DESC*, IDXGISwapChain**, HRESULT>)_vtbl[10])(Unsafe.AsPointer(ref this), pDevice, pDesc, ppSwapChain);

    //HRESULT(STDMETHODCALLTYPE* CreateSoftwareAdapter)(
    // IDXGIFactory1* This,
    // /* [in] */ HMODULE Module,
    // /* [annotation][out] */
    // _COM_Outptr_ IDXGIAdapter ** ppAdapter);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT EnumAdapters1(uint adapter, IDXGIAdapter1** ppAdapter) =>
        ((delegate* unmanaged[Stdcall]<void*, uint, IDXGIAdapter1**, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), adapter, ppAdapter);

    //BOOL(STDMETHODCALLTYPE* IsCurrent)(
    // IDXGIFactory1* This);


    //BOOL(STDMETHODCALLTYPE* IsWindowedStereoEnabled)(
    //         IDXGIFactory2* This);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateSwapChainForHwnd(IUnknown * pDevice, HWND hWnd, DXGI_SWAP_CHAIN_DESC1* pDesc, DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pFullscreenDesc, IDXGIOutput* pRestrictToOutput, IDXGISwapChain1** ppSwapChain) =>
        ((delegate* unmanaged[Stdcall]<void*, IUnknown*, HWND, DXGI_SWAP_CHAIN_DESC1*, DXGI_SWAP_CHAIN_FULLSCREEN_DESC*, IDXGIOutput*, IDXGISwapChain1**, HRESULT>)_vtbl[15])(Unsafe.AsPointer(ref this), pDevice, hWnd, pDesc, pFullscreenDesc, pRestrictToOutput, ppSwapChain);

    //        HRESULT(STDMETHODCALLTYPE* CreateSwapChainForCoreWindow)(
    //         IDXGIFactory2* This,
    //         /* [annotation][in] */
    //         _In_ IUnknown * pDevice,
    //         /* [annotation][in] */
    //         _In_  IUnknown* pWindow,
    //         /* [annotation][in] */
    //         _In_  const DXGI_SWAP_CHAIN_DESC1* pDesc,
    //         /* [annotation][in] */
    //         _In_opt_  IDXGIOutput* pRestrictToOutput,
    //         /* [annotation][out] */
    //         _COM_Outptr_  IDXGISwapChain1** ppSwapChain);

    //        HRESULT(STDMETHODCALLTYPE* GetSharedResourceAdapterLuid)(
    //         IDXGIFactory2* This,
    //         /* [annotation] */
    //         _In_ HANDLE hResource,
    //         /* [annotation] */
    //         _Out_  LUID* pLuid);

    //        HRESULT(STDMETHODCALLTYPE* RegisterStereoStatusWindow)(
    //         IDXGIFactory2* This,
    //         /* [annotation][in] */
    //         _In_ HWND WindowHandle,
    //         /* [annotation][in] */
    //         _In_  UINT wMsg,
    //         /* [annotation][out] */
    //         _Out_  DWORD* pdwCookie);

    //        HRESULT(STDMETHODCALLTYPE* RegisterStereoStatusEvent)(
    //         IDXGIFactory2* This,
    //         /* [annotation][in] */
    //         _In_ HANDLE hEvent,
    //         /* [annotation][out] */
    //         _Out_  DWORD* pdwCookie);

    //        void (STDMETHODCALLTYPE* UnregisterStereoStatus ) (
    //            IDXGIFactory2* This,
    //            /* [annotation][in] */
    //            _In_ DWORD dwCookie);

    //        HRESULT(STDMETHODCALLTYPE* RegisterOcclusionStatusWindow)(
    //         IDXGIFactory2* This,
    //         /* [annotation][in] */
    //         _In_ HWND WindowHandle,
    //         /* [annotation][in] */
    //         _In_  UINT wMsg,
    //         /* [annotation][out] */
    //         _Out_  DWORD* pdwCookie);

    //        HRESULT(STDMETHODCALLTYPE* RegisterOcclusionStatusEvent)(
    //         IDXGIFactory2* This,
    //         /* [annotation][in] */
    //         _In_ HANDLE hEvent,
    //         /* [annotation][out] */
    //         _Out_  DWORD* pdwCookie);

    //        void (STDMETHODCALLTYPE* UnregisterOcclusionStatus ) (
    //            IDXGIFactory2* This,
    //            /* [annotation][in] */
    //            _In_ DWORD dwCookie);

    //        HRESULT(STDMETHODCALLTYPE* CreateSwapChainForComposition)(
    //         IDXGIFactory2* This,
    //         /* [annotation][in] */
    //         _In_ IUnknown * pDevice,
    //         /* [annotation][in] */
    //         _In_  const DXGI_SWAP_CHAIN_DESC1* pDesc,
    //         /* [annotation][in] */
    //         _In_opt_  IDXGIOutput* pRestrictToOutput,
    //         /* [annotation][out] */
    //         _COM_Outptr_  IDXGISwapChain1** ppSwapChain);

}
