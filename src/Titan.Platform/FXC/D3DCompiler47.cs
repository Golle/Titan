using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D;

namespace Titan.Platform.FXC;

public static unsafe partial class D3DCompiler47
{
    //NOTE(Jens): The Dll lookup should happen in some other module, so we can set a temp folder for dlls. don't what them checked in on github.
    private const string DllName = "d3dcompiler_47";

    public static readonly ID3DInclude* D3D_COMPILE_STANDARD_FILE_INCLUDE = (ID3DInclude*)1;

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HRESULT D3DCompile(
        void* pSrcData,
        nuint SrcDataSize,
        byte* pSourceName,
        D3D_SHADER_MACRO* pDefines,
        ID3DInclude* pInclude,
        byte* pEntrypoint,
        byte* pTarget,
        uint Flags1,
        uint Flags2,
        ID3DBlob** ppCode,
        ID3DBlob** ppErrorMsgs);

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HRESULT D3DCompile2(
        void* pSrcData,
        nuint SrcDataSize,
        byte* pSourceName,
        D3D_SHADER_MACRO* pDefines,
        ID3DInclude* pInclude,
        byte* pEntrypoint,
        byte* pTarget,
        uint Flags1,
        uint Flags2,
        uint SecondaryDataFlags,
        void* pSecondaryData,
        nuint SecondaryDataSize,
        ID3DBlob** ppCode,
        ID3DBlob** ppErrorMsgs);

    [LibraryImport(DllName, SetLastError = true)]
    [UnmanagedCallConv(CallConvs = new[] { typeof(CallConvStdcall) })]
    public static partial HRESULT D3DCompileFromFile(
        char* pFileName,
        D3D_SHADER_MACRO* pDefines,
        ID3DInclude* pInclude,
        byte* pEntrypoint,
        byte* pTarget,
        uint Flags1,
        uint Flags2,
        ID3DBlob** ppCode,
        ID3DBlob** ppErrorMsgs);
}
