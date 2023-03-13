using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D12;

namespace Titan.Platform.Win32.WIC;

[Guid("9EDDE9E7-8DEE-47ea-99DF-E6FAF2ED44BF")]
public unsafe struct IWICBitmapDecoder : INativeGuid
{
    public static Guid* Guid => IID.IID_IWICBitmapDecoder;
    private void** _vtbl;

    //HRESULT(STDMETHODCALLTYPE* QueryInterface)(
    // __RPC__in IWICBitmapDecoder * This,
    // /* [in] */ __RPC__in REFIID riid,
    // /* [annotation][iid_is][out] */
    // _COM_Outptr_  void** ppvObject);

    //ULONG(STDMETHODCALLTYPE* AddRef)(
    // __RPC__in IWICBitmapDecoder * This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate * unmanaged[Stdcall] <void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //HRESULT(STDMETHODCALLTYPE* QueryCapability)(
    // __RPC__in IWICBitmapDecoder * This,
    // /* [in] */ __RPC__in_opt IStream* pIStream,
    // /* [out] */ __RPC__out DWORD* pdwCapability);

    //HRESULT(STDMETHODCALLTYPE* Initialize)(
    // __RPC__in IWICBitmapDecoder * This,
    // /* [in] */ __RPC__in_opt IStream* pIStream,
    // /* [in] */ WICDecodeOptions cacheOptions);

    //HRESULT(STDMETHODCALLTYPE* GetContainerFormat)(
    // __RPC__in IWICBitmapDecoder * This,
    // /* [out] */ __RPC__out GUID* pguidContainerFormat);

    //HRESULT(STDMETHODCALLTYPE* GetDecoderInfo)(
    // __RPC__in IWICBitmapDecoder * This,
    // /* [out] */ __RPC__deref_out_opt IWICBitmapDecoderInfo** ppIDecoderInfo);

    //HRESULT(STDMETHODCALLTYPE* CopyPalette)(
    // __RPC__in IWICBitmapDecoder * This,
    // /* [in] */ __RPC__in_opt IWICPalette* pIPalette);

    //HRESULT(STDMETHODCALLTYPE* GetMetadataQueryReader)(
    // __RPC__in IWICBitmapDecoder * This,
    // /* [out] */ __RPC__deref_out_opt IWICMetadataQueryReader** ppIMetadataQueryReader);

    //HRESULT(STDMETHODCALLTYPE* GetPreview)(
    // __RPC__in IWICBitmapDecoder * This,
    // /* [out] */ __RPC__deref_out_opt IWICBitmapSource** ppIBitmapSource);

    //HRESULT(STDMETHODCALLTYPE* GetColorContexts)(
    // __RPC__in IWICBitmapDecoder * This,
    // /* [in] */ UINT cCount,
    //    /* [size_is][unique][out][in] */ __RPC__inout_ecount_full_opt(cCount) IWICColorContext** ppIColorContexts,
    //    /* [out] */ __RPC__out UINT* pcActualCount);

    //HRESULT(STDMETHODCALLTYPE* GetThumbnail)(
    // __RPC__in IWICBitmapDecoder * This,
    // /* [out] */ __RPC__deref_out_opt IWICBitmapSource** ppIThumbnail);

    //HRESULT(STDMETHODCALLTYPE* GetFrameCount)(
    // __RPC__in IWICBitmapDecoder * This,
    // /* [out] */ __RPC__out UINT* pCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetFrame(uint index, IWICBitmapFrameDecode** ppIBitmapFrame) => ((delegate * unmanaged[Stdcall] <void*, uint, IWICBitmapFrameDecode**, HRESULT>)_vtbl[13])(Unsafe.AsPointer(ref this), index, ppIBitmapFrame);
}
