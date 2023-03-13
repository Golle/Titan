using System.Runtime.CompilerServices;
using Titan.Platform.Win32;

namespace Titan.Platform.DXC;

internal unsafe struct IDXCLibrary
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
    //virtual HRESULT STDMETHODCALLTYPE SetMalloc(_In_opt_ IMalloc *pMalloc) = 0;
    //virtual HRESULT STDMETHODCALLTYPE CreateBlobFromBlob(
    //    _In_ IDxcBlob *pBlob, UINT32 offset, UINT32 length, _COM_Outptr_ IDxcBlob **ppResult) = 0;
    //public HRESULT CreateBlobFromFile(
    //    char* pFileName, UINT32* codePage,
    //_COM_Outptr_ IDxcBlobEncoding **pBlobEncoding) = 0;
    //virtual HRESULT STDMETHODCALLTYPE CreateBlobWithEncodingFromPinned(
    //    _In_bytecount_(size) LPCVOID pText, UINT32 size, UINT32 codePage,
    //_COM_Outptr_ IDxcBlobEncoding **pBlobEncoding) = 0;
    //virtual HRESULT STDMETHODCALLTYPE CreateBlobWithEncodingOnHeapCopy(
    //    _In_bytecount_(size) LPCVOID pText, UINT32 size, UINT32 codePage,
    //_COM_Outptr_ IDxcBlobEncoding **pBlobEncoding) = 0;
    //virtual HRESULT STDMETHODCALLTYPE CreateBlobWithEncodingOnMalloc(
    //    _In_bytecount_(size) LPCVOID pText, IMalloc* pIMalloc, UINT32 size, UINT32 codePage,
    //    _COM_Outptr_ IDxcBlobEncoding **pBlobEncoding) = 0;
    //virtual HRESULT STDMETHODCALLTYPE CreateIncludeHandler(
    //    _COM_Outptr_ IDxcIncludeHandler **ppResult) = 0;
    //virtual HRESULT STDMETHODCALLTYPE CreateStreamFromBlobReadOnly(
    //    _In_ IDxcBlob *pBlob, _COM_Outptr_ IStream **ppStream) = 0;
    //virtual HRESULT STDMETHODCALLTYPE GetBlobAsUtf8(
    //    _In_ IDxcBlob *pBlob, _COM_Outptr_ IDxcBlobEncoding **pBlobEncoding) = 0;

    //// Renamed from GetBlobAsUtf16 to GetBlobAsWide
    //virtual HRESULT STDMETHODCALLTYPE GetBlobAsWide(
    //    _In_ IDxcBlob *pBlob, _COM_Outptr_ IDxcBlobEncoding **pBlobEncoding) = 0;

    //#ifdef _WIN32
    //// Alias to GetBlobAsWide on Win32
    //inline HRESULT GetBlobAsUtf16(
    //    _In_ IDxcBlob *pBlob, _COM_Outptr_ IDxcBlobEncoding **pBlobEncoding)
    //{
    //    return this->GetBlobAsWide(pBlob, pBlobEncoding);
    //}
    //#endif
};
