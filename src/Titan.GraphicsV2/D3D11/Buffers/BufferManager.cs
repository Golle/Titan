using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Titan.Core.Common;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

namespace Titan.GraphicsV2.D3D11.Buffers
{
    internal unsafe class BufferManager : IDisposable
    {
        private readonly ID3D11Device* _device;
        private ResourcePool<Buffer> _resourcePool;

        private readonly List<Handle<Buffer>> _usedHandles = new();
        public BufferManager(Device device)
        {
            _device = device.Get();
            _resourcePool.Init(10_000);
        }

        internal Handle<Buffer> Create(BufferCreation args)
        {
            var handle = _resourcePool.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create a Buffer Handle");
            }

            var bindFlag = args.Type switch
            {
                BufferTypes.ConstantBuffer => D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                BufferTypes.IndexBuffer => D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER,
                BufferTypes.VertexBuffer => D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER,
                _ => throw new ArgumentOutOfRangeException()
            };

            var desc = new D3D11_BUFFER_DESC
            {
                BindFlags = bindFlag,
                Usage = args.Usage,
                CpuAccessFlags = args.CpuAccessFlags,
                MiscFlags = args.MiscFlags,
                ByteWidth = args.Count * args.Stride,
                StructureByteStride = args.Stride
            };

            var buffer = _resourcePool.GetResourcePointer(handle);
            buffer->Handle = handle;
            buffer->BindFlag = bindFlag;
            buffer->Count = args.Count;
            buffer->CpuAccessFlag = args.CpuAccessFlags;
            buffer->MiscFlag = args.MiscFlags;
            buffer->Stride = args.Stride;
            buffer->Usage = args.Usage;

            if (args.InitialData.HasValue())
            {
                var subResource = new D3D11_SUBRESOURCE_DATA
                {
                    pSysMem = args.InitialData
                };
                CheckAndThrow(_device->CreateBuffer(&desc, &subResource, &buffer->Resource), nameof(ID3D11Device.CreateBuffer));
            }
            else
            {
                CheckAndThrow(_device->CreateBuffer(&desc, null, &buffer->Resource), nameof(ID3D11Device.CreateBuffer));
            }
            _usedHandles.Add(handle);
            return handle;
        }

        internal void Release(in Handle<Buffer> handle)
        {
            var buffer = _resourcePool.GetResourcePointer(handle);
            if (buffer->Resource != null)
            {
                buffer->Resource->Release();
            }
            *buffer = default;
            _usedHandles.Remove(handle);
            _resourcePool.ReleaseResource(handle);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly Buffer Access(in Handle<Buffer> handle) => ref _resourcePool.GetResourceReference(handle);

        public void Dispose()
        {
            foreach (var handle in _usedHandles)
            {
                Release(handle);
            }
            _resourcePool.Terminate();
        }
    }
}
