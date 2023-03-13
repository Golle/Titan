using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D12;

namespace Titan.Platform.Win32.WIC;

[Guid("23BC3F0A-698B-4357-886B-F24D50671334")]
public unsafe struct IWICComponentInfo : INativeGuid
{
    public static Guid* Guid => IID.IID_IWICComponentInfo;
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject) => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetComponentType(WICComponentType* pType) => ((delegate* unmanaged[Stdcall]<void*, WICComponentType*, HRESULT>)_vtbl[3])(Unsafe.AsPointer(ref this), pType);

    //HRESULT(STDMETHODCALLTYPE* GetCLSID)(
    // __RPC__in IWICComponentInfo * This,
    // /* [out] */ __RPC__out CLSID* pclsid);

    //HRESULT(STDMETHODCALLTYPE* GetSigningStatus)(
    // __RPC__in IWICComponentInfo * This,
    // /* [out] */ __RPC__out DWORD* pStatus);

    //HRESULT(STDMETHODCALLTYPE* GetAuthor)(
    // __RPC__in IWICComponentInfo * This,
    // /* [in] */ UINT cchAuthor,
    //    /* [size_is][unique][out][in] */ __RPC__inout_ecount_full_opt(cchAuthor) WCHAR* wzAuthor,
    //    /* [out] */ __RPC__out UINT* pcchActual);

    //HRESULT(STDMETHODCALLTYPE* GetVendorGUID)(
    // __RPC__in IWICComponentInfo * This,
    // /* [out] */ __RPC__out GUID* pguidVendor);

    //HRESULT(STDMETHODCALLTYPE* GetVersion)(
    // __RPC__in IWICComponentInfo * This,
    // /* [in] */ UINT cchVersion,
    //    /* [size_is][unique][out][in] */ __RPC__inout_ecount_full_opt(cchVersion) WCHAR* wzVersion,
    //    /* [out] */ __RPC__out UINT* pcchActual);

    //HRESULT(STDMETHODCALLTYPE* GetSpecVersion)(
    // __RPC__in IWICComponentInfo * This,
    // /* [in] */ UINT cchSpecVersion,
    //    /* [size_is][unique][out][in] */ __RPC__inout_ecount_full_opt(cchSpecVersion) WCHAR* wzSpecVersion,
    //    /* [out] */ __RPC__out UINT* pcchActual);

    //HRESULT(STDMETHODCALLTYPE* GetFriendlyName)(
    // __RPC__in IWICComponentInfo * This,
    // /* [in] */ UINT cchFriendlyName,
    //    /* [size_is][unique][out][in] */ __RPC__inout_ecount_full_opt(cchFriendlyName) WCHAR* wzFriendlyName,
    //    /* [out] */ __RPC__out UINT* pcchActual);

}
