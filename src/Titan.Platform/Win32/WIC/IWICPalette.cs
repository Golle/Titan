using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.WIC;

public unsafe struct IWICPalette
{
    private void** _vtbl;

    //HRESULT(STDMETHODCALLTYPE* QueryInterface)(
    // __RPC__in IWICPalette * This,
    // /* [in] */ __RPC__in REFIID riid,
    // /* [annotation][iid_is][out] */
    // _COM_Outptr_  void** ppvObject);

    //ULONG(STDMETHODCALLTYPE* AddRef)(
    // __RPC__in IWICPalette * This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //HRESULT(STDMETHODCALLTYPE* InitializePredefined)(
    // __RPC__in IWICPalette * This,
    // /* [in] */ WICBitmapPaletteType ePaletteType,
    //    /* [in] */ BOOL fAddTransparentColor);

    //HRESULT(STDMETHODCALLTYPE* InitializeCustom)(
    // __RPC__in IWICPalette * This,
    // /* [size_is][in] */ __RPC__in_ecount_full(cCount) WICColor* pColors,
    // /* [in] */ UINT cCount);
        
    //HRESULT(STDMETHODCALLTYPE* InitializeFromBitmap)(
    // __RPC__in IWICPalette * This,
    // /* [in] */ __RPC__in_opt IWICBitmapSource* pISurface,
    // /* [in] */ UINT cCount,
    //    /* [in] */ BOOL fAddTransparentColor);

    //HRESULT(STDMETHODCALLTYPE* InitializeFromPalette)(
    // __RPC__in IWICPalette * This,
    // /* [in] */ __RPC__in_opt IWICPalette* pIPalette);

    //HRESULT(STDMETHODCALLTYPE* GetType)(
    // __RPC__in IWICPalette * This,
    // /* [out] */ __RPC__out WICBitmapPaletteType* pePaletteType);

    //HRESULT(STDMETHODCALLTYPE* GetColorCount)(
    // __RPC__in IWICPalette * This,
    // /* [out] */ __RPC__out UINT* pcCount);

    //HRESULT(STDMETHODCALLTYPE* GetColors)(
    // __RPC__in IWICPalette * This,
    // /* [in] */ UINT cCount,
    //    /* [size_is][out] */ __RPC__out_ecount_full(cCount) WICColor* pColors,
    //    /* [out] */ __RPC__out UINT* pcActualColors);

    //HRESULT(STDMETHODCALLTYPE* IsBlackWhite)(
    // __RPC__in IWICPalette * This,
    // /* [out] */ __RPC__out BOOL* pfIsBlackWhite);

    //HRESULT(STDMETHODCALLTYPE* IsGrayscale)(
    // __RPC__in IWICPalette * This,
    // /* [out] */ __RPC__out BOOL* pfIsGrayscale);

    //HRESULT(STDMETHODCALLTYPE* HasAlpha)(
    // __RPC__in IWICPalette * This,
    // /* [out] */ __RPC__out BOOL* pfHasAlpha);
}
