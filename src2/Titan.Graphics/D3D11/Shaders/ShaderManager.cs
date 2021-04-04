using System;
using System.Collections.Generic;
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
    public unsafe class ShaderManager : IDisposable
    {
        private readonly ShaderCompiler _compiler = new();
        private readonly ID3D11Device* _device;
        private ResourcePool<Shader> _resourcePool;
        private readonly List<Handle<Shader>> _usedHandles = new();

        private const uint MaxShaders = 1000;
        public ShaderManager(ID3D11Device*  device)
        {
            Logger.Trace<ShaderManager>($"Init with {MaxShaders} slots");
            _device = device;
            _resourcePool.Init(MaxShaders);
        }

        internal Handle<Shader> Create(ShaderCreation args)
        {
            // TODO: implement a cache for shaders
            // TODO: if we're going to support Reloading shaders we need to track the resources alloced so they can be released if an exception is thrown
            // TODO: support loading just a single shader (either VS+Input or PS). would this ever be used?
            
            if (args.InputLayout == null) throw new ArgumentNullException(nameof(ShaderCreation.InputLayout));

            var handle = _resourcePool.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create Shader Handle");
            }
            var shader = _resourcePool.GetResourcePointer(handle);
            shader->Handle = handle;


            using var vertexShaderBytecode = new ComPtr<ID3DBlob>(_compiler.Compile(args.VertexShader.Source, args.VertexShader.Entrypoint, args.VertexShader.Version));
            using var pixelShaderBytecode = new ComPtr<ID3DBlob>(_compiler.Compile(args.PixelShader.Source, args.PixelShader.Entrypoint, args.PixelShader.Version));

            static uint GetSize(TextureFormats format) =>
                (uint)(format switch
                {
                    TextureFormats.RG32F => sizeof(Vector2),
                    TextureFormats.RGB32F => sizeof(Vector3),
                    TextureFormats.RGBA32F => sizeof(Vector4),
                    _ => throw new NotSupportedException($"Format {format} is not supported.")
                });

            static MemoryChunk<sbyte> AllocAndCopy(string value)
            {
                var memory = MemoryUtils.AllocateBlock<sbyte>((uint) (value.Length + 1));
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

            CheckAndThrow(_device->CreateInputLayout(inputLayoutDescs, (uint)numberOfElements, vertexShaderBytecode.Get()->GetBufferPointer(), vertexShaderBytecode.Get()->GetBufferSize(), &shader->InputLayout), nameof(ID3D11Device.CreateInputLayout));
            for (var i = 0; i < numberOfElements; ++i)
            {
                ((MemoryChunk<sbyte>)inputLayoutDescs[i].SemanticName).Free();
            }

            CheckAndThrow(_device->CreateVertexShader(vertexShaderBytecode.Get()->GetBufferPointer(), vertexShaderBytecode.Get()->GetBufferSize(), null, &shader->VertexShader), nameof(ID3D11Device.CreateVertexShader));
            CheckAndThrow(_device->CreatePixelShader(pixelShaderBytecode.Get()->GetBufferPointer(), pixelShaderBytecode.Get()->GetBufferSize(), null, &shader->PixelShader), nameof(ID3D11Device.CreatePixelShader));

            _usedHandles.Add(handle);
            return handle;
        }


        internal void Release(in Handle<Shader> handle)
        {
            ReleaseInternal(handle);

            _usedHandles.Remove(handle);
            _resourcePool.ReleaseResource(handle);
        }

        private void ReleaseInternal(in Handle<Shader> handle)
        {
            var shader = _resourcePool.GetResourcePointer(handle);
            if (shader->InputLayout != null)
            {
                shader->InputLayout->Release();
            }
            if (shader->PixelShader != null)
            {
                shader->PixelShader->Release();
            }
            if (shader->VertexShader != null)
            {
                shader->VertexShader->Release();
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly Shader Access(in Handle<Shader> handle) => ref _resourcePool.GetResourceReference(handle);

        public void Dispose()
        {
            if (_usedHandles.Count > 0)
            {
                Logger.Warning<ShaderManager>($"{_usedHandles.Count} unreleased resources when disposing the manager");
                Logger.Trace<ShaderManager>($"Releasing {_usedHandles.Count} shaders");
            }
            foreach (var handle in _usedHandles)
            {
                ReleaseInternal(handle);
            }
            Logger.Trace<ShaderManager>("Terminate resource pool");
            _resourcePool.Terminate();
        }
    }
}
