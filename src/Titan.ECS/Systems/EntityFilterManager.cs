using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Events;
using Titan.ECS.Worlds;

namespace Titan.ECS.Systems;

internal class EntityFilterManager : IDisposable
{
    private readonly EntityInfoManager _entityInfo;
    private readonly List<EntityFilter> _filters = new (100);

    private readonly uint _maxEntities;
    private readonly uint _worldId;

    public EntityFilterManager(WorldConfigurationOld configuration, EntityInfoManager entityInfo)
    {
        _entityInfo = entityInfo;
        _maxEntities = configuration.MaxEntities;
        _worldId = configuration.Id;
    }

    public EntityFilter Create(EntityFilterConfiguration configuration)
    {
        var filter = _filters.FirstOrDefault(f => f.Components == configuration.Components && f.Exclude == configuration.ExcludeComponents); // Re-use existing filters
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
            filter.OnEntityDestroyed(entity.Id);
        }
    }

    public void Update()
    {
        foreach (ref readonly var @event in EventManager.GetEvents())
        {
            if (@event.Type == ComponentAddedEvent.Id)
            {
                ref readonly var componentEvent = ref @event.As<ComponentAddedEvent>();
                if (componentEvent.Entity.WorldId == _worldId)
                {
                    EntityChanged(componentEvent.Entity, _entityInfo[componentEvent.Entity].Components);
                }
            }
            else if (@event.Type == ComponentBeingRemovedEvent.Id)
            {
                ref readonly var componentEvent = ref @event.As<ComponentBeingRemovedEvent>();
                if (componentEvent.Entity.WorldId == _worldId)
                {
                    EntityChanged(componentEvent.Entity, _entityInfo[componentEvent.Entity].Components);
                }
            }
            else if (@event.Type == EntityBeingDestroyedEvent.Id)
            {
                ref readonly var e = ref @event.As<EntityBeingDestroyedEvent>();
                if (e.Entity.WorldId == _worldId)
                {
                    EntityDestroyed(e.Entity);
                }
            }
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
}
