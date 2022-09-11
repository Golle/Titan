using System.Text;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Platform.Win32.D3D;
using static Titan.Platform.Win32.Common;
using static Titan.Platform.Win32.D3D.D3DCompiler;

namespace Titan.Graphics.D3D11.Shaders;

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
    internal ID3DBlob* Compile(in MemoryChunk<byte> source, string entrypoint, string shaderVersion)
    {
        static byte[] AsBytes(string str) => Encoding.ASCII.GetBytes(str);

        // TODO: can these allocatations be replaces with stackalloc? (if they are less than 1mb it should be fine)
        ID3DBlob* shader;
        fixed (byte* pEntrypoint = AsBytes(entrypoint))
        fixed (byte* pTarget = AsBytes(shaderVersion))
        {
            ID3DBlob* error = null;
            var result = D3DCompile(source.AsPointer(), source.Size, null, null, null, (sbyte*)pEntrypoint, (sbyte*)pTarget, 0, 0, &shader, &error);
            if (FAILED(result) && error != null)
            {
                var errorMessage = Encoding.ASCII.GetString((byte*)error->GetBufferPointer(), (int)error->GetBufferSize());
                Logger.Error<ShaderCompiler>(errorMessage);
            }

            if (error != null)
            {
                error->Release();
            }
            CheckAndThrow(result, nameof(D3DCompile));
        }
        return shader;
    }


    //internal CompiledShader CompileFromFile(string filename, string entrypoint, string shaderVersion)
    //{
    //    ID3DBlob* shader;
    //    fixed (byte* pEntrypoint = entrypoint.AsBytes())
    //    fixed (byte* pTarget = shaderVersion.AsBytes())
    //    fixed (char* pFilename = filename)
    //    {
    //        ID3DBlob* error = null;
    //        var result = D3DCompileFromFile(pFilename, null, null, (sbyte*) pEntrypoint, (sbyte*) pTarget, 0, 0, &shader, &error);
    //        if (FAILED(result) && error != null)
    //        {
    //            LOGGER.Debug(new string((char*) error->GetBufferPointer(), 0, (int) error->GetBufferSize()));
    //        }
    //        if( error != null) error->Release();
    //        CheckAndThrow(result, nameof(D3DCompileFromFile));
    //    }
    //    return new CompiledShader(shader);
    //}
}
