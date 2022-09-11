using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming

namespace Titan.Platform.Win32.DXGI
{
    public unsafe struct IDXGISwapChain1
    {
        private void** _vtbl;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT QueryInterface(in Guid riid, void** ppvObject) => ((delegate* unmanaged[Stdcall]<void*, in Guid, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

        //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
        //  IDXGISwapChain* This,
        //  /* [annotation][in] */
        //  _In_ REFGUID Name,
        //  /* [in] */ UINT DataSize,
        //    /* [annotation][in] */ 
        //    _In_reads_bytes_(DataSize)  const void* pData);

        //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
        // IDXGISwapChain* This,
        // /* [annotation][in] */
        // _In_ REFGUID Name,
        // /* [annotation][in] */
        // _In_opt_  const IUnknown* pUnknown);

        //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
        // IDXGISwapChain* This,
        // /* [annotation][in] */
        // _In_ REFGUID Name,
        // /* [annotation][out][in] */
        // _Inout_  UINT* pDataSize,
        // /* [annotation][out] */
        // _Out_writes_bytes_(*pDataSize)  void* pData);

        //HRESULT(STDMETHODCALLTYPE* GetParent)(
        // IDXGISwapChain* This,
        // /* [annotation][in] */
        // _In_ REFIID riid,
        // /* [annotation][retval][out] */
        // _COM_Outptr_  void** ppParent);

        //HRESULT(STDMETHODCALLTYPE* GetDevice)(
        // IDXGISwapChain* This,
        // /* [annotation][in] */
        // _In_ REFIID riid,
        // /* [annotation][retval][out] */
        // _COM_Outptr_  void** ppDevice);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT Present(uint syncInterval, uint flags) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, HRESULT>)_vtbl[8])(Unsafe.AsPointer(ref this), syncInterval, flags);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT GetBuffer(uint buffer, Guid* riid, void** ppSurface) => ((delegate* unmanaged[Stdcall]<void*, uint, Guid*, void**, HRESULT>)_vtbl[9])(Unsafe.AsPointer(ref this), buffer, riid, ppSurface);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT SetFullscreenState([MarshalAs(UnmanagedType.Bool)] bool Fullscreen, IDXGIOutput* pTarget) =>
            ((delegate* unmanaged[Stdcall]<void*, bool, IDXGIOutput*, HRESULT>)_vtbl[10])(Unsafe.AsPointer(ref this), Fullscreen, pTarget);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT GetFullscreenState([MarshalAs(UnmanagedType.Bool)] bool* pFullscreen, IDXGIOutput** ppTarget) =>
            ((delegate* unmanaged[Stdcall]<void*, bool*, IDXGIOutput**, HRESULT>)_vtbl[11])(Unsafe.AsPointer(ref this), pFullscreen, ppTarget);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT GetDesc(DXGI_SWAP_CHAIN_DESC* pDesc) => ((delegate* unmanaged[Stdcall]<void*, DXGI_SWAP_CHAIN_DESC*, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), pDesc);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT ResizeBuffers(uint bufferCount, uint width, uint height, DXGI_FORMAT newFormat, uint swapChainFlags) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, DXGI_FORMAT, uint, HRESULT>)_vtbl[13])(Unsafe.AsPointer(ref this), bufferCount, width, height, newFormat, swapChainFlags);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT ResizeTarget(DXGI_MODE_DESC* pNewTargetParameters) => ((delegate* unmanaged[Stdcall]<void*, DXGI_MODE_DESC*, HRESULT>)_vtbl[14])(Unsafe.AsPointer(ref this), pNewTargetParameters);

        //HRESULT(STDMETHODCALLTYPE* GetContainingOutput)(
        // IDXGISwapChain* This,
        // /* [annotation][out] */
        // _COM_Outptr_ IDXGIOutput ** ppOutput);

        //HRESULT(STDMETHODCALLTYPE* GetFrameStatistics)(
        // IDXGISwapChain* This,
        // /* [annotation][out] */
        // _Out_ DXGI_FRAME_STATISTICS * pStats);

        //HRESULT(STDMETHODCALLTYPE* GetLastPresentCount)(
        // IDXGISwapChain* This,
        // /* [annotation][out] */
        // _Out_ UINT * pLastPresentCount);

        //HRESULT(STDMETHODCALLTYPE* GetDesc1)(
        // IDXGISwapChain1* This,
        // /* [annotation][out] */
        // _Out_ DXGI_SWAP_CHAIN_DESC1 * pDesc);

        //HRESULT(STDMETHODCALLTYPE* GetFullscreenDesc)(
        // IDXGISwapChain1* This,
        // /* [annotation][out] */
        // _Out_ DXGI_SWAP_CHAIN_FULLSCREEN_DESC * pDesc);

        //HRESULT(STDMETHODCALLTYPE* GetHwnd)(
        // IDXGISwapChain1* This,
        // /* [annotation][out] */
        // _Out_ HWND * pHwnd);

        //HRESULT(STDMETHODCALLTYPE* GetCoreWindow)(
        // IDXGISwapChain1* This,
        // /* [annotation][in] */
        // _In_ REFIID refiid,
        // /* [annotation][out] */
        // _COM_Outptr_  void** ppUnk);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT Present1(uint syncInterval, uint flags, DXGI_PRESENT_PARAMETERS* pPresentParameters) => ((delegate* unmanaged[Stdcall]<void*, uint, uint, DXGI_PRESENT_PARAMETERS*, HRESULT>)_vtbl[22])(Unsafe.AsPointer(ref this), syncInterval, flags, pPresentParameters);

        //BOOL(STDMETHODCALLTYPE* IsTemporaryMonoSupported)(
        // IDXGISwapChain1* This);

        //HRESULT(STDMETHODCALLTYPE* GetRestrictToOutput)(
        // IDXGISwapChain1* This,
        // /* [annotation][out] */
        // _Out_ IDXGIOutput ** ppRestrictToOutput);

        //HRESULT(STDMETHODCALLTYPE* SetBackgroundColor)(
        // IDXGISwapChain1* This,
        // /* [annotation][in] */
        // _In_  const DXGI_RGBA* pColor);

        //HRESULT(STDMETHODCALLTYPE* GetBackgroundColor)(
        // IDXGISwapChain1* This,
        // /* [annotation][out] */
        // _Out_ DXGI_RGBA * pColor);

        //HRESULT(STDMETHODCALLTYPE* SetRotation)(
        // IDXGISwapChain1* This,
        // /* [annotation][in] */
        // _In_ DXGI_MODE_ROTATION Rotation);

        //HRESULT(STDMETHODCALLTYPE* GetRotation)(
        // IDXGISwapChain1* This,
        // /* [annotation][out] */
        // _Out_ DXGI_MODE_ROTATION * pRotation);
    }
}
    
