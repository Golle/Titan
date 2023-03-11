using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.WIC;

public unsafe struct IWICBitmapFrameDecode
{
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject) => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetSize(uint* puiWidth, uint* puiHeight) => ((delegate* unmanaged[Stdcall]<void*,uint*, uint*, HRESULT>) _vtbl[3])(Unsafe.AsPointer(ref this), puiWidth, puiHeight);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetPixelFormat(Guid* pPixelFormat) => ((delegate* unmanaged[Stdcall]<void*, Guid*, HRESULT>)_vtbl[4])(Unsafe.AsPointer(ref this), pPixelFormat);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetResolution(double* pDpiX, double* pDpiY) => ((delegate* unmanaged[Stdcall]<void*, double*, double*, HRESULT>)_vtbl[5])(Unsafe.AsPointer(ref this), pDpiX, pDpiY);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CopyPalette(IWICPalette* pIPalette) => ((delegate* unmanaged[Stdcall]<void*, IWICPalette*, HRESULT>)_vtbl[6])(Unsafe.AsPointer(ref this), pIPalette);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CopyPixels(WICRect* prc, uint cbStride, uint cbBufferSize, byte* pbBuffer) => ((delegate* unmanaged[Stdcall]<void*, WICRect*, uint, uint, byte*, HRESULT>)_vtbl[7])(Unsafe.AsPointer(ref this), prc, cbStride, cbBufferSize, pbBuffer);

    //HRESULT(STDMETHODCALLTYPE* GetMetadataQueryReader)(
    // __RPC__in IWICBitmapFrameDecode * This,
    // /* [out] */ __RPC__deref_out_opt IWICMetadataQueryReader** ppIMetadataQueryReader);

    //HRESULT(STDMETHODCALLTYPE* GetColorContexts)(
    // __RPC__in IWICBitmapFrameDecode * This,
    // /* [in] */ UINT cCount,
    //    /* [size_is][unique][out][in] */ __RPC__inout_ecount_full_opt(cCount) IWICColorContext** ppIColorContexts,
    //    /* [out] */ __RPC__out UINT* pcActualCount);

    //HRESULT(STDMETHODCALLTYPE* GetThumbnail)(
    // __RPC__in IWICBitmapFrameDecode * This,
    // /* [out] */ __RPC__deref_out_opt IWICBitmapSource** ppIThumbnail);
}
