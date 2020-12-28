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
    internal unsafe class ConstantBufferManager : IConstantBufferManager
    {
        private readonly IMemoryManager _memoryManager;
        private ComPtr<ID3D11Device> _device;
        
        private ConstantBuffer* _buffers;
        private uint _maxBuffers;
        private int _numberOfBuffers;

        private readonly ConcurrentQueue<int> _freeHandles = new();

        public ConstantBufferManager(IMemoryManager memoryManager)
        {
            _memoryManager = memoryManager;
        }

        public void Initialize(IGraphicsDevice graphicsDevice)
        {
            if (_buffers != null)
            {
                throw new InvalidOperationException($"{nameof(ConstantBufferManager)} has already been initialized.");
            }
            _device = graphicsDevice is GraphicsDevice device ? new ComPtr<ID3D11Device>(device.Ptr) : throw new ArgumentException($"Trying to initialize a D3D11 {nameof(ConstantBufferManager)} with the wrong device.", nameof(graphicsDevice));
            var memory = _memoryManager.GetMemoryChunkValidated<ConstantBuffer>("ConstantBuffer");
            _buffers = memory.Pointer;
            _maxBuffers = memory.Count;
        }

        public Handle<ConstantBuffer> CreateConstantBuffer<T>(in T data = default, D3D11_USAGE usage = default, D3D11_CPU_ACCESS_FLAG cpuAccess = default, D3D11_RESOURCE_MISC_FLAG miscFlags = default) where T : unmanaged
        {
            Debug.Assert(sizeof(T) % 16 == 0, "ConstantBuffer must be 16 byte aligned");
            Debug.Assert(!_freeHandles.IsEmpty || _numberOfBuffers < _maxBuffers, "Max number of buffers have been reached.");

            // TODO: this is not thread safe.
            if (!_freeHandles.TryDequeue(out var handle))
            {
                handle = Interlocked.Increment(ref _numberOfBuffers) - 1;
            }

            var size = (uint)sizeof(T);
            var desc = new D3D11_BUFFER_DESC
            {
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                ByteWidth = size,
                StructureByteStride = size,
                CpuAccessFlags = cpuAccess,
                MiscFlags = miscFlags,
                Usage = usage,
            };
            fixed (void* pData = &data)
            {
                var subresourceData = new D3D11_SUBRESOURCE_DATA
                {
                    pSysMem = pData,
                };
                Common.CheckAndThrow(_device.Get()->CreateBuffer(&desc, &subresourceData, &_buffers[handle].Pointer), "CreateBuffer");
            }
            return handle;
        }

        public void DestroyBuffer(in Handle<ConstantBuffer> handle)
        {
            ref var buffer = ref _buffers[handle];
            if (buffer.Pointer != null)
            {
                buffer.Pointer->Release();
                buffer.Pointer = null;
                _freeHandles.Enqueue(handle);
            }
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
            }
        }

        public ref readonly ConstantBuffer this[in Handle<ConstantBuffer> handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffers[handle];
        }
    }
}
