using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.ECS.Events;

public struct EventSystem : IStructSystem<EventSystem>
{
    private MutableResource<EventsRegistry> _registry;
    public static void Init(ref EventSystem system, in SystemsInitializer init) => system._registry = init.GetMutableResource<EventsRegistry>();
    public static void Update(ref EventSystem system) => system._registry.Get().Swap();
    public static bool ShouldRun(in EventSystem system) => true;
}
