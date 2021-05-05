using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Titan.ECS
{
    internal class IdContainer
    {
        private readonly uint _max;
        private readonly ConcurrentBag<uint> _ids = new();
        private uint _nextId;

        public IdContainer(uint max)
        {
            _max = max;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Next()
        {
            Debug.Assert(_nextId < _max, $"Max ID reached: {_max}");
            return _ids.TryTake(out var id) ? id : Interlocked.Increment(ref _nextId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Return(uint id) => _ids.Add(id);
    }
}
