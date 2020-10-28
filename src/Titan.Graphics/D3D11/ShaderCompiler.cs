using Titan.Core;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11Common;
using static Titan.Windows.Win32.Common;

namespace Titan.Graphics.D3D11
{
    internal unsafe class ShaderCompiler : IShaderCompiler
    {
        public CompiledShader CompileShaderFromFile(string filename, string entrypoint, string shaderVersion) => Compile(filename, entrypoint, shaderVersion, null);

        public CompiledShader CompileShaderFromFile(string filename, string entrypoint, string shaderVersion, ShaderDefines[] defines)
        {
            var pDefines = stackalloc D3D_SHADER_MACRO[defines.Length + 1];
            for (var i = 0; i < defines.Length; ++i)
            {
                fixed (byte* name = defines[i].Name.AsBytes())
                fixed (byte* value = defines[i].Value.AsBytes())
                {
                    pDefines[i] = new D3D_SHADER_MACRO
                    {
                        Definition = (sbyte*) value,
                        Name = (sbyte*) name
                    };
                }
            }
            return Compile(filename, entrypoint, shaderVersion, pDefines);
        }

        private static CompiledShader Compile(string filename, string entrypoint, string shaderVersion, D3D_SHADER_MACRO* pDefines)
        {
            using ComPtr<ID3DBlob> error = default;
            using ComPtr<ID3DBlob> shader = default;

            fixed (byte* pEntrypoint = entrypoint.AsBytes())
            fixed (byte* pTarget = shaderVersion.AsBytes())
            fixed (char* pFilename = filename)
            {
                // TODO add error handling
                CheckAndThrow(D3DCompileFromFile(pFilename, pDefines, null, (sbyte*)pEntrypoint, (sbyte*)pTarget, 0, 0, shader.GetAddressOf(), error.GetAddressOf()), "D3DCompileFromFile");
            }

            return new CompiledShader(shader);
        }
    }
}
