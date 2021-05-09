using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Titan.ECS
{

    //TODO: faster but a bit more complex, worth it?
    internal class IdContainer
    {
        private readonly uint _max;
        private readonly uint[] _availableIds;
        private uint _head;
        public IdContainer(uint max)
        {
            _max = max;
            _availableIds = new uint[max];
            for (var i = 1u; i < _availableIds.Length; ++i)
            {
                _availableIds[i] = max - i;
            }
            _head = max;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Next()
        {
            var index = Interlocked.Decrement(ref _head);
            Debug.Assert(index != 0, "Max number of ids has been reached.");
            return _availableIds[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(uint id)
        {
            // TODO: add a check it's a valid id that gets returned, or we might have duplicates
            _availableIds[Interlocked.Increment(ref _head) - 1] = id;
        }
    }

    // TODO: slower, but easier to understand
    internal class IdContainerWithBag
    {
        private readonly uint _max;
        private readonly ConcurrentBag<uint> _ids = new();
        private uint _nextId;
        public IdContainerWithBag(uint max) => _max = max;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Next() => _ids.TryTake(out var id) ? id : Interlocked.Increment(ref _nextId);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(uint id) => _ids.Add(id);
    }
}
