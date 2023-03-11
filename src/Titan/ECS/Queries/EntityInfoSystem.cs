using System.Runtime.CompilerServices;
using Titan.ECS.Entities;
using Titan.Events;
using Titan.Systems;

namespace Titan.ECS.Queries;

internal struct EntityInfoSystem : ISystem
{
    private EventsReader<EntityBeingDestroyed> _entityDestroyed;
    private EventsReader<ComponentBeingRemoved> _componentRemoved;
    private EventsReader<ComponentAdded> _componentAdded;

    private MutableResource<EntityInfoRegistry> _entityInfo;

    public void Init(in SystemInitializer init)
    {
        _entityDestroyed = init.GetEventsReader<EntityBeingDestroyed>();
        _componentRemoved = init.GetEventsReader<ComponentBeingRemoved>();
        _componentAdded = init.GetEventsReader<ComponentAdded>();
        _entityInfo = init.GetMutableResource<EntityInfoRegistry>();
    }

    public void Update()
    {
        ref var info = ref _entityInfo.Get();
        foreach (ref readonly var @event in _componentAdded)
        {
            info.Get(@event.Entity).Components += @event.Id;
        }

        foreach (ref readonly var @event in _componentRemoved)
        {
            info.Get(@event.Entity).Components -= @event.Id;
        }
        foreach (ref readonly var @event in _entityDestroyed)
        {
            info.Get(@event.Entity) = default;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ShouldRun() => _entityDestroyed.HasEvents() || _componentAdded.HasEvents() || _componentRemoved.HasEvents();
}
;
