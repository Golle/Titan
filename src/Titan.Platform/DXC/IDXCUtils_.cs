using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;

namespace Titan.Platform.DXC;

[Guid("4605C4CB-2019-492A-ADA4-65F20BB7D67F")]
public unsafe struct IDXCUtils : INativeGuid
{
    public static Guid* Guid => IID.IID_IDXCUtils;
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

    // Create a sub-blob that holds a reference to the outer blob and points to its memory.
    //virtual HRESULT STDMETHODCALLTYPE CreateBlobFromBlob(
    //  _In_ IDxcBlob *pBlob, UINT32 offset, UINT32 length, _COM_Outptr_ IDxcBlob **ppResult) = 0;

    //// For codePage, use 0 (or DXC_CP_ACP) for raw binary or ANSI code page

    //// Creates a blob referencing existing memory, with no copy.
    //// User must manage the memory lifetime separately.
    //// (was: CreateBlobWithEncodingFromPinned)
    //virtual HRESULT STDMETHODCALLTYPE CreateBlobFromPinned(
    //  _In_bytecount_(size) LPCVOID pData, UINT32 size, UINT32 codePage,
    //  _COM_Outptr_ IDxcBlobEncoding **pBlobEncoding) = 0;

    //// Create blob, taking ownership of memory allocated with supplied allocator.
    //// (was: CreateBlobWithEncodingOnMalloc)
    //virtual HRESULT STDMETHODCALLTYPE MoveToBlob(
    //  _In_bytecount_(size) LPCVOID pData, IMalloc* pIMalloc, UINT32 size, UINT32 codePage,
    //  _COM_Outptr_ IDxcBlobEncoding **pBlobEncoding) = 0;

    //////
    //// New blobs and copied contents are allocated with the current allocator

    //// Copy blob contents to memory owned by the new blob.
    //// (was: CreateBlobWithEncodingOnHeapCopy)
    //virtual HRESULT STDMETHODCALLTYPE CreateBlob(
    //  _In_bytecount_(size) LPCVOID pData, UINT32 size, UINT32 codePage,
    //  _COM_Outptr_ IDxcBlobEncoding **pBlobEncoding) = 0;

    //// (was: CreateBlobFromFile)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT LoadFile(
        char* pFileName,
        uint* pCodePage,
        IDXCBlobEncoding** pBlobEncoding)
        => ((delegate* unmanaged[Stdcall]<void*, char*, uint*, IDXCBlobEncoding**, HRESULT>)_vtbl[7])(Unsafe.AsPointer(ref this), pFileName, pCodePage, pBlobEncoding);

    //virtual HRESULT STDMETHODCALLTYPE CreateReadOnlyStreamFromBlob(
    //  _In_ IDxcBlob *pBlob, _COM_Outptr_ IStream **ppStream) = 0;

    // Create default file-based include handler
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateDefaultIncludeHandler(IDXCIncludeHandler** ppResult)
        => ((delegate* unmanaged[Stdcall]<void*, IDXCIncludeHandler**, HRESULT>)_vtbl[9])(Unsafe.AsPointer(ref this), ppResult);

    //// Convert or return matching encoded text blobs
    //virtual HRESULT STDMETHODCALLTYPE GetBlobAsUtf8(
    //  _In_ IDxcBlob *pBlob, _COM_Outptr_ IDxcBlobUtf8 **pBlobEncoding) = 0;
    //virtual HRESULT STDMETHODCALLTYPE GetBlobAsUtf16(
    //  _In_ IDxcBlob *pBlob, _COM_Outptr_ IDxcBlobUtf16 **pBlobEncoding) = 0;

    //virtual HRESULT STDMETHODCALLTYPE GetDxilContainerPart(
    //  _In_ const DxcBuffer* pShader,
    //  _In_ UINT32 DxcPart,
    //  _Outptr_result_nullonfailure_ void** ppPartData,
    //  _Out_ UINT32 *pPartSizeInBytes) = 0;

    //// Create reflection interface from serialized Dxil container, or DXC_PART_REFLECTION_DATA.
    //// TBD: Require part header for RDAT?  (leaning towards yes)
    //virtual HRESULT STDMETHODCALLTYPE CreateReflection(
    //  _In_ const DxcBuffer* pData, REFIID iid, void** ppvReflection) = 0;

    //virtual HRESULT STDMETHODCALLTYPE BuildArguments(
    //  _In_opt_z_ LPCWSTR pSourceName,               // Optional file name for pSource. Used in errors and include handlers.
    //  _In_opt_z_ LPCWSTR pEntryPoint,               // Entry point name. (-E)
    //  _In_z_ LPCWSTR pTargetProfile,                // Shader profile to compile. (-T)
    //  _In_opt_count_(argCount) LPCWSTR *pArguments, // Array of pointers to arguments
    //  _In_ UINT32 argCount,                         // Number of arguments
    //  _In_count_(defineCount)
    //    const DxcDefine* pDefines,                  // Array of defines
    //  _In_ UINT32 defineCount,                      // Number of defines
    //  _COM_Outptr_ IDxcCompilerArgs **ppArgs        // Arguments you can use with Compile() method
    //) = 0;

    //// Takes the shader PDB and returns the hash and the container inside it
    //virtual HRESULT STDMETHODCALLTYPE GetPDBContents(
    //  _In_ IDxcBlob *pPDBBlob, _COM_Outptr_ IDxcBlob **ppHash, _COM_Outptr_ IDxcBlob **ppContainer) = 0;

    //DECLARE_CROSS_PLATFORM_UUIDOF(IDxcUtils)
    
};
