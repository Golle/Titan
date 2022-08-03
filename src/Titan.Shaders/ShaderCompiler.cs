using System;
using System.Text;
using Titan.Shaders.Windows;
using Titan.Shaders.Windows.DXC;
using Titan.Shaders.Windows.FXC;
using Titan.Windows;
using Titan.Windows.D3D;

namespace Titan.Shaders;



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

            var hr = DxcCompiler.DxcCreateInstance(DxcCompiler.CLSID, typeof(IDXCCompiler3).GUID, (void**)compiler.GetAddressOf());
            if (Common.FAILED(hr))
            {
                Console.WriteLine($"Failed to create {nameof(IDXCCompiler3)} instance with HRESULT {hr}");
            }
            hr = DxcCompiler.DxcCreateInstance(DxcCompiler.CLSID_DxcLibrary, typeof(IDxcUtils).GUID, (void**)utils.GetAddressOf());
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


            using ComPtr<IDxcOperationResult> result = default;
            fixed (char* pEntryPoint = entryPoint)
            fixed (char* pTargetProfile = shaderModel.ToString().ToLower())
            {
                hr = compiler.Get()->Compile((IDxcBlob*)pSource.Get(), null, pEntryPoint, pTargetProfile, null, 0, null, 0, null, result);
                if (Common.FAILED(hr))
                {
                    Console.WriteLine($"Failed to compile shader with with HRESULT {hr}");
                }
                else
                {
                    Console.WriteLine($"Successfully compiled shader with ShaderModel {shaderModel} (DXC)");
                }
            }




        }

    }

    public static void CompileShader(ReadOnlySpan<byte> source, string entryPoint, ShaderModels shaderModel)
        => throw new NotImplementedException();
}


public unsafe struct DxcBuffer
{
    public void* Ptr;
    public nuint Size;
    public uint Encoding;
}
