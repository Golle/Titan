namespace Titan.Tools.Core.Shaders;

internal interface IShaderCompiler
{
    bool IsSupported(ShaderModels model);
    ShaderCompilationResult CompileShader(string filePath, string entryPoint, ShaderModels shaderModel);
}
