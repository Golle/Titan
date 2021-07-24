using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Windows;
using Titan.Windows.D3D11;
using static Titan.Windows.Common;

namespace Titan.Graphics.D3D11.Shaders
{
    //internal D3D11_INPUT_CLASSIFICATION Classification; // TODO: add support for instance shaders
    public record InputLayoutDescription(string Name, TextureFormats Format, uint Slot);
    public record PixelShaderCreation(MemoryChunk<byte> Buffer, string Entrypoint, string Version);
    public record VertexShaderCreation(MemoryChunk<byte> Buffer, string Entrypoint, string Version, InputLayoutDescription[] InputLayout);

    public unsafe class ShaderManager : IDisposable
    {
        private readonly ShaderCompiler _compiler = new();
        private const uint MaxShaders = 200;

        private readonly ID3D11Device* _device;

        private ResourcePool<VertexShader> _vertexShaderPool;
        private ResourcePool<PixelShader> _pixelShaderPool;

        public ShaderManager(ID3D11Device* device)
        {
            Logger.Trace<ShaderManager>($"Init with {MaxShaders} slots");
            _device = device;
            _vertexShaderPool.Init(MaxShaders);
            _pixelShaderPool.Init(MaxShaders);
        }

        public Handle<VertexShader> CreateVertexShader(VertexShaderCreation args)
        {
            if (args.InputLayout == null) throw new ArgumentNullException(nameof(VertexShaderCreation.InputLayout));
            var handle = _vertexShaderPool.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create VertexShader Handle");
            }
            var vertexShader = _vertexShaderPool.GetResourcePointer(handle);
            vertexShader->Handle = handle;

            using var vertexShaderBytecode = new ComPtr<ID3DBlob>(_compiler.Compile(args.Buffer, args.Entrypoint, args.Version));

            static uint GetSize(TextureFormats format) =>
                (uint)(format switch
                {
                    TextureFormats.RG32F => sizeof(Vector2),
                    TextureFormats.RGB32F => sizeof(Vector3),
                    TextureFormats.RGBA32F => sizeof(Vector4),
                    TextureFormats.R32U => sizeof(uint),
                    _ => throw new NotSupportedException($"Format {format} is not supported.")
                });

            static MemoryChunk<sbyte> AllocAndCopy(string value)
            {
                var memory = MemoryUtils.AllocateBlock<sbyte>((uint)(value.Length + 1));
                memory[value.Length] = 0;
                fixed (byte* pValue = Encoding.ASCII.GetBytes(value))
                {
                    Buffer.MemoryCopy(pValue, memory, value.Length, value.Length);
                }
                return memory;
            }

            var numberOfElements = args.InputLayout.Length;
            var inputLayoutDescs = stackalloc D3D11_INPUT_ELEMENT_DESC[numberOfElements];
            var size = 0u;
            for (var i = 0; i < numberOfElements; ++i)
            {
                var input = args.InputLayout[i];
                inputLayoutDescs[i] = new D3D11_INPUT_ELEMENT_DESC
                {
                    Format = (DXGI_FORMAT)input.Format,
                    InputSlot = input.Slot,
                    AlignedByteOffset = size,
                    InputSlotClass = D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_VERTEX_DATA, // Only support VertexData now
                    InstanceDataStepRate = 0,
                    SemanticIndex = 0,
                    SemanticName = AllocAndCopy(input.Name)
                };
                size += GetSize(input.Format);
            }

            CheckAndThrow(_device->CreateInputLayout(inputLayoutDescs, (uint)numberOfElements, vertexShaderBytecode.Get()->GetBufferPointer(), vertexShaderBytecode.Get()->GetBufferSize(), &vertexShader->InputLayout), nameof(ID3D11Device.CreateInputLayout));
            for (var i = 0; i < numberOfElements; ++i)
            {
                ((MemoryChunk<sbyte>)inputLayoutDescs[i].SemanticName).Free();
            }

            CheckAndThrow(_device->CreateVertexShader(vertexShaderBytecode.Get()->GetBufferPointer(), vertexShaderBytecode.Get()->GetBufferSize(), null, &vertexShader->Shader), nameof(ID3D11Device.CreateVertexShader));
            
            return handle;
        }


        public Handle<PixelShader> CreatePixelShader(PixelShaderCreation args)
        {
            var handle = _pixelShaderPool.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create VertexShader Handle");
            }
            var pixelShader = _pixelShaderPool.GetResourcePointer(handle);
            pixelShader->Handle = handle;
            
            using var pixelShaderBytecode = new ComPtr<ID3DBlob>(_compiler.Compile(args.Buffer, args.Entrypoint, args.Version));
            CheckAndThrow(_device->CreatePixelShader(pixelShaderBytecode.Get()->GetBufferPointer(), pixelShaderBytecode.Get()->GetBufferSize(), null, &pixelShader->Shader), nameof(ID3D11Device.CreatePixelShader));
            
            return handle;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly PixelShader Access(in Handle<PixelShader> handle) => ref _pixelShaderPool.GetResourceReference(handle);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly VertexShader Access(in Handle<VertexShader> handle) => ref _vertexShaderPool.GetResourceReference(handle);


        public void Release(in Handle<PixelShader> handle)
        {
            ReleaseInternal(handle);
            _pixelShaderPool.ReleaseResource(handle);
        }

        private void ReleaseInternal(in Handle<PixelShader> handle)
        {
            var shader = _pixelShaderPool.GetResourcePointer(handle);
            if (shader->Shader != null)
            {
                shader->Shader->Release();
            }
            *shader = default;
        }

        public void Release(in Handle<VertexShader> handle)
        {
            ReleaseInternal(handle);
            _vertexShaderPool.ReleaseResource(handle);
        }

        private void ReleaseInternal(in Handle<VertexShader> handle)
        {
            var shader = _vertexShaderPool.GetResourcePointer(handle);
            if (shader->Shader != null)
            {
                shader->Shader->Release();
            }

            if (shader->InputLayout != null)
            {
                shader->InputLayout->Release();
            }
            *shader = default;
        }

        public void Dispose()
        {
            foreach (var handle in _vertexShaderPool.EnumerateUsedResources())
            {
                Logger.Warning<ShaderManager>($"Releasing a unreleased Vertex Shader with handle {handle.Value}");
                ReleaseInternal(handle);
            }

            foreach (var handle in _pixelShaderPool.EnumerateUsedResources())
            {
                Logger.Warning<ShaderManager>($"Releasing a unreleased Pixel Shader with handle {handle.Value}");
                ReleaseInternal(handle);
            }
            Logger.Trace<ShaderManager>("Terminate resource pools");
            _vertexShaderPool.Terminate();
            _pixelShaderPool.Terminate();
        }
    }
}
