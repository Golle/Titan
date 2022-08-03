using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Windows;

namespace Titan.Shaders.Windows.DXC;


[Guid("8BA5FB08-5195-40e2-AC58-0D989C3A0102")]
internal unsafe struct IDxcBlob
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(in Guid riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, in Guid, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* GetBufferPointer()
        => ((delegate* unmanaged[Stdcall]<void*, void*>)_vtbl[3])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nuint GetBufferSize()
        => ((delegate* unmanaged[Stdcall]<void*, nuint>)_vtbl[4])(Unsafe.AsPointer(ref this));

}

internal unsafe struct DxcDefine
{
    private void** _vtbl;
}

public unsafe struct IDxcIncludeHandler
{
    private void** _vtbl;
}

internal unsafe struct IDxcOperationResult
{
    private void** _vtbl;
}

[Guid("A005A9D9-B8BB-4594-B5C9-0E633BEC4D37")]
internal unsafe struct IDXCCompiler3
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(in Guid riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, in Guid, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Compile(
        IDxcBlob* pSource, // Source text to compile
        char* pSourceName, // Optional file name for pSource. Used in errors and include handlers.
        char* pEntryPoint, // entry point name
        char* pTargetProfile, // shader profile to compile
        char** pArguments, // Array of pointers to arguments
        uint argCount, // Number of arguments
        DxcDefine* pDefines, // Array of defines
        uint defineCount, // Number of defines
        IDxcIncludeHandler* pIncludeHandler, // user-provided interface to handle #include directives (optional)
        IDxcOperationResult** ppResult // Compiler output status, buffer, and errors
    )
        => ((delegate* unmanaged[Stdcall]<void*, IDxcBlob*, char*, char*, char*, char**, uint, DxcDefine*, uint, IDxcIncludeHandler*, IDxcOperationResult**, HRESULT>)_vtbl[3])(Unsafe.AsPointer(ref this), pSource, pSourceName, pEntryPoint, pTargetProfile, pArguments, argCount, pDefines, defineCount, pIncludeHandler, ppResult);

    //  // Preprocess source text
    //  virtual HRESULT STDMETHODCALLTYPE Preprocess(
    //    _In_ IDxcBlob *pSource,                       // Source text to preprocess
    //    _In_opt_z_ LPCWSTR pSourceName,               // Optional file name for pSource. Used in errors and include handlers.
    //    _In_opt_count_(argCount) LPCWSTR *pArguments, // Array of pointers to arguments
    //    _In_ UINT32 argCount,                         // Number of arguments
    //    _In_count_(defineCount)
    //    const DxcDefine* pDefines,                  // Array of defines
    //  _In_ UINT32 defineCount,                      // Number of defines
    //  _In_opt_ IDxcIncludeHandler* pIncludeHandler, // user-provided interface to handle #include directives (optional)
    //  _COM_Outptr_ IDxcOperationResult** ppResult   // Preprocessor output status, buffer, and errors
    //) = 0;

    //  // Disassemble a program.
    //  virtual HRESULT STDMETHODCALLTYPE Disassemble(
    //    _In_ IDxcBlob *pSource,                         // Program to disassemble.
    //    _COM_Outptr_ IDxcBlobEncoding **ppDisassembly   // Disassembly text.

    //    ) = 0;

    //virtual HRESULT STDMETHODCALLTYPE CompileWithDebug(
    //    _In_ IDxcBlob *pSource,                       // Source text to compile
    //_In_opt_z_ LPCWSTR pSourceName,               // Optional file name for pSource. Used in errors and include handlers.
    //_In_opt_z_ LPCWSTR pEntryPoint,               // Entry point name
    //_In_z_ LPCWSTR pTargetProfile,                // Shader profile to compile
    //_In_opt_count_(argCount) LPCWSTR *pArguments, // Array of pointers to arguments
    //    _In_ UINT32 argCount,                         // Number of arguments
    //    _In_count_(defineCount)
    //const DxcDefine* pDefines,                  // Array of defines
    //    _In_ UINT32 defineCount,                      // Number of defines
    //    _In_opt_ IDxcIncludeHandler* pIncludeHandler, // user-provided interface to handle #include directives (optional)
    //    _COM_Outptr_ IDxcOperationResult** ppResult,  // Compiler output status, buffer, and errors
    //    _Outptr_opt_result_z_ LPWSTR* ppDebugBlobName,// Suggested file name for debug blob. (Must be CoTaskMemFree()'d!)
    //    _COM_Outptr_opt_ IDxcBlob** ppDebugBlob       // Debug blob
    //) = 0;

    //virtual HRESULT STDMETHODCALLTYPE Compile(
    //    _In_ const DxcBuffer* pSource,                // Source text to compile
    //    _In_opt_count_(argCount) LPCWSTR* pArguments, // Array of pointers to arguments
    //_In_ UINT32 argCount,                         // Number of arguments
    //_In_opt_ IDxcIncludeHandler* pIncludeHandler, // user-provided interface to handle #include directives (optional)
    //_In_ REFIID riid, _Out_ LPVOID* ppResult      // IDxcResult: status, buffer, and errors
    //) = 0;

    //// Disassemble a program.
    //virtual HRESULT STDMETHODCALLTYPE Disassemble(
    //    _In_ const DxcBuffer* pObject,                // Program to disassemble: dxil container or bitcode.
    //    _In_ REFIID riid, _Out_ LPVOID* ppResult      // IDxcResult: status, disassembly text, and errors
    //) = 0;
}


internal unsafe struct IDxcLibrary
{

    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(in Guid riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, in Guid, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

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

[Guid("4605C4CB-2019-492A-ADA4-65F20BB7D67F")]
public unsafe struct IDxcUtils
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(in Guid riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, in Guid, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

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
        IDxcBlobEncoding** pBlobEncoding)
        => ((delegate* unmanaged[Stdcall]<void*, char*, uint*, IDxcBlobEncoding**, HRESULT>)_vtbl[7])(Unsafe.AsPointer(ref this), pFileName, pCodePage, pBlobEncoding);

    //virtual HRESULT STDMETHODCALLTYPE CreateReadOnlyStreamFromBlob(
    //  _In_ IDxcBlob *pBlob, _COM_Outptr_ IStream **ppStream) = 0;

    // Create default file-based include handler
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateDefaultIncludeHandler(IDxcIncludeHandler** ppResult)
        => ((delegate* unmanaged[Stdcall]<void*, IDxcIncludeHandler**, HRESULT>)_vtbl[9])(Unsafe.AsPointer(ref this), ppResult);

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


[Guid("7241d424-2646-4191-97c0-98e96e42fc68")]
public unsafe struct IDxcBlobEncoding
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(in Guid riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, in Guid, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void* GetBufferPointer()
        => ((delegate* unmanaged[Stdcall]<void*, void*>)_vtbl[3])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public nuint GetBufferSize()
        => ((delegate* unmanaged[Stdcall]<void*, nuint>)_vtbl[4])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetEncoding(int* pKnown, uint* pCodePage)
        => ((delegate* unmanaged[Stdcall]<void*, int*, uint*, HRESULT>)_vtbl[4])(Unsafe.AsPointer(ref this), pKnown, pCodePage);
}
