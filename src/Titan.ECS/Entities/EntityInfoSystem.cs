using Titan.Core.Logging;
using Titan.ECS.Events;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.ECS.Entities;

internal struct EntityInfoSystem : IStructSystem<EntityInfoSystem>
{
    private EventsReader<ComponentBeingDestroyed> Destroyed;
    private EventsReader<ComponentAdded> Added;
    private MutableResource<EntityInfoRegistry> EntityInfo;

    public static void Init(ref EntityInfoSystem system, in SystemsInitializer init)
    {
        system.Added = init.GetEventsReader<ComponentAdded>();
        system.Destroyed = init.GetEventsReader<ComponentBeingDestroyed>();
        system.EntityInfo = init.GetMutableResource<EntityInfoRegistry>();
    }

    public static void Update(ref EntityInfoSystem system)
    {
        ref var entityInfo = ref system.EntityInfo.Get();
        foreach (ref readonly var @event in system.Added)
        {
            entityInfo.Get(@event.Entity).Components += @event.Id;
            Logger.Trace<EntityInfoSystem>($"Component {@event.Id} added to Entity {@event.Entity.Id}");
        }
        foreach (ref readonly var @event in system.Destroyed)
        {
            entityInfo.Get(@event.Entity).Components -= @event.Id;
            Logger.Trace<EntityInfoSystem>($"Component {@event.Id} removed from Entity {@event.Entity.Id}");
        }
    }

    public static bool ShouldRun(in EntityInfoSystem system) => system.Added.HasEvents() || system.Destroyed.HasEvents();
}
