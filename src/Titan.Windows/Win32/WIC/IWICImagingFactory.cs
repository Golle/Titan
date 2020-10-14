using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Titan.Windows.Win32.WIC
{
    [Guid("ec5ec8a9-c395-4314-9c77-54d7a935ff70")]
    public unsafe struct IWICImagingFactory
    {
        private void** _vtbl;

        //HRESULT(STDMETHODCALLTYPE* QueryInterface)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in REFIID riid,
        // /* [annotation][iid_is][out] */
        // _COM_Outptr_  void** ppvObject);

        //ULONG(STDMETHODCALLTYPE* AddRef)(
        // __RPC__in IWICImagingFactory * This);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public HRESULT CreateDecoderFromFilename(char* wzFilename, Guid* pguidVendor, uint dwDesiredAccess, WICDecodeOptions metadataOptions, IWICBitmapDecoder** ppIDecoder)
        => ((delegate* unmanaged[Stdcall]<void*, char*, Guid*, uint, WICDecodeOptions, IWICBitmapDecoder**,  HRESULT>)_vtbl[2])(Unsafe.AsPointer(ref this), wzFilename, pguidVendor, dwDesiredAccess, metadataOptions, ppIDecoder);

        //HRESULT(STDMETHODCALLTYPE* CreateDecoderFromStream)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in_opt IStream* pIStream,
        // /* [unique][in] */ __RPC__in_opt const GUID* pguidVendor,
        // /* [in] */ WICDecodeOptions metadataOptions,
        //    /* [retval][out] */ __RPC__deref_out_opt IWICBitmapDecoder **ppIDecoder);

        //HRESULT(STDMETHODCALLTYPE* CreateDecoderFromFileHandle)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ ULONG_PTR hFile,
        //    /* [unique][in] */ __RPC__in_opt const GUID* pguidVendor,
        //    /* [in] */ WICDecodeOptions metadataOptions,
        //    /* [retval][out] */ __RPC__deref_out_opt IWICBitmapDecoder **ppIDecoder);

        //HRESULT(STDMETHODCALLTYPE* CreateComponentInfo)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in REFCLSID clsidComponent,
        // /* [out] */ __RPC__deref_out_opt IWICComponentInfo** ppIInfo);

        //HRESULT(STDMETHODCALLTYPE* CreateDecoder)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in REFGUID guidContainerFormat,
        // /* [unique][in] */ __RPC__in_opt const GUID* pguidVendor,
        // /* [retval][out] */ __RPC__deref_out_opt IWICBitmapDecoder** ppIDecoder);

        //HRESULT(STDMETHODCALLTYPE* CreateEncoder)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in REFGUID guidContainerFormat,
        // /* [unique][in] */ __RPC__in_opt const GUID* pguidVendor,
        // /* [retval][out] */ __RPC__deref_out_opt IWICBitmapEncoder** ppIEncoder);

        //HRESULT(STDMETHODCALLTYPE* CreatePalette)(
        // __RPC__in IWICImagingFactory * This,
        // /* [out] */ __RPC__deref_out_opt IWICPalette** ppIPalette);

        //HRESULT(STDMETHODCALLTYPE* CreateFormatConverter)(
        // __RPC__in IWICImagingFactory * This,
        // /* [out] */ __RPC__deref_out_opt IWICFormatConverter** ppIFormatConverter);

        //HRESULT(STDMETHODCALLTYPE* CreateBitmapScaler)(
        // __RPC__in IWICImagingFactory * This,
        // /* [out] */ __RPC__deref_out_opt IWICBitmapScaler** ppIBitmapScaler);

        //HRESULT(STDMETHODCALLTYPE* CreateBitmapClipper)(
        // __RPC__in IWICImagingFactory * This,
        // /* [out] */ __RPC__deref_out_opt IWICBitmapClipper** ppIBitmapClipper);

        //HRESULT(STDMETHODCALLTYPE* CreateBitmapFlipRotator)(
        // __RPC__in IWICImagingFactory * This,
        // /* [out] */ __RPC__deref_out_opt IWICBitmapFlipRotator** ppIBitmapFlipRotator);

        //HRESULT(STDMETHODCALLTYPE* CreateStream)(
        // __RPC__in IWICImagingFactory * This,
        // /* [out] */ __RPC__deref_out_opt IWICStream** ppIWICStream);

        //HRESULT(STDMETHODCALLTYPE* CreateColorContext)(
        // __RPC__in IWICImagingFactory * This,
        // /* [out] */ __RPC__deref_out_opt IWICColorContext** ppIWICColorContext);

        //HRESULT(STDMETHODCALLTYPE* CreateColorTransformer)(
        // __RPC__in IWICImagingFactory * This,
        // /* [out] */ __RPC__deref_out_opt IWICColorTransform** ppIWICColorTransform);

        //HRESULT(STDMETHODCALLTYPE* CreateBitmap)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ UINT uiWidth,
        //    /* [in] */ UINT uiHeight,
        //    /* [in] */ __RPC__in REFWICPixelFormatGUID pixelFormat,
        //    /* [in] */ WICBitmapCreateCacheOption option,
        //    /* [out] */ __RPC__deref_out_opt IWICBitmap **ppIBitmap);

        //HRESULT(STDMETHODCALLTYPE* CreateBitmapFromSource)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in_opt IWICBitmapSource* pIBitmapSource,
        // /* [in] */ WICBitmapCreateCacheOption option,
        //    /* [out] */ __RPC__deref_out_opt IWICBitmap **ppIBitmap);

        //HRESULT(STDMETHODCALLTYPE* CreateBitmapFromSourceRect)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in_opt IWICBitmapSource* pIBitmapSource,
        // /* [in] */ UINT x,
        //    /* [in] */ UINT y,
        //    /* [in] */ UINT width,
        //    /* [in] */ UINT height,
        //    /* [out] */ __RPC__deref_out_opt IWICBitmap** ppIBitmap);

        //HRESULT(STDMETHODCALLTYPE* CreateBitmapFromMemory)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ UINT uiWidth,
        //    /* [in] */ UINT uiHeight,
        //    /* [in] */ __RPC__in REFWICPixelFormatGUID pixelFormat,
        //    /* [in] */ UINT cbStride,
        //    /* [in] */ UINT cbBufferSize,
        //    /* [size_is][in] */ __RPC__in_ecount_full(cbBufferSize) BYTE* pbBuffer,
        //    /* [out] */ __RPC__deref_out_opt IWICBitmap** ppIBitmap);

        //HRESULT(STDMETHODCALLTYPE* CreateBitmapFromHBITMAP)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in HBITMAP hBitmap,
        // /* [unique][in] */ __RPC__in_opt HPALETTE hPalette,
        // /* [in] */ WICBitmapAlphaChannelOption options,
        //    /* [out] */ __RPC__deref_out_opt IWICBitmap **ppIBitmap);

        //HRESULT(STDMETHODCALLTYPE* CreateBitmapFromHICON)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in HICON hIcon,
        // /* [out] */ __RPC__deref_out_opt IWICBitmap** ppIBitmap);

        //HRESULT(STDMETHODCALLTYPE* CreateComponentEnumerator)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ DWORD componentTypes,
        //    /* [in] */ DWORD options,
        //    /* [out] */ __RPC__deref_out_opt IEnumUnknown** ppIEnumUnknown);

        //HRESULT(STDMETHODCALLTYPE* CreateFastMetadataEncoderFromDecoder)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in_opt IWICBitmapDecoder* pIDecoder,
        // /* [out] */ __RPC__deref_out_opt IWICFastMetadataEncoder** ppIFastEncoder);

        //HRESULT(STDMETHODCALLTYPE* CreateFastMetadataEncoderFromFrameDecode)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in_opt IWICBitmapFrameDecode* pIFrameDecoder,
        // /* [out] */ __RPC__deref_out_opt IWICFastMetadataEncoder** ppIFastEncoder);

        //HRESULT(STDMETHODCALLTYPE* CreateQueryWriter)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in REFGUID guidMetadataFormat,
        // /* [unique][in] */ __RPC__in_opt const GUID* pguidVendor,
        // /* [out] */ __RPC__deref_out_opt IWICMetadataQueryWriter** ppIQueryWriter);

        //HRESULT(STDMETHODCALLTYPE* CreateQueryWriterFromReader)(
        // __RPC__in IWICImagingFactory * This,
        // /* [in] */ __RPC__in_opt IWICMetadataQueryReader* pIQueryReader,
        // /* [unique][in] */ __RPC__in_opt const GUID* pguidVendor,
        // /* [out] */ __RPC__deref_out_opt IWICMetadataQueryWriter** ppIQueryWriter);
    }
}
