using System;
using System.Runtime.CompilerServices;
using System.Text;
using Titan.Shaders.Windows;
using Titan.Shaders.Windows.DXC;
using Titan.Shaders.Windows.FXC;
using Titan.Windows;
using Titan.Windows.D3D;

namespace Titan.Shaders;


public static class ShaderCompiler1
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



public static unsafe class ShaderCompiler
{

    public static void CompileShader(string filePath, string entryPoint, ShaderModels shaderModel)
    {
        //NOTE(Jens): Could use stackalloc if we're going to use this for hot reloading in the engine to avoid GC allocations
        var entryPointUtf8 = Encoding.UTF8.GetBytes(entryPoint);
        var target = Encoding.UTF8.GetBytes(shaderModel.ToString().ToLower());

        if ((int)shaderModel < 100)
        {
            // Old compiler
            using ComPtr<ID3DBlob> error = default;
            using ComPtr<ID3DBlob> data = default;
            fixed (char* pFilePath = filePath)
            fixed (byte* pEntryPoint = entryPointUtf8)
            fixed (byte* pTarget = target)
            {
                var hr = D3DCompiler47.D3DCompileFromFile(
                    pFilePath,
                    null,
                    D3DCompiler47.D3D_COMPILE_STANDARD_FILE_INCLUDE,
                    pEntryPoint,
                    pTarget,
                    0, // Flags1
                    0, // Flags2
                    data.GetAddressOf(),
                    error.GetAddressOf()
                );

                if (Common.FAILED(hr))
                {
                    Console.WriteLine($"Failed to compile shader with HRESULT {hr}");
                    var err = new ReadOnlySpan<byte>(error.Get()->GetBufferPointer(), (int)error.Get()->GetBufferSize());
                    Console.WriteLine("Error: {0}", Encoding.UTF8.GetString(err));
                }
                else
                {
                    Console.WriteLine($"Successfully compiled shader with ShaderModel {shaderModel} (FXC)");
                }
            }
        }
        else
        {


            using ComPtr<IDXCCompiler3> compiler = default;
            using ComPtr<IDxcUtils> utils = default;

            var hr = DxcCompilerCommon.DxcCreateInstance(DxcCompilerCommon.CLSID_Compiler, typeof(IDXCCompiler3).GUID, (void**)compiler.GetAddressOf());
            if (Common.FAILED(hr))
            {
                Console.WriteLine($"Failed to create {nameof(IDXCCompiler3)} instance with HRESULT {hr}");
            }
            hr = DxcCompilerCommon.DxcCreateInstance(DxcCompilerCommon.CLSID_DxcUtils, typeof(IDxcUtils).GUID, (void**)utils.GetAddressOf());
            if (Common.FAILED(hr))
            {
                Console.WriteLine($"Failed to create {nameof(IDxcUtils)} instance with HRESULT {hr}");
            }

            using ComPtr<IDxcIncludeHandler> includeHandler = default;
            hr = utils.Get()->CreateDefaultIncludeHandler(includeHandler.GetAddressOf());
            if (Common.FAILED(hr))
            {
                Console.WriteLine($"Failed to CreateDefaultIncludeHandler {nameof(IDxcIncludeHandler)} instance with HRESULT {hr}");
            }



            using ComPtr<IDxcBlobEncoding> pSource = default;
            fixed (char* pFilePath = filePath)
            {
                hr = utils.Get()->LoadFile(pFilePath, null, pSource.GetAddressOf());
                if (Common.FAILED(hr))
                {
                    Console.WriteLine($"Failed to LoadFile with HRESULT {hr}");
                }

            }
            var buffer = new DxcBuffer
            {
                Ptr = pSource.Get()->GetBufferPointer(),
                Size = pSource.Get()->GetBufferSize(),
                Encoding = 0
            };


            var arguments = new TestArgs(stackalloc char[2048]);
            arguments.AddArgument("-E");
            arguments.AddArgument(entryPoint);
            arguments.AddArgument("-T");
            arguments.AddArgument(shaderModel.ToString().ToLower());
            arguments.AddArgument("-Qstrip_reflect");

            //LPCWSTR pszArgs[] =
            //{
            //    L"myshader.hlsl",            // Optional shader source file name for error reporting and for PIX shader source view.  
            //    L"-E", L"main",              // Entry point.
            //    L"-T", L"ps_6_0",            // Target.
            //    L"-Zs",                      // Enable debug information (slim format)
            //    L"-D", L"MYDEFINE=1",        // A single define.
            //    L"-Fo", L"myshader.bin",     // Optional. Stored in the pdb. 
            //    L"-Fd", L"myshader.pdb",     // The file name of the pdb. This must either be supplied or the autogenerated file name must be used.
            //    L"-Qstrip_reflect",          // Strip reflection into a separate blob. 
            //};


            {

                ComPtr<IDxcResult> result = default;
                hr = compiler.Get()->Compile(&buffer, (char**)arguments.Arguments, arguments._count, includeHandler.Get(), typeof(IDxcResult).GUID, (void**)result.GetAddressOf());
                if (Common.FAILED(hr))
                {
                    Console.WriteLine($"Failed to compile shader with HRESULT {hr}");
                }

                ComPtr<IDxcBlobUtf8> pErrors = default;
                result.Get()->GetOutput(DXC_OUT_KIND.DXC_OUT_ERRORS, typeof(IDxcBlobUtf8).GUID, (void**)pErrors.GetAddressOf(), null);
                // Note that d3dcompiler would return null if no errors or warnings are present.  
                // IDxcCompiler3::Compile will always return an error buffer, but its length will be zero if there are no warnings or errors.
                if (pErrors.Get() != null && pErrors.Get()->GetStringLength() != 0)
                {
                    var error = new ReadOnlySpan<byte>(pErrors.Get()->GetStringPointer(), (int)pErrors.Get()->GetStringLength());

                    Console.Error.WriteLine($"Warnings and Errors:\n{Encoding.UTF8.GetString(error)}\n");
                }
            }

            //using ComPtr<IDxcOperationResult> result = default;
            //fixed (char* pEntryPoint = entryPoint)
            //fixed (char* pTargetProfile = shaderModel.ToString().ToLower())
            //{
            //    hr = compiler.Get()->Compile((IDxcBlob*)pSource.Get(), null, pEntryPoint, pTargetProfile, null, 0, null, 0, null, result);
            //    if (Common.FAILED(hr))
            //    {
            //        Console.WriteLine($"Failed to compile shader with with HRESULT {hr}");
            //    }
            //    else
            //    {
            //        Console.WriteLine($"Successfully compiled shader with ShaderModel {shaderModel} (DXC)");
            //    }
            //}




        }

    }

    public static void CompileShader(ReadOnlySpan<byte> source, string entryPoint, ShaderModels shaderModel)
        => throw new NotImplementedException();
}


public unsafe ref struct TestArgs
{
    public fixed ulong Arguments[100];
    public Span<char> _buffer;
    public int _index;
    public uint _count;
    public TestArgs(Span<char> buffer)
    {
        _buffer = buffer;
    }

    public void AddArgument(string arg)
    {
        var nextIndex = _index + arg.Length + 1;
        arg.CopyTo(_buffer[_index..]);
        Arguments[_count++] = (ulong)Unsafe.AsPointer(ref _buffer[_index]);
        _index = nextIndex;
    }
}

public unsafe ref struct TestArg
{
    private Span<char> _arg;
    public TestArg(Span<char> arg)
    {
        _arg = arg;
    }
    public char* AsPointer() => (char*)Unsafe.AsPointer(ref _arg.GetPinnableReference());
}

public unsafe struct DxcBuffer
{
    public void* Ptr;
    public nuint Size;
    public uint Encoding;
}
