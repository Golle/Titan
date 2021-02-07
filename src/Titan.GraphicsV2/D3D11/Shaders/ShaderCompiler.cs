using System.Diagnostics.CodeAnalysis;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D11Common;

namespace Titan.GraphicsV2.D3D11.Shaders
{
    internal unsafe class ShaderCompiler
    {

        /* TODO: add support for #defines later
         *var pDefines = stackalloc D3D_SHADER_MACRO[defines.Length + 1];
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
         *
         */

        internal CompiledShader Compile(string source, string entrypoint, string shaderVersion)
        {
            ID3DBlob* shader;
            fixed (char* pSource = source)
            fixed (byte* pEntrypoint = entrypoint.AsBytes())
            fixed (byte* pTarget = shaderVersion.AsBytes())
            {
                ID3DBlob* error = null;
                var result = D3DCompile(pSource, (nuint) source.Length, null, null, null, (sbyte*) pEntrypoint, (sbyte*) pTarget, 0, 0, &shader, &error);
                if (FAILED(result) && error != null)
                {
                    LOGGER.Debug(new string((char*)error->GetBufferPointer(), 0, (int)error->GetBufferSize()));
                }

                if (error != null)
                {
                    error->Release();
                }
                CheckAndThrow(result, nameof(D3DCompileFromFile));
            }

            return new CompiledShader(shader);
        }


        internal CompiledShader CompileFromFile(string filename, string entrypoint, string shaderVersion)
        {
            ID3DBlob* shader;
            fixed (byte* pEntrypoint = entrypoint.AsBytes())
            fixed (byte* pTarget = shaderVersion.AsBytes())
            fixed (char* pFilename = filename)
            {
                ID3DBlob* error = null;
                var result = D3DCompileFromFile(pFilename, null, null, (sbyte*) pEntrypoint, (sbyte*) pTarget, 0, 0, &shader, &error);
                if (FAILED(result) && error != null)
                {
                    LOGGER.Debug(new string((char*) error->GetBufferPointer(), 0, (int) error->GetBufferSize()));
                }
                if( error != null) error->Release();
                CheckAndThrow(result, nameof(D3DCompileFromFile));
            }
            return new CompiledShader(shader);
        }
    }
}
