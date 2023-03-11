// ReSharper disable InconsistentNaming
namespace Titan.Platform.Win32.WIC;

public unsafe struct IWICBitmapSource
{
    private void** _vtbl;
    //    HRESULT(STDMETHODCALLTYPE* QueryInterface)(
    //__RPC__in IWICBitmapSource * This,
    ///* [in] */ __RPC__in REFIID riid,
    ///* [annotation][iid_is][out] */
    //_COM_Outptr_  void** ppvObject);

    //ULONG(STDMETHODCALLTYPE* AddRef)(
    //__RPC__in IWICBitmapSource * This);

    //ULONG(STDMETHODCALLTYPE* Release)(
    //__RPC__in IWICBitmapSource * This);

    //HRESULT(STDMETHODCALLTYPE* GetSize)(
    //__RPC__in IWICBitmapSource * This,
    ///* [out] */ __RPC__out UINT* puiWidth,
    ///* [out] */ __RPC__out UINT* puiHeight);

    //HRESULT(STDMETHODCALLTYPE* GetPixelFormat)(
    //__RPC__in IWICBitmapSource * This,
    ///* [out] */ __RPC__out WICPixelFormatGUID* pPixelFormat);

    //HRESULT(STDMETHODCALLTYPE* GetResolution)(
    //__RPC__in IWICBitmapSource * This,
    ///* [out] */ __RPC__out double* pDpiX,
    //    /* [out] */ __RPC__out double* pDpiY);

    //HRESULT(STDMETHODCALLTYPE* CopyPalette)(
    //__RPC__in IWICBitmapSource * This,
    ///* [in] */ __RPC__in_opt IWICPalette* pIPalette);

    //HRESULT(STDMETHODCALLTYPE* CopyPixels)(
    //__RPC__in IWICBitmapSource * This,
    ///* [unique][in] */ __RPC__in_opt const WICRect* prc,
    //    /* [in] */ UINT cbStride,
    ///* [in] */ UINT cbBufferSize,
    //    /* [size_is][out] */ __RPC__out_ecount_full(cbBufferSize) BYTE* pbBuffer);

}
