using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.ECS.Events;
using Titan.ECS.World;

namespace Titan.ECS.Entities
{
    internal class EntityFactory : IEntityFactory
    {
        private readonly IEventQueue _eventQueue;
        private readonly uint _worldId;
        private uint _nextId;
        private readonly ConcurrentQueue<uint> _freeIds = new();

        public EntityFactory(WorldConfiguration configuration, IEventQueue eventQueue)
        {
            _eventQueue = eventQueue;
            _worldId = configuration.WorldId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Create() => new(_freeIds.TryDequeue(out var id) ? id : Interlocked.Increment(ref _nextId), _worldId);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update()
        {
            foreach (ref readonly var @event in _eventQueue.GetEvents())
            {
                if (@event.Type == EntityDestroyedEvent.Id)
                {
                    _freeIds.Enqueue(@event.As<EntityDestroyedEvent>().EntityId);
                }
            }
        }
    }
}
