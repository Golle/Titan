using System;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32.D3D11
{
    public unsafe struct IDXGISwapChain
    {
        private void** _vtbl;

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

        //HRESULT(STDMETHODCALLTYPE* SetFullscreenState)(
        // IDXGISwapChain* This,
        // /* [in] */ BOOL Fullscreen,
        // /* [annotation][in] */
        // _In_opt_ IDXGIOutput * pTarget);

        //HRESULT(STDMETHODCALLTYPE* GetFullscreenState)(
        // IDXGISwapChain* This,
        // /* [annotation][out] */
        // _Out_opt_ BOOL * pFullscreen,
        // /* [annotation][out] */
        // _COM_Outptr_opt_result_maybenull_  IDXGIOutput** ppTarget);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT GetDesc(DXGI_SWAP_CHAIN_DESC * pDesc) => ((delegate* unmanaged[Stdcall]<void*, DXGI_SWAP_CHAIN_DESC*, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), pDesc);

        //HRESULT(STDMETHODCALLTYPE* ResizeBuffers)(
        // IDXGISwapChain* This,
        // /* [in] */ UINT BufferCount,
        // /* [in] */ UINT Width,
        // /* [in] */ UINT Height,
        // /* [in] */ DXGI_FORMAT NewFormat,
        // /* [in] */ UINT SwapChainFlags);

        //HRESULT(STDMETHODCALLTYPE* ResizeTarget)(
        // IDXGISwapChain* This,
        // /* [annotation][in] */
        // _In_  const DXGI_MODE_DESC* pNewTargetParameters);

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
    }
}
