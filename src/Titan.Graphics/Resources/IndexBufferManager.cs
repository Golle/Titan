using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    internal unsafe class IndexBufferManager : IIndexBufferManager
    {
        private readonly IMemoryManager _memoryManager;
        private ComPtr<ID3D11Device> _device;
        private IndexBuffer* _buffers;
        private uint _maxBuffers;
        private int _numberOfBuffers;

        private readonly ConcurrentQueue<int> _freeHandles = new();
        public IndexBufferManager(IMemoryManager memoryManager)
        {
            _memoryManager = memoryManager;
        }

        public void Initialize(IGraphicsDevice graphicsDevice)
        {
            if (_buffers != null)
            {
                throw new InvalidOperationException($"{nameof(IndexBufferManager)} has already been initialized.");
            }
            _device = graphicsDevice is GraphicsDevice device ? new ComPtr<ID3D11Device>(device.Ptr) : throw new ArgumentException($"Trying to initialize a D3D11 {nameof(IndexBufferManager)} with the wrong device.", nameof(graphicsDevice));

            var memory = _memoryManager.GetMemoryChunkValidated<IndexBuffer>("IndexBuffer");
            _buffers = memory.Pointer;
            _maxBuffers = memory.Count;
        }

        public Handle<IndexBuffer> CreateIndexBuffer<T>(uint count, void* initialData = null, D3D11_USAGE usage = default, D3D11_CPU_ACCESS_FLAG cpuAccess = default, D3D11_RESOURCE_MISC_FLAG miscFlags = default) where T : unmanaged
        {
            Debug.Assert(typeof(T) == typeof(ushort) || typeof(T) == typeof(uint), "Only int/uint or short/ushort are supported");
            Debug.Assert(!_freeHandles.IsEmpty || _numberOfBuffers < _maxBuffers, "Max number of buffers have been reached.");

            // TODO: this is not thread safe.
            if (!_freeHandles.TryDequeue(out var handle))
            {
                handle = Interlocked.Increment(ref _numberOfBuffers) - 1;
            }
            var stride = (uint)sizeof(T);
            var desc = new D3D11_BUFFER_DESC
            {
                Usage = usage,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER,
                ByteWidth = stride * count,
                MiscFlags = miscFlags,
                CpuAccessFlags = cpuAccess,
                StructureByteStride = stride
            };

            ref var indexBuffer = ref _buffers[handle];
            indexBuffer.Format = stride == 2 ? DXGI_FORMAT.DXGI_FORMAT_R16_UINT : DXGI_FORMAT.DXGI_FORMAT_R32_UINT;
            indexBuffer.NumberOfIndices = count;

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

        public void DestroyBuffer(in Handle<IndexBuffer> handle)
        {
            ref var buffer = ref _buffers[handle];
            if (buffer.Pointer != null)
            {
                buffer.Pointer->Release();
                buffer.Pointer = null;
                _freeHandles.Enqueue(handle);
            }
        }

        public ref readonly IndexBuffer this[in Handle<IndexBuffer> handle]
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
                    if (_buffers->Pointer != null)
                    {
                        _buffers[i].Pointer->Release();
                        _buffers[i].Pointer = null;
                    }
                }
                _numberOfBuffers = 0;
                _device.Dispose();
                _buffers = null;
            }
        }
    }
}
