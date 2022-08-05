namespace Titan.Shaders.Windows;

internal interface IShaderCompiler
{
    bool IsSupported(ShaderModels model);
    ShaderCompilationResult CompileShader(string filePath, string entryPoint, ShaderModels shaderModel);
}
