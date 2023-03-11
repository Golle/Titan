using Titan.Core;
using Titan.Events;
using Titan.Systems;

namespace Titan.ECS.Entities;

internal struct EntityLifetimeSystem : ISystem
{
    private EventsReader<EntityBeingDestroyed> _destroyed;
    private ObjectHandle<EntityRegistry> _registry;

    public void Init(in SystemInitializer init)
    {
        _destroyed = init.GetEventsReader<EntityBeingDestroyed>();
        _registry = init.GetManagedApi<EntityRegistry>();
    }

    public void Update()
    {
        var registry = _registry.Value;

        foreach (ref readonly var @event in _destroyed)
        {
            registry.Destroy(@event.Entity);
        }
    }
    public bool ShouldRun() => _destroyed.HasEvents();
}
