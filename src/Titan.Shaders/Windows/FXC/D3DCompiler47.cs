using System.Runtime.InteropServices;
using Titan.Windows;
using Titan.Windows.D3D;

namespace Titan.Shaders.Windows.FXC;

internal static unsafe class D3DCompiler47
{
    //NOTE(Jens): The Dll lookup should happen in some other module, so we can set a temp folder for dlls. don't what them checked in on github.
    private const string DllName = "d3dcompiler_47";

    public static readonly ID3DInclude* D3D_COMPILE_STANDARD_FILE_INCLUDE = (ID3DInclude*)1;

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3DCompile(
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

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3DCompile2(
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

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3DCompileFromFile(
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
