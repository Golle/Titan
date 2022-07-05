using Titan.ECS.Systems;

namespace Titan.ECS.SystemsV2.Components;

public struct ComponentSystem<T> : IStructSystem<ComponentSystem<T>> where T : unmanaged
{
    private EventsReader<EntityDestroyed> _entityDestroyed;
    private EventsReader<ComponentDestroyed> _componentDestroyed;
    private MutableStorage2<T> _components;

    // Init is called when the system is created, setting up the dependencies to helpt with the execution graph
    public static void Init(ref ComponentSystem<T> system, in SystemsInitializer init)
    {
        system._componentDestroyed = init.GetEventsReader<ComponentDestroyed>();
        system._entityDestroyed = init.GetEventsReader<EntityDestroyed>();
        system._components = init.GetMutableStorage<T>();
    }

    // ShouldRun will be called by the scheduler and if the system doesn't have to run it wont schedule it.
    static bool ShouldRun(in ComponentSystem<T> system) => system._componentDestroyed.HasEvents() || system._entityDestroyed.HasEvents();

    // Update is executed on the thread pool
    public static void Update(ref ComponentSystem<T> system)
    {
        foreach (ref readonly var @event in system._componentDestroyed.GetEvents())
        {
            system._components.DestroyImmediately(@event.Entity);
        }

        foreach (ref readonly var @event in system._entityDestroyed.GetEvents())
        {
            // NOTE(Jens): we can probably call destroy even if the entity does not exist in the component pool since it will be handled by the pool
            if (system._components.Contains(@event.Entity))
            {
                system._components.DestroyImmediately(@event.Entity);
            }
        }
    }
}



