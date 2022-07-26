using Titan.Core.Logging;
using Titan.ECS.Events;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.ECS.Components;

internal struct ComponentSystem : IStructSystem<ComponentSystem>
{
    private ApiResource<ComponentRegistry> _registry;

    private EventsReader<ComponentBeingDestroyed> _componentBeingDestroyed;
    private EventsReader<EntityBeingDestroyed> _entityDestroyed;
    private EventsWriter<ComponentDestroyed> _componentDestroyed;

    public static void Init(ref ComponentSystem system, in SystemsInitializer init)
    {
        system._registry = init.GetApi<ComponentRegistry>();
        system._entityDestroyed = init.GetEventsReader<EntityBeingDestroyed>();
        system._componentBeingDestroyed = init.GetEventsReader<ComponentBeingDestroyed>();
        system._componentDestroyed = init.GetEventsWriter<ComponentDestroyed>();
    }

    public static void Update(ref ComponentSystem system)
    {
        ref var registry = ref system._registry.Get();
        foreach (ref readonly var @event in system._componentBeingDestroyed)
        {
            Logger.Trace<ComponentSystem>($"Component with ID: {@event.Id} and entity ID: {@event.Entity.Id} is being destroyed");
            registry.Destroy(@event.Entity, @event.Id);
            system._componentDestroyed.Send(new ComponentDestroyed(@event.Id, @event.Entity));
        }

        foreach (ref readonly var @event in system._entityDestroyed)
        {
            Logger.Trace<ComponentSystem>($"Entity with ID: {@event.Entity.Id} is being destroyed");
            registry.Destroy(@event.Entity);
        }
    }

    public static bool ShouldRun(in ComponentSystem system) => system._componentBeingDestroyed.HasEvents() || system._entityDestroyed.HasEvents();
}


