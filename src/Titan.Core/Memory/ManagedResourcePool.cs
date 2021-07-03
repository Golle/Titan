using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Logging;

namespace Titan.Core.Memory
{
    public struct ManagedResourcePool<T>
    {
        private static readonly int Offset = new Random().Next(100, 100000);

        private PinnedMemoryChunk<T> _data;
        private int _maxCount;
        private volatile int _nextResource;

        public void Init(int count)
        {
            _data = MemoryUtils.AllocateArray<T>(count, true);
            _maxCount = count;
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

                // If the resource is used, loop again and try to find a new spot
                if (_data[index] != null)
                {
                    Logger.Trace("previousState != null", GetType());
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
        public ref T GetResourceReference(in Handle<T> handle) => ref _data[handle.Value - Offset];
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResource(in Handle<T> handle, in T data) => _data[handle.Value - Offset] = data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetResource(in Handle<T> handle) => _data[handle.Value - Offset];

        public void Terminate()
        {
            _data = default;
        }
    }
}
