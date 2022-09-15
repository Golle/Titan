using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Titan.Platform.Win32.D3D11;

public unsafe struct ID3D11Debug
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject) => ((delegate * unmanaged[Stdcall] <void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate * unmanaged[Stdcall] <void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //HRESULT(STDMETHODCALLTYPE* SetFeatureMask)(
    //    ID3D11Debug* This,
    //    UINT Mask);

    //UINT(STDMETHODCALLTYPE* GetFeatureMask)(
    //    ID3D11Debug* This);

    //HRESULT(STDMETHODCALLTYPE* SetPresentPerRenderOpDelay)(
    //    ID3D11Debug* This,
    //    UINT Milliseconds);

    //UINT(STDMETHODCALLTYPE* GetPresentPerRenderOpDelay)(
    //    ID3D11Debug* This);

    //HRESULT(STDMETHODCALLTYPE* SetSwapChain)(
    //ID3D11Debug* This,
    ///* [annotation] */
    //_In_opt_ IDXGISwapChain * pSwapChain);

    //HRESULT(STDMETHODCALLTYPE* GetSwapChain)(
    //ID3D11Debug* This,
    ///* [annotation] */
    //_Out_ IDXGISwapChain ** ppSwapChain);

    //HRESULT(STDMETHODCALLTYPE* ValidateContext)(
    //ID3D11Debug* This,
    ///* [annotation] */
    //_In_ ID3D11DeviceContext * pContext);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT ReportLiveDeviceObjects(D3D11_RLDO_FLAGS flags) => ((delegate* unmanaged[Stdcall]<void*, D3D11_RLDO_FLAGS, HRESULT>)_vtbl[10])(Unsafe.AsPointer(ref this), flags);

    //HRESULT(STDMETHODCALLTYPE* ValidateContextForDispatch)(
    //ID3D11Debug* This,
    ///* [annotation] */
    //_In_ ID3D11DeviceContext * pContext);
}
