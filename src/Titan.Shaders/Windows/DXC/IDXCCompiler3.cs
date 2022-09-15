using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32;

namespace Titan.Shaders.Windows.DXC;


[Guid("228B4687-5A6A-4730-900C-9702B2203F54")]
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
        DxcBuffer* pSource, // Source text to compile
        char** pArguments, // Array of pointers to arguments
        uint argCount, // Number of arguments
        IDxcIncludeHandler* pIncludeHandler, // user-provided interface to handle #include directives (optional)
        in Guid riid,
        void** ppResult // IDxcResult: status, buffer, and errors
    )
        => ((delegate* unmanaged[Stdcall]<void*, DxcBuffer*, char**, uint, IDxcIncludeHandler*, in Guid, void**, HRESULT>)_vtbl[3])(Unsafe.AsPointer(ref this), pSource, pArguments, argCount, pIncludeHandler, riid, ppResult);

    //// Disassemble a program.
    //virtual HRESULT STDMETHODCALLTYPE Disassemble(
    //    _In_ const DxcBuffer* pObject,                // Program to disassemble: dxil container or bitcode.
    //    _In_ REFIID riid, _Out_ LPVOID* ppResult      // IDxcResult: status, disassembly text, and errors
    //) = 0;
}

[Guid("A005A9D9-B8BB-4594-B5C9-0E633BEC4D37")]

internal unsafe struct IDXCCompiler2
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

   
}
