using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.ECS.Messaging;
using Titan.ECS.Messaging.Events;
using Titan.ECS.World;

namespace Titan.ECS.Entities
{
    internal class EntityFactory : IEntityFactory
    {
        private readonly IEventManager _eventManager;
        private readonly uint _worldId;
        private volatile uint _nextId;
        private readonly ConcurrentQueue<uint> _freeIds = new();

        public EntityFactory(WorldConfiguration configuration, IEventManager eventManager)
        {
            _eventManager = eventManager;
            _worldId = configuration.WorldId;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Entity Create() => new(_freeIds.TryDequeue(out var id) ? id : Interlocked.Increment(ref _nextId), _worldId);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update()
        {
            foreach (ref readonly var @event in _eventManager.GetEvents())
            {
                if (@event.Type == EntityDestroyedEvent.Id)
                {
                    _freeIds.Enqueue(@event.As<EntityDestroyedEvent>().EntityId);
                }
            }
        }
    }
}
