using Titan.Core.Events;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;

namespace Titan.NewStuff;

public struct EventSystem<T> : IStructSystem<EventSystem<T>> where T : unmanaged
{
    private MutableResource<EventCollection<T>> _events;
    public static void Init(ref EventSystem<T> system, in SystemsInitializer init) => system._events = init.GetMutableResource<EventCollection<T>>();
    public static void Update(ref EventSystem<T> system) => system._events.Get().Swap();
}
