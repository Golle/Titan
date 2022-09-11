using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D11;

namespace Titan.Platform.Win32.D3D;

public static unsafe class D3DCompiler
{
    private const string DllName = "d3dcompiler_47";

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3DCompile(
        void* pSrcData,
        nuint srcDataSize,
        sbyte* pSourceName,
        D3D_SHADER_MACRO* pDefines,
        ID3DInclude* pInclude,
        sbyte* pEntrypoint,
        sbyte* pTarget,
        D3D_COMPILE_FLAGS flags1,
        D3D_COMPILE_FLAGS flags2,
        ID3DBlob** ppCode,
        ID3DBlob** ppErrorMsgs
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3DCompileFromFile(
        char* pFileName,
        D3D_SHADER_MACRO* pDefines,
        ID3DInclude* pInclude,
        sbyte* pEntrypoint,
        sbyte* pTarget,
        D3D_COMPILE_FLAGS flags1,
        D3D_COMPILE_FLAGS flags2,
        ID3DBlob** ppCode,
        ID3DBlob** ppErrorMsgs
    );

    [DllImport(DllName, CallingConvention = CallingConvention.StdCall, SetLastError = true, EntryPoint = "D3DCompileFromFile")]
    public static extern HRESULT D3DCompileFromFileNew(
        char* pFileName,
        D3D_SHADER_MACRO* pDefines,
        ID3DInclude* pInclude,
        byte* pEntrypoint,
        byte* pTarget,
        D3D_COMPILE_FLAGS flags1,
        D3D_COMPILE_FLAGS flags2,
        ID3DBlob** ppCode,
        ID3DBlob** ppErrorMsgs
    );
}
