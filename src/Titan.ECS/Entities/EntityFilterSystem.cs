using Titan.ECS.Events;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.ECS.Entities;

internal struct EntityFilterSystem : IStructSystem<EntityFilterSystem>
{
    private ApiResource<EntityFilterRegistry> Registry;
    private EventsReader<ComponentAdded> ComponenAdded;
    private EventsReader<ComponentBeingDestroyed> ComponenDestroyed;
    private EventsReader<EntityDestroyed> EntityDestroyed;
    private ReadOnlyResource<EntityInfoRegistry> EntityInfo;

    public static void Init(ref EntityFilterSystem system, in SystemsInitializer init)
    {
        system.Registry = init.GetApi<EntityFilterRegistry>();
        system.ComponenAdded = init.GetEventsReader<ComponentAdded>();
        system.ComponenDestroyed = init.GetEventsReader<ComponentBeingDestroyed>();
        system.EntityDestroyed = init.GetEventsReader<EntityDestroyed>();
        system.EntityInfo = init.GetReadOnlyResource<EntityInfoRegistry>();
    }

    public static void Update(ref EntityFilterSystem system)
    {
        ref var registry = ref system.Registry.Get();
        ref readonly var entityInfo = ref system.EntityInfo.Get();
        foreach (ref readonly var @event in system.ComponenAdded)
        {
            registry.EntityChanged(@event.Entity, entityInfo.Get(@event.Entity).Components);
        }

        foreach (ref readonly var @event in system.ComponenDestroyed)
        {
            registry.EntityChanged(@event.Entity, entityInfo.Get(@event.Entity).Components);
        }

        foreach (ref readonly var @event in system.EntityDestroyed)
        {
            registry.EntityDestroyed(@event.Entity);
        }
    }

    public static bool ShouldRun(in EntityFilterSystem system) => system.ComponenAdded.HasEvents() || system.ComponenDestroyed.HasEvents() || system.EntityDestroyed.HasEvents();
}
