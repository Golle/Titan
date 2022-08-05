using System;
using Titan.Shaders.Windows;

namespace Titan.Shaders;

public static class ShaderCompiler
{
    private static readonly IShaderCompiler[] _compilers =
    {
        new FxcCompiler(),
        new DxcCompiler()
    };

    public static ShaderCompilationResult Compile(string filePath, string entryPoint, ShaderModels shaderModel)
    {
        foreach (var shaderCompiler in _compilers)
        {
            if (shaderCompiler.IsSupported(shaderModel))
            {
                return shaderCompiler.CompileShader(filePath, entryPoint, shaderModel);
            }
        }
        throw new NotSupportedException($"Shader model {shaderModel} is not yet supported.");
    }
}
