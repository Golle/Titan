using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.Core.Logging;

namespace Titan.Core.Memory
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public unsafe struct ResourcePool<T> where T : unmanaged
    {
        private MemoryChunk<T> _data;
        private MemoryChunk<int> _usedData;
        private volatile int _nextResource;

        private int _maxCount;
        private static readonly int Offset = new Random().Next(100, 100000);

        public void Init(uint count)
        {
            _data = MemoryUtilsOld.AllocateBlock<T>(count, true);
            _usedData = MemoryUtilsOld.AllocateBlock<int>(count, true);
            _maxCount = (int) count;
        }

        public void Terminate()
        {
            _data.Free();
            _usedData.Free();
            _data = default;
            _usedData = default;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Handle<T> CreateResource() => GetFreeIndex() + Offset;

        private int GetFreeIndex()
        {
            var maxIterations = _maxCount;
            while (maxIterations-- > 0)
            {
                var current = _nextResource;
                var index = Interlocked.CompareExchange(ref _nextResource, (current + 1) % _maxCount, current);
                // Some other thread updated the counter, do another lap
                if (index != current)
                {
                    Logger.Trace("index != current", GetType());
                    continue;
                }
                
                var previousStatus = Interlocked.CompareExchange(ref _usedData[index], 1, 0);
                // If the resource is used, loop again and try to find a new spot
                if (previousStatus != 0)
                {
                    Logger.Trace("previousState !=0", GetType());
                    continue;
                }

                return index;
            }
            ThrowException();
            static void ThrowException()
            {
                throw new InvalidOperationException("MaxIterations to allocate a resource has been reached, can't continue.");
            }
            // Unreachable code
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReleaseResource(in Handle<T> handle)
        {
            var index = handle - Offset;
            if (handle.IsValid() && index <= _maxCount)
            {
                _usedData[index] = 0;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetResourceReference(in Handle<T> handle) => ref _data[handle.Value-Offset];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T* GetResourcePointer(in Handle<T> handle) => _data.GetPointer(handle.Value-Offset);


        /// <summary>
        /// Should only be used to cleanup resources, and never inside a game loop.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<Handle<T>> EnumerateUsedResources()
        {
            for (var i = 0; i < _maxCount; ++i)
            {
                if (_usedData[i] == 1)
                {
                    yield return i + Offset;
                }
            }
        }
    }
}
