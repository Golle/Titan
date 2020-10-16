using Titan.Core;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11Common;
using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11
{
    internal unsafe class ShaderCompiler : IShaderCompiler
    {
        public CompiledShader CompileShaderFromFile(string filename, string entrypoint, string shaderVersion)
        {
            using ComPtr<ID3DBlob> error = default;
            using ComPtr<ID3DBlob> shader = default;

            fixed(byte* pEntrypoint = entrypoint.AsBytes())
            fixed(byte* pTarget = shaderVersion.AsBytes())
            fixed (char* pFilename = filename)
            {
                // TODO add error handling
                CheckAndThrow(D3DCompileFromFile(pFilename, null, null, (sbyte*) pEntrypoint, (sbyte*) pTarget, 0, 0, shader.GetAddressOf(), error.GetAddressOf()), "D3DCompileFromFile");
            }

            return new CompiledShader(shader);
        }

    }
}
