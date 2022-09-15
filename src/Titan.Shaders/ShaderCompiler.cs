using System.Runtime.InteropServices;
using Titan.Platform.Win32.Win32;
using Titan.Shaders.Windows;

namespace Titan.Shaders;

/// <summary>
/// The ShaderCompiler requires dxccompiler.dll, dxil.dll and d3dcompiler_47.dll. The latter one is probably in the dll search path on a dev machine, but the other ones must be downloaded.
/// Call the SetShaderCompilerDllFolder to set the search path for the dlls
/// </summary>
public static class ShaderCompiler
{
    private static readonly IShaderCompiler[] _compilers =
    {
        new FxcCompiler(),
        new DxcCompiler()
    };

    public static void SetShaderCompilerDllFolder(string path)
    {
        unsafe
        {
            //NOTE(Jens): I think we need the dxil.dll that is loaded by dxcompiler. Not sure the NativeLibrary.SetDllImportResolver will be triggered by that load.
            fixed (char* pPath = path)
            {
                Kernel32.SetDllDirectoryW(pPath);
            }
        }
        
        NativeLibrary.SetDllImportResolver(typeof(ShaderCompiler).Assembly, (name, assembly, _) =>
        {
            if (name.Equals("dxcompiler"))
            {
                return NativeLibrary.Load(Path.Combine(path, "dxcompiler.dll"), assembly, null);
            }
            return IntPtr.Zero;
        });
    }

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
