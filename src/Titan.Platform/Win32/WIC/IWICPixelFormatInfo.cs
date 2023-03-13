using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D12;

namespace Titan.Platform.Win32.WIC;

[Guid("E8EDA601-3D48-431a-AB44-69059BE88BBE")]
public unsafe struct IWICPixelFormatInfo : INativeGuid
{
    public static Guid* Guid => IID.IID_IWICPixelFormatInfo;
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject) => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //HRESULT(STDMETHODCALLTYPE* GetComponentType)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [out] */ __RPC__out WICComponentType* pType);

    //HRESULT(STDMETHODCALLTYPE* GetCLSID)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [out] */ __RPC__out CLSID* pclsid);

    //HRESULT(STDMETHODCALLTYPE* GetSigningStatus)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [out] */ __RPC__out DWORD* pStatus);

    //HRESULT(STDMETHODCALLTYPE* GetAuthor)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [in] */ UINT cchAuthor,
    //    /* [size_is][unique][out][in] */ __RPC__inout_ecount_full_opt(cchAuthor) WCHAR* wzAuthor,
    //    /* [out] */ __RPC__out UINT* pcchActual);

    //HRESULT(STDMETHODCALLTYPE* GetVendorGUID)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [out] */ __RPC__out GUID* pguidVendor);

    //HRESULT(STDMETHODCALLTYPE* GetVersion)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [in] */ UINT cchVersion,
    //    /* [size_is][unique][out][in] */ __RPC__inout_ecount_full_opt(cchVersion) WCHAR* wzVersion,
    //    /* [out] */ __RPC__out UINT* pcchActual);

    //HRESULT(STDMETHODCALLTYPE* GetSpecVersion)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [in] */ UINT cchSpecVersion,
    //    /* [size_is][unique][out][in] */ __RPC__inout_ecount_full_opt(cchSpecVersion) WCHAR* wzSpecVersion,
    //    /* [out] */ __RPC__out UINT* pcchActual);

    //HRESULT(STDMETHODCALLTYPE* GetFriendlyName)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [in] */ UINT cchFriendlyName,
    //    /* [size_is][unique][out][in] */ __RPC__inout_ecount_full_opt(cchFriendlyName) WCHAR* wzFriendlyName,
    //    /* [out] */ __RPC__out UINT* pcchActual);

    //HRESULT(STDMETHODCALLTYPE* GetFormatGUID)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [out] */ __RPC__out GUID* pFormat);

    //HRESULT(STDMETHODCALLTYPE* GetColorContext)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [out] */ __RPC__deref_out_opt IWICColorContext** ppIColorContext);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetBitsPerPixel(uint* puiBitsPerPixel) => ((delegate* unmanaged[Stdcall]<void*, uint*, HRESULT>)_vtbl[13])(Unsafe.AsPointer(ref this), puiBitsPerPixel);

    //HRESULT(STDMETHODCALLTYPE* GetChannelCount)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [out] */ __RPC__out UINT* puiChannelCount);

    //HRESULT(STDMETHODCALLTYPE* GetChannelMask)(
    // __RPC__in IWICPixelFormatInfo * This,
    // /* [in] */ UINT uiChannelIndex,
    //    /* [in] */ UINT cbMaskBuffer,
    //    /* [size_is][unique][out][in] */ __RPC__inout_ecount_full_opt(cbMaskBuffer) BYTE* pbMaskBuffer,
    //    /* [out] */ __RPC__out UINT* pcbActual);
}
