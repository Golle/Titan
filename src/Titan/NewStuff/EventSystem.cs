using Titan.Core.Events;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;

namespace Titan.NewStuff;

public struct EventSystem<T> : IStructSystem<EventSystem<T>> where T : unmanaged
{
    private MutableResource<Events<T>> _events;
    public static void Init(ref EventSystem<T> system, ISystemsInitializer init) => system._events = init.GetMutableResource<Events<T>>();
    public static void Update(in EventSystem<T> system) => system._events.Get().Swap();
}
