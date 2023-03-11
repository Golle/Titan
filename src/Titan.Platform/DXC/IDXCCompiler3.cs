using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32;

namespace Titan.Platform.DXC;


[Guid("228B4687-5A6A-4730-900C-9702B2203F54")]
public unsafe struct IDXCCompiler3
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
        DXCBuffer* pSource, // Source text to compile
        char** pArguments, // Array of pointers to arguments
        uint argCount, // Number of arguments
        IDXCIncludeHandler* pIncludeHandler, // user-provided interface to handle #include directives (optional)
        in Guid riid,
        void** ppResult // IDxcResult: status, buffer, and errors
    )
        => ((delegate* unmanaged[Stdcall]<void*, DXCBuffer*, char**, uint, IDXCIncludeHandler*, in Guid, void**, HRESULT>)_vtbl[3])(Unsafe.AsPointer(ref this), pSource, pArguments, argCount, pIncludeHandler, riid, ppResult);

    //// Disassemble a program.
    //virtual HRESULT STDMETHODCALLTYPE Disassemble(
    //    _In_ const DxcBuffer* pObject,                // Program to disassemble: dxil container or bitcode.
    //    _In_ REFIID riid, _Out_ LPVOID* ppResult      // IDxcResult: status, disassembly text, and errors
    //) = 0;
}
