namespace Titan.Graphics.D3D11
{
    public interface IShaderCompiler
    {
        CompiledShader CompileShaderFromFile(string filename, string entrypoint, string shaderVersion);
        CompiledShader CompileShaderFromFile(string filename, string entrypoint, string shaderVersion, ShaderDefines[] defines);
    }
}
