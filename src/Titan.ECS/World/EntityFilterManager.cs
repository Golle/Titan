using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Messaging;
using Titan.ECS.Messaging.Events;

namespace Titan.ECS.World
{
    internal class EntityFilterManager : IEntityFilterManager, IDisposable
    {
        private readonly IEventManager _eventManager;
        private readonly IList<EntityFilter> _filters = new List<EntityFilter>(100);

        private readonly uint _maxEntities;
        
        public EntityFilterManager(WorldConfiguration configuration, IEventManager eventManager)
        {
            _eventManager = eventManager;
            _maxEntities = configuration.MaxEntities;
        }

        public IEntityFilter Create(EntityFilterConfiguration configuration)
        {
            var filter = _filters.FirstOrDefault(f => f.Components == configuration.Components && f.Components == configuration.ExcludeComponents); // Re-use existing filters
            if (filter == null)
            {
                _filters.Add(filter = new EntityFilter(configuration.Components, configuration.ExcludeComponents, _maxEntities));
            }
            return filter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EntityChanged(in Entity entity, in ComponentId components)
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
            foreach (ref readonly var @event in _eventManager.GetEvents())
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
