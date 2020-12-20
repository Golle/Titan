using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Events;

namespace Titan.ECS.World
{
    internal class EntityFilterManager : IEntityFilterManager, IDisposable
    {
        private readonly IEventQueue _eventQueue;
        private readonly IList<EntityFilter> _filters = new List<EntityFilter>(100);

        private readonly uint _maxEntities;
        
        public EntityFilterManager(WorldConfiguration configuration, IEventQueue eventQueue)
        {
            _eventQueue = eventQueue;
            _maxEntities = configuration.MaxEntities;
        }

        public IEntityFilter Create(EntityFilterConfiguration configuration)
        {
            var filter = _filters.FirstOrDefault(f => f.ComponentMask == configuration.ComponentMask); // Re-use existing filters
            if (filter == null)
            {
                _filters.Add(filter = new EntityFilter(configuration.ComponentMask, _maxEntities));
            }
            return filter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EntityChanged(in Entity entity, in ComponentMask components)
        {
            foreach (var filter in _filters)
            {
                filter.OnEntityChanged(entity, components);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EntityDestroyed(in Entity entity)
        {
            foreach (var filter in _filters)
            {
                filter.OnEntityDestroyed(entity);
            }
        }

        public void Dispose()
        {
            foreach (var filter in _filters)
            {
                filter.Dispose();
            }
            _filters.Clear();
        }

        public void Update()
        {
            foreach (ref readonly var @event in _eventQueue.GetEvents())
            {
                if (@event.Type == EntityChangedEvent.Id)
                {
                    ref readonly var e = ref @event.As<EntityChangedEvent>();
                    EntityChanged(e.Entity, e.Components);
                }
                else if (@event.Type == EntityBeingDestroyedEvent.Id)
                {
                    EntityDestroyed(@event.As<EntityBeingDestroyedEvent>().Entity);
                }
            }
        }
    }
}
