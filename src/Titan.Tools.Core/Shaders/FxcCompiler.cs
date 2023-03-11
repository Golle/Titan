using System.Text;
using Titan.Core.Logging;
using Titan.Platform.FXC;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D;
using static Titan.Platform.Win32.Win32Common;

namespace Titan.Tools.Core.Shaders;

internal unsafe class FxcCompiler : IShaderCompiler
{
    public bool IsSupported(ShaderModels model) =>
        model
            is <= ShaderModels.CS_5_1
            or <= ShaderModels.VS_5_1
            or <= ShaderModels.PS_5_1;

    public ShaderCompilationResult CompileShader(string filePath, string entryPoint, ShaderModels shaderModel)
    {
        //NOTE(Jens): Configure this when we need it. Right now we're keeping it simple
        D3D_SHADER_MACRO* defines = null;
        ID3DInclude* includes = null;
        var flags1 = 0u;
        var flags2 = 0u;

        var entryPointUtf8 = Encoding.UTF8.GetBytes(entryPoint);
        var targetUtf8 = Encoding.UTF8.GetBytes(shaderModel.ToString().ToLower());

        using ComPtr<ID3DBlob> error = default;
        using ComPtr<ID3DBlob> code = default;
        HRESULT hr;

        fixed (byte* pEntryPoint = entryPointUtf8)
        fixed (byte* pTarget = targetUtf8)
        fixed (char* pFilePath = filePath)
        {
            hr = D3DCompiler47.D3DCompileFromFile(pFilePath, defines, includes, pEntryPoint, pTarget, flags1, flags2, code.GetAddressOf(), error.GetAddressOf());
        }

        if (FAILED(hr))
        {
            Logger.Error<FxcCompiler>($"Failed §to compile shader with HRESULT {hr}");
            if (error.Get() != null && error.Get()->GetBufferPointer() != null && error.Get()->GetBufferSize() > 0)
            {
                var errorMessage = Encoding.UTF8.GetString((byte*)error.Get()->GetBufferPointer(), (int)error.Get()->GetBufferSize());
                Logger.Error<FxcCompiler>(errorMessage);
                return new FXCCompilerResult(errorMessage);
            }
            return new FXCCompilerResult($"Failed with HRESULT {hr} and no Error message.");
        }

        if (code.Get() == null || code.Get()->GetBufferSize() == 0 || code.Get()->GetBufferPointer() == null)
        {
            return new FXCCompilerResult("Compilation succeeded but code is null or size 0.");
        }
        Logger.Info<FxcCompiler>("Compilation successful!");
        return new FXCCompilerResult(code);
    }

    private class FXCCompilerResult : ShaderCompilationResult
    {
        private ComPtr<ID3DBlob> _byteCode;
        public FXCCompilerResult(string error)
            : base(error)
        {
        }
        public FXCCompilerResult(ComPtr<ID3DBlob> byteCode)
        {
            // Call new here to increase the internal ref  counter
            _byteCode = new ComPtr<ID3DBlob>(byteCode);
        }

        public override ReadOnlySpan<byte> GetByteCode()
        {
            var ptr = _byteCode.Get();
            if (ptr == null || ptr->GetBufferSize() == 0 || ptr->GetBufferPointer() == null)
            {
                return ReadOnlySpan<byte>.Empty;
            }
            return new ReadOnlySpan<byte>(ptr->GetBufferPointer(), (int)ptr->GetBufferSize());
        }

        public override void Dispose()
            => _byteCode.Dispose();
    }

}
