using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Common;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    
    
    internal unsafe class VertexBufferManager : IVertexBufferManager
    {
        private readonly IMemoryManager _memoryManager;
        private ComPtr<ID3D11Device> _device;
        private VertexBuffer* _buffers;
        private uint _maxBuffers;
        private int _numberOfBuffers;

        private readonly ConcurrentQueue<int> _freeHandles = new();
        public VertexBufferManager(IMemoryManager memoryManager)
        {
            _memoryManager = memoryManager;

        }

        public void Initialize(IGraphicsDevice graphicsDevice)
        {
            if (_buffers != null)
            {
                throw new InvalidOperationException($"{nameof(VertexBufferManager)} has already been initialized.");
            }
            _device = graphicsDevice is D3D11GraphicsDevice device ? new ComPtr<ID3D11Device>(device.Ptr) : throw new ArgumentException($"Trying to initialize a D3D11 {nameof(VertexBufferManager)} with the wrong device.", nameof(graphicsDevice));

            var memory = _memoryManager.GetMemoryChunkValidated<VertexBuffer>("VertexBuffer");
            _buffers = memory.Pointer;
            _maxBuffers = memory.Count;
        }

        public Handle<VertexBuffer> CreateVertexBuffer(uint count, uint stride, void* initialData, D3D11_USAGE usage, D3D11_CPU_ACCESS_FLAG cpuAccess, D3D11_RESOURCE_MISC_FLAG miscFlags)
        {
            Debug.Assert(!_freeHandles.IsEmpty || _numberOfBuffers < _maxBuffers, "Max number of buffers have been reached.");

            // TODO: this is not thread safe.
            if (!_freeHandles.TryDequeue(out var handle))
            {
                handle = Interlocked.Increment(ref _numberOfBuffers) - 1;
            }
            var desc = new D3D11_BUFFER_DESC
            {
                Usage = usage,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER,
                ByteWidth = stride * count,
                MiscFlags = miscFlags,
                CpuAccessFlags = cpuAccess,
                StructureByteStride = stride
            };
            
            ref var vertexBuffer = ref _buffers[handle];
            vertexBuffer.CpuAccessFlag = desc.CpuAccessFlags;
            vertexBuffer.Usage = desc.Usage;
            vertexBuffer.Stride = desc.StructureByteStride;

            if (initialData == null)
            {
                Common.CheckAndThrow(_device.Get()->CreateBuffer(&desc, null, &_buffers[handle].Pointer), "CreateBuffer");
            }
            else
            {
                var data = new D3D11_SUBRESOURCE_DATA
                {
                    pSysMem = initialData
                };
                Common.CheckAndThrow(_device.Get()->CreateBuffer(&desc, &data, &_buffers[handle].Pointer), "CreateBuffer");
            }
            
            return handle;
        }

        public void DestroyBuffer(in Handle<VertexBuffer> handle)
        {
            ref var buffer = ref _buffers[handle];
            if (buffer.Pointer != null)
            {
                buffer.Pointer->Release();
                buffer.Pointer = null;
                _freeHandles.Enqueue(handle);
            }
        }

        public ref readonly VertexBuffer this[in Handle<VertexBuffer> handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffers[handle];
        }

        public void Dispose()
        {
            if (_buffers != null)
            {
                for (var i = 0; i < _numberOfBuffers; ++i)
                {
                    ref var buffer = ref _buffers[i];
                    if (buffer.Pointer != null)
                    {
                        buffer.Pointer->Release();
                        buffer.Pointer = null;
                    }
                }
                _numberOfBuffers = 0;
                _device.Dispose();
                _buffers = null;
            }
            
        }
    }
}
