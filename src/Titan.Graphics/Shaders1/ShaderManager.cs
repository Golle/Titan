using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Graphics.Shaders;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Shaders1
{
    internal unsafe class ShaderManager : IShaderManager
    {
        private ComPtr<ID3D11Device> _device;

        private readonly TitanConfiguration _configuration; // TODO: should the configuration be here?
        private readonly IShaderCompiler _shaderCompiler;

        private readonly IDictionary<PixelShaderDescriptor, PixelShaderHandle> _pixelShaderCache = new Dictionary<PixelShaderDescriptor, PixelShaderHandle>();
        private readonly IDictionary<VertexShaderDescriptor, VertexShaderHandle> _vertexShaderCache = new Dictionary<VertexShaderDescriptor, VertexShaderHandle>();
        private readonly IDictionary<string, InputLayoutHandle> _inputLayoutCache = new Dictionary<string, InputLayoutHandle>();


        private readonly void* _memory;
        private readonly uint _maxHandles;

        private int _handle;

        public ShaderManager(ID3D11Device* device, IMemoryManager memoryManager, IShaderCompiler shaderCompiler, TitanConfiguration configuration)
        {
            Debug.Assert(sizeof(VertexShader) == sizeof(PixelShader) && sizeof(PixelShader) == sizeof(InputLayout), "VertexShader, PixelShader and InputLayout must be of the same size.");

            _device = new ComPtr<ID3D11Device>(device);
            _shaderCompiler = shaderCompiler;
            _configuration = configuration;

            var memory = memoryManager.GetMemoryChunk("Shaders");
            _memory = memory.Pointer;
            _maxHandles = memory.Count;
        }

        public ShaderProgram GetOrCreate(in VertexShaderDescriptor vertexShaderDescriptor, in PixelShaderDescriptor pixelShaderDescriptor, in InputLayoutDescriptor[] layout)
        {
            var inputLayoutKey = CreateInputLayoutKey(layout);
            var hasCachedInputHandle = _inputLayoutCache.TryGetValue(inputLayoutKey, out var inputHandle);

            if (!_vertexShaderCache.TryGetValue(vertexShaderDescriptor, out var vertexShaderHandle))
            {
                vertexShaderHandle = NextHandle();
                using var shader = _shaderCompiler.CompileShaderFromFile(_configuration.GetPath(vertexShaderDescriptor.Filename), vertexShaderDescriptor.Entrypoint, vertexShaderDescriptor.Version, vertexShaderDescriptor.Defines);
                Common.CheckAndThrow(_device.Get()->CreateVertexShader(shader.Buffer, shader.BufferSize, null, &((VertexShader*)_memory)[vertexShaderHandle].Pointer), "CreateVertexShader");
                _vertexShaderCache.Add(vertexShaderDescriptor, vertexShaderHandle);
                if (!hasCachedInputHandle)
                {
                    inputHandle = CreateInputLayout(shader, layout);
                    _inputLayoutCache.Add(inputLayoutKey, inputHandle);
                }
            }

            var pixelShaderHandle = GetOrCreatePixelShader(pixelShaderDescriptor);
            return new ShaderProgram(vertexShaderHandle, pixelShaderHandle, inputHandle);
        }

        private PixelShaderHandle GetOrCreatePixelShader(PixelShaderDescriptor pixelShaderDescriptor)
        {
            if (!_pixelShaderCache.TryGetValue(pixelShaderDescriptor, out var pixelShaderHandle))
            {
                pixelShaderHandle = NextHandle();
                using var shader = _shaderCompiler.CompileShaderFromFile(_configuration.GetPath(pixelShaderDescriptor.Filename), pixelShaderDescriptor.Entrypoint, pixelShaderDescriptor.Version, pixelShaderDescriptor.Defines);
                
                Common.CheckAndThrow(_device.Get()->CreatePixelShader(shader.Buffer, shader.BufferSize, null, &((PixelShader*) _memory)[pixelShaderHandle].Pointer), "CreatePixelShader");
                _pixelShaderCache.Add(pixelShaderDescriptor, pixelShaderHandle);
            }

            return pixelShaderHandle;
        }

        private InputLayoutHandle CreateInputLayout(CompiledShader shader, in InputLayoutDescriptor[] layout)
        {
            static uint GetSize(DXGI_FORMAT format) =>
                (uint)(format switch
                {
                    DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT => sizeof(Vector2),
                    DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT => sizeof(Vector3),
                    DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT => sizeof(Vector4),
                    _ => throw new NotSupportedException($"Format {format} is not supported.")
                });
            
            var handle = NextHandle();

            var inputDescs = stackalloc D3D11_INPUT_ELEMENT_DESC[layout.Length];
            var size = 0u;
            for (var i = 0; i < layout.Length; ++i)
            {
                var (name, format, classification) = layout[i];
                ref var desc = ref inputDescs[i];
                fixed (byte* pName = name.AsBytes())
                {
                    desc.SemanticName = (sbyte*)pName;
                }

                //TODO: This needs some changes to support different things properly
                desc.InstanceDataStepRate = classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 1 : 0;
                desc.InputSlot = classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 1 : 0;
                desc.InputSlotClass = classification;
                desc.SemanticIndex = classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 1 : 0;
                desc.AlignedByteOffset = classification == D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA ? 0 : size;
                desc.Format = format;

                size += GetSize(format);
            }

            Common.CheckAndThrow(_device.Get()->CreateInputLayout(inputDescs, (uint)layout.Length, shader.Buffer, shader.BufferSize, &((InputLayout*)_memory)[handle].Pointer), "CreateInputLayout");
            return handle;
        }
        
        private int NextHandle() => Interlocked.Increment(ref _handle) - 1;
        private static string CreateInputLayoutKey(in InputLayoutDescriptor[] layout) => string.Join('#', layout.Select(l => $"{l.Name}:{l.Format}:{l.Classification}".ToLowerInvariant()));
        public ref readonly InputLayout this[in InputLayoutHandle handle] => ref ((InputLayout*)_memory)[handle];
        public ref readonly PixelShader this[in PixelShaderHandle handle] => ref ((PixelShader*)_memory)[handle];
        public ref readonly VertexShader this[in VertexShaderHandle handle] => ref ((VertexShader*)_memory)[handle];

        public void Dispose()
        {
            for (var i = 0; i < _handle; ++i)
            {
                ref var pUnknown = ref ((IUnknown**) _memory)[i];
                pUnknown->Release();
                pUnknown = null;
            }
            _handle = 0;
            _device.Dispose();
        }
    }
}
