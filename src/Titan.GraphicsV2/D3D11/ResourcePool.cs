using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.Core.Common;

namespace Titan.GraphicsV2.D3D11
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal unsafe struct ResourcePool<T> where T : unmanaged
    {
        private T* _data;
        private int* _freeIndices;

        private int _maxCount;
        private volatile int _freeIndicesHead;

        internal void Init(int count)
        {
            _data = (T*) Marshal.AllocHGlobal(sizeof(T) * count);
            if (_data == null)
            {
                throw new OutOfMemoryException("Failed to allocated memory for resource");
            }

            _freeIndices = (int*) Marshal.AllocHGlobal(count * sizeof(uint));
            if (_freeIndices == null)
            {
                throw new OutOfMemoryException("Failed to allocated memory for resource");
            }

            for (var i = 0; i < count; ++i)
            {
                _freeIndices[i] = i;
            }
            _freeIndicesHead = 0; // Ignore the first resource, it's used for an invalid handle
            _maxCount = count;
        }

        internal void Terminate()
        {
            if (_data != null)
            {
                Marshal.FreeHGlobal((nint)_data);
                _data = null;
            }
            if (_freeIndices != null)
            {
                Marshal.FreeHGlobal((nint)_freeIndices);
                _freeIndices = null;
            }
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
        public T* GetResourcePointer(in Handle<T> handle) => & _data[handle.Value];
    }
}
