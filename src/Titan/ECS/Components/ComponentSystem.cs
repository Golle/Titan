using Titan.Core;
using Titan.Events;
using Titan.Systems;

namespace Titan.ECS.Components;


internal struct ComponentSystem : ISystem
{
    private ObjectHandle<ComponentsRegistry> _registry;
    private EventsReader<ComponentBeingRemoved> _componentRemoved;
    private EventsReader<EntityBeingDestroyed> _entityDestroyed;

    public void Init(in SystemInitializer init)
    {
        _registry = init.GetManagedApi<ComponentsRegistry>();
        _componentRemoved = init.GetEventsReader<ComponentBeingRemoved>();
        _entityDestroyed = init.GetEventsReader<EntityBeingDestroyed>();
    }

    public void Update()
    {
        var registry = _registry.Value;
        foreach (ref readonly var @event in _componentRemoved)
        {
            registry.Destroy(@event.Pool, @event.Entity);
        }

        foreach (ref readonly var @event in _entityDestroyed)
        {
            registry.DestroyEntity(@event.Entity);
        }
    }

    public bool ShouldRun() => _componentRemoved.HasEvents() || _entityDestroyed.HasEvents();
}


