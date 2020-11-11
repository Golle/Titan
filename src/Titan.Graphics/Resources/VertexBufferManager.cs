using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Memory;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    internal unsafe class VertexBufferManager : IVertexBufferManager
    {
        private ComPtr<ID3D11Device> _device;
        private readonly VertexBuffer* _buffers;
        private readonly uint _maxBuffers;
        private int _numberOfBuffers;

        private readonly ConcurrentQueue<int> _freeHandles = new ConcurrentQueue<int>();
        public VertexBufferManager(ID3D11Device* device, IMemoryManager memoryManager)
        {
            _device = new ComPtr<ID3D11Device>(device);
            
            var memory = memoryManager.GetMemoryChunkValidated<VertexBuffer>("VertexBuffer");
            _buffers = memory.Pointer;
            _maxBuffers = memory.Count;
        }

        public VertexBufferHandle CreateVertexBuffer(uint count, uint stride, void* initialData, D3D11_USAGE usage, D3D11_CPU_ACCESS_FLAG cpuAccess, D3D11_RESOURCE_MISC_FLAG miscFlags)
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

        public void DestroyBuffer(in VertexBufferHandle handle)
        {
            ref var buffer = ref _buffers[handle];
            if (buffer.Pointer != null)
            {
                buffer.Pointer->Release();
                buffer.Pointer = null;
                _freeHandles.Enqueue(handle);
            }
        }

        public ref readonly VertexBuffer this[in VertexBufferHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffers[handle];
        }

        public void Dispose()
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
}
