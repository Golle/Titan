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
        private ComPtr<ID3D11Device> _device;
        private readonly IndexBuffer* _buffers;
        private readonly uint _maxBuffers;
        private int _numberOfBuffers;

        private readonly ConcurrentQueue<int> _freeHandles = new ConcurrentQueue<int>();
        public IndexBufferManager(IGraphicsDevice device, IMemoryManager memoryManager)
        {
            _device = new ComPtr<ID3D11Device>(device.Ptr);

            var memory = memoryManager.GetMemoryChunk("IndexBuffer");
            Debug.Assert(memory.Stride == sizeof(IndexBuffer), "The stride of the memory chunk is not matching the expected size");
            _buffers = (IndexBuffer*)memory.Pointer;
            _maxBuffers = memory.Count;
        }

        public IndexBufferHandle CreateIndexBuffer<T>(uint count, void* initialData = null, D3D11_USAGE usage = default, D3D11_CPU_ACCESS_FLAG cpuAccess = default, D3D11_RESOURCE_MISC_FLAG miscFlags = default) where T : unmanaged
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

        public void DestroyBuffer(in IndexBufferHandle handle)
        {
            ref var buffer = ref _buffers[handle];
            if (buffer.Pointer != null)
            {
                buffer.Pointer->Release();
                buffer.Pointer = null;
                _freeHandles.Enqueue(handle);
            }
        }

        public ref readonly IndexBuffer this[in IndexBufferHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffers[handle];
        }

        public void Dispose()
        {
            _device.Dispose();
            for (var i = 0; i < _numberOfBuffers; ++i)
            {
                _buffers[i].Pointer->Release();
                _buffers[i].Pointer = null;
            }
            _numberOfBuffers = 0;
        }
    }
}
