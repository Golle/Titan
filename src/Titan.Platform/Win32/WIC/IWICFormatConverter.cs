using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.WIC;

public unsafe struct IWICFormatConverter
{
    private void** _vtbl;
    //HRESULT(STDMETHODCALLTYPE* QueryInterface)(
    // __RPC__in IWICFormatConverter * This,
    // /* [in] */ __RPC__in REFIID riid,
    // /* [annotation][iid_is][out] */
    // _COM_Outptr_  void** ppvObject);

    //ULONG(STDMETHODCALLTYPE* AddRef)(
    // __RPC__in IWICFormatConverter * This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //HRESULT(STDMETHODCALLTYPE* GetSize)(
    // __RPC__in IWICFormatConverter * This,
    // /* [out] */ __RPC__out UINT* puiWidth,
    // /* [out] */ __RPC__out UINT* puiHeight);

    //HRESULT(STDMETHODCALLTYPE* GetPixelFormat)(
    // __RPC__in IWICFormatConverter * This,
    // /* [out] */ __RPC__out WICPixelFormatGUID* pPixelFormat);

    //HRESULT(STDMETHODCALLTYPE* GetResolution)(
    // __RPC__in IWICFormatConverter * This,
    // /* [out] */ __RPC__out double* pDpiX,
    // /* [out] */ __RPC__out double* pDpiY);

    //HRESULT(STDMETHODCALLTYPE* CopyPalette)(
    // __RPC__in IWICFormatConverter * This,
    // /* [in] */ __RPC__in_opt IWICPalette* pIPalette);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CopyPixels(WICRect* prc, uint cbStride, uint cbBufferSize, byte* pbBuffer) =>
        ((delegate* unmanaged[Stdcall]<void*, WICRect*, uint, uint, byte*, HRESULT >)_vtbl[7])(Unsafe.AsPointer(ref this), prc, cbStride, cbBufferSize, pbBuffer);
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Initialize(IWICBitmapSource* pISource, Guid* dstFormat, WICBitmapDitherType dither, IWICPalette* pIPalette, double alphaThresholdPercent, WICBitmapPaletteType paletteTranslate) =>
        ((delegate* unmanaged[Stdcall]<void*, IWICBitmapSource*, Guid *, WICBitmapDitherType, IWICPalette *, double, WICBitmapPaletteType, HRESULT>)_vtbl[8])(Unsafe.AsPointer(ref this), pISource, dstFormat, dither, pIPalette, alphaThresholdPercent, paletteTranslate);
    //HRESULT(STDMETHODCALLTYPE* CanConvert)(
    // __RPC__in IWICFormatConverter * This,
    // /* [in] */ __RPC__in REFWICPixelFormatGUID srcPixelFormat,
    // /* [in] */ __RPC__in REFWICPixelFormatGUID dstPixelFormat,
    // /* [out] */ __RPC__out BOOL* pfCanConvert);

}
