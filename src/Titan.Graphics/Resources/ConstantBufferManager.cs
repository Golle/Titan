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
        private ComPtr<ID3D11Device> _device;
        private readonly ConstantBuffer* _buffers;
        private readonly uint _maxBuffers;
        private int _numberOfBuffers;

        private readonly ConcurrentQueue<int> _freeHandles = new ConcurrentQueue<int>();

        public ConstantBufferManager(ID3D11Device* device, IMemoryManager memoryManager)
        {
            _device = new ComPtr<ID3D11Device>(device);

            var memory = memoryManager.GetMemoryChunk("ConstantBuffer");
            Debug.Assert(memory.Stride == sizeof(ConstantBuffer), "The stride of the memory chunk is not matching the expected size");
            _buffers = (ConstantBuffer*)memory.Pointer;
            _maxBuffers = memory.Count;
        }
        public ConstantBufferHandle CreateConstantBuffer<T>(in T data = default, D3D11_USAGE usage = default, D3D11_CPU_ACCESS_FLAG cpuAccess = default, D3D11_RESOURCE_MISC_FLAG miscFlags = default) where T : unmanaged
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
                Common.CheckAndThrow(_device.Get()->CreateBuffer(&desc, &subresourceData, &_buffers[handle].Buffer), "CreateBuffer");
            }
            return handle;
        }

        public void DestroyBuffer(in ConstantBufferHandle handle)
        {
            ref var buffer = ref _buffers[handle];
            if (buffer.Buffer != null)
            {
                buffer.Buffer->Release();
                buffer.Buffer = null;
                _freeHandles.Enqueue(handle);
            }
        }

        public void Dispose()
        {
            _device.Dispose();
            for (var i = 0; i < _numberOfBuffers; ++i)
            {
                _buffers[i].Buffer->Release();
                _buffers[i].Buffer = null;
            }
            _numberOfBuffers = 0;
        }

        public ref readonly ConstantBuffer this[in ConstantBufferHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _buffers[handle];
        }
    }
}
