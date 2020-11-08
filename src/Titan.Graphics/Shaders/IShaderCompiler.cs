namespace Titan.Graphics.Shaders
{
    public interface IShaderCompiler
    {
        CompiledShader CompileShaderFromFile(string filename, string entrypoint, string shaderVersion, ShaderDefines[] defines = null);
    }
}
