using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Titan.Core.Memory
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct ResourcePool<T> where T : unmanaged
    {
        private MemoryChunk<T> _data;
        private MemoryChunk<int> _freeIndices;

        private uint _maxCount;
        private volatile int _freeIndicesHead;

        public void Init(uint count)
        {
            _data = MemoryUtils.AllocateBlock<T>(count, true);
            _freeIndices = MemoryUtils.AllocateBlock<int>(count);
            for (var i = 0; i < count; ++i)
            {
                _freeIndices[i] = i;
            }
            _freeIndicesHead = 0; // Ignore the first resource, it's used for an invalid handle
            _maxCount = count;
        }

        public void Terminate()
        {
            _data.Free();
            _freeIndices.Free();
            _data = default;
            _freeIndices = default;
        }

        public Handle<T> CreateResource()
        {
            var index = Interlocked.Increment(ref _freeIndicesHead); 
            if (index >= _maxCount)
            {
                return 0;
            }
            // If a resource is released at the same time as it's created this will fail
            return _freeIndices[index];
        }

        public void ReleaseResource(in Handle<T> handle)
        {
            if (handle.IsValid() && handle.Value <= _maxCount)
            {
                var index = Interlocked.Decrement(ref _freeIndicesHead) + 1;
                if (index > 0)
                {
                    _freeIndices[index] = handle;
                }
                else
                {
                    throw new InvalidOperationException("Released more resources than was created.");
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetResourceReference(in Handle<T> handle) => ref _data[handle.Value];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* GetResourcePointer(in Handle<T> handle) => _data.GetPointer(handle.Value);
    }
}
