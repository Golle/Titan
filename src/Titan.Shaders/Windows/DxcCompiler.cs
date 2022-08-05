using System;
using System.IO;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Shaders.Windows.DXC;
using Titan.Windows;
using static System.Net.WebRequestMethods;
using static Titan.Shaders.Windows.DXC.DxcCompilerCommon;
using static Titan.Windows.Common;

namespace Titan.Shaders.Windows;

internal unsafe class DxcCompiler : IShaderCompiler
{
    public bool IsSupported(ShaderModels model) =>
        model
            is >= ShaderModels.CS_6_0
            or >= ShaderModels.VS_6_0
            or >= ShaderModels.PS_6_0;

    public ShaderCompilationResult CompileShader(string filePath, string entryPoint, ShaderModels shaderModel)
    {
        using ComPtr<IDxcUtils> utils = default;
        using ComPtr<IDXCCompiler3> compiler = default;
        using ComPtr<IDxcIncludeHandler> includeHandler = default;

        HRESULT hr;

        hr = DxcCreateInstance(CLSID_DxcUtils, typeof(IDxcUtils).GUID, (void**)utils.GetAddressOf());
        if (FAILED(hr))
        {
            var errorMessage = $"Failed to create and instance of {nameof(IDxcUtils)} with HRESULT {hr}";
            Logger.Error<DxcCompiler>(errorMessage);
            return new DxcCompilerResult(errorMessage);
        }

        hr = DxcCreateInstance(CLSID_Compiler, typeof(IDXCCompiler3).GUID, (void**)compiler.GetAddressOf());
        if (FAILED(hr))
        {
            var errorMessage = $"Failed to create and instance of {nameof(IDXCCompiler3)} with HRESULT {hr}";
            Logger.Error<DxcCompiler>(errorMessage);
            return new DxcCompilerResult(errorMessage);
        }

        hr = utils.Get()->CreateDefaultIncludeHandler(includeHandler.GetAddressOf());
        if (FAILED(hr))
        {
            var errrMessage = $"Failed to create the default include handler with HRESULT {hr}";
            Logger.Error<DxcCompiler>(errrMessage);
            return new DxcCompilerResult(errrMessage);
        }


        using ComPtr<IDxcBlobEncoding> source = default;
        fixed (char* pFilePath = filePath)
        {
            hr = utils.Get()->LoadFile(pFilePath, null, source.GetAddressOf());
            if (FAILED(hr))
            {
                var errorMessage = $"Failed to read the file contents with HRESULT {hr}";
                Logger.Error<DxcCompiler>(errorMessage);
                return new DxcCompilerResult(errorMessage);
            }
        }

        var args = new CompilerArgs(stackalloc char[2048]); // 2048 characters, 4kb stack allocation.
        args.AddArgument(Path.GetFileName(filePath)); // add the file name for debugging
        args.AddArgument("-E");
        args.AddArgument(entryPoint);
        args.AddArgument("-T");
        args.AddArgument(shaderModel.ToString().ToLower());
        args.AddArgument("-Qstrip_reflect");

        //NOTE(Jens): add more arguments when we want to do debug builds, pdbs etc.


        var buffer = new DxcBuffer
        {
            Encoding = 0, // assume default encoding (UTF8 with/wothout BOM)
            Ptr = source.Get()->GetBufferPointer(),
            Size = source.Get()->GetBufferSize()
        };

        using ComPtr<IDxcResult> result = default;
        using ComPtr<IDxcBlobWide> compileErrors = default;
        hr = compiler.Get()->Compile(&buffer, args.GetArguments(), args.GetArgumentsCount(), includeHandler.Get(), typeof(IDxcResult).GUID, (void**)result.GetAddressOf());

        result.Get()->GetOutput(DXC_OUT_KIND.DXC_OUT_ERRORS, typeof(IDxcBlobWide).GUID, (void**)compileErrors.GetAddressOf(), null);
        var hasErrorsOrWarnings = compileErrors.Get() != null && compileErrors.Get()->GetStringLength() != 0 && compileErrors.Get()->GetBufferPointer() != null;
        if (hasErrorsOrWarnings)
        {
            Logger.Warning<DxcCompiler>(BlobToString(compileErrors));
        }

        if (FAILED(hr))
        {
            var errorMessage = $"Failed to Compiler the shader with HRESULT {hr}";
            Logger.Error<DxcCompiler>(errorMessage);
            return hasErrorsOrWarnings ? new DxcCompilerResult(BlobToString(compileErrors)) : new DxcCompilerResult(errorMessage);
        }

        using ComPtr<IDxcBlob> byteCode = default;
        result.Get()->GetOutput(DXC_OUT_KIND.DXC_OUT_OBJECT, typeof(IDxcBlob).GUID, (void**)byteCode.GetAddressOf(), null/* shader name if we want it*/);
        if (byteCode.Get() != null)
        {
            Logger.Info<DxcCompiler>("Compilation successful!");
            return new DxcCompilerResult(byteCode);
        }

        return new DxcCompilerResult("The compiled shader has the size 0.");

        static string BlobToString(in ComPtr<IDxcBlobWide> blob) =>
            new((char*)blob.Get()->GetBufferPointer(), 0, (int)blob.Get()->GetBufferSize());
    }

    /// <summary>
    /// struct used to simply the WCHAR** data type used for compiler arguments.
    /// </summary>
    private ref struct CompilerArgs
    {
        private const int MaxArguments = 100;
        private fixed ulong _arguments[MaxArguments];
        private Span<char> _buffer;
        private int _index;
        private uint _count;
        public CompilerArgs(Span<char> buffer)
        {
            _buffer = buffer;
        }

        public void AddArgument(string arg)
        {
            var nextIndex = _index + arg.Length + 1;
            arg.CopyTo(_buffer[_index..]);
            _arguments[_count++] = (ulong)Unsafe.AsPointer(ref _buffer[_index]);
            _index = nextIndex;
        }

        public char** GetArguments() => (char**)Unsafe.AsPointer(ref _arguments[0]);
        public uint GetArgumentsCount() => _count;
    }


    private class DxcCompilerResult : ShaderCompilationResult
    {
        private ComPtr<IDxcBlob> _byteCode;
        public DxcCompilerResult(string error)
            : base(error)
        {
        }

        public DxcCompilerResult(ComPtr<IDxcBlob> byteCode)
        {
            // create a new instance to call AddRef on the ComObject
            _byteCode = new ComPtr<IDxcBlob>(byteCode);
        }

        public override ReadOnlySpan<byte> GetByteCode()
        {
            var ptr = _byteCode.Get();
            if (ptr == null || ptr->GetBufferPointer() == null || ptr->GetBufferSize() == 0)
            {
                return ReadOnlySpan<byte>.Empty;
            }
            return new ReadOnlySpan<byte>(ptr->GetBufferPointer(), (int)ptr->GetBufferSize());
        }

        public override void Dispose() => _byteCode.Dispose();
    }

}
