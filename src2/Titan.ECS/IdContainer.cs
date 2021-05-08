using System.Collections.Concurrent;
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
        public uint Next() => _availableIds[Interlocked.Decrement(ref _head)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(uint id) => _availableIds[Interlocked.Increment(ref _head) - 1] = id;
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
