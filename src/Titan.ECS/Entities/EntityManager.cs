using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Titan.ECS.Entities
{
    internal class EntityManager : IEntityManager
    {
        private readonly uint _worldId;
        private uint _nextId;
        private readonly ConcurrentQueue<uint> _freeIds = new();

        public EntityManager(uint worldId)
        {
            _worldId = worldId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Create() => new(_freeIds.TryDequeue(out var id) ? id : Interlocked.Increment(ref _nextId), _worldId);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(in Entity entity)
        {
            Debug.Assert(entity.WorldId == _worldId, "Trying to destroy an entity that belongs to another world.");
            _freeIds.Enqueue(entity.Id); // TODO: handle this by messages and delayed destruction to support multiple threads
        }
    }
}
