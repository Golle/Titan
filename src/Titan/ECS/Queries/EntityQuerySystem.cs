using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Entities;
using Titan.Events;
using Titan.Systems;

namespace Titan.ECS.Queries;

internal struct EntityQuerySystem : ISystem
{
    private EventsReader<EntityBeingDestroyed> _entityDestroyed;
    private EventsReader<ComponentBeingRemoved> _componentRemoved;
    private EventsReader<ComponentAdded> _componentAdded;
    private ObjectHandle<EntityQueryRegistry> _entityQuery;

    private ReadOnlyResource<EntityInfoRegistry> _entityInfo;
    public void Init(in SystemInitializer init)
    {
        _entityDestroyed = init.GetEventsReader<EntityBeingDestroyed>();
        _componentRemoved = init.GetEventsReader<ComponentBeingRemoved>();
        _componentAdded = init.GetEventsReader<ComponentAdded>();
        _entityInfo = init.GetReadOnlyResource<EntityInfoRegistry>();

        _entityQuery = init.GetManagedApi<EntityQueryRegistry>();
    }

    public void Update()
    {
        ref readonly var info = ref _entityInfo.Get();
        var queries = _entityQuery.Value;
        foreach (ref readonly var @event in _componentAdded)
        {
            queries.EntityChanged(@event.Entity, info.Get(@event.Entity).Components);
        }

        foreach (ref readonly var @event in _componentRemoved)
        {
            queries.EntityChanged(@event.Entity, info.Get(@event.Entity).Components);
        }
        foreach (ref readonly var @event in _entityDestroyed)
        {
            queries.EntityDestroyed(@event.Entity);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ShouldRun() => _entityDestroyed.HasEvents() || _componentAdded.HasEvents() || _componentRemoved.HasEvents();
}
