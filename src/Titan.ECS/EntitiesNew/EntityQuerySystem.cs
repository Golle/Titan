using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;

namespace Titan.ECS.EntitiesNew;

internal struct EntityQuerySystem : IStructSystem<EntityQuerySystem>
{
    private EventsReader<ComponentDestroyed> _componentDestroyed;
    private EventsReader<EntityDestroyed> _entityDestroyed;

    public static void Init(ref EntityQuerySystem system, in SystemsInitializer init)
    {
        system._componentDestroyed = init.GetEventsReader<ComponentDestroyed>();
        system._entityDestroyed = init.GetEventsReader<EntityDestroyed>();
    }

    public static void Update(ref EntityQuerySystem system)
    {
        foreach (ref readonly var @event in system._componentDestroyed.GetEvents())
        {
        }

        foreach (ref readonly var @event in system._entityDestroyed.GetEvents())
        {
        }
    }

    public static bool ShouldRun(in EntityQuerySystem system)
        => system._componentDestroyed.HasEvents()
           || system._entityDestroyed.HasEvents();
}