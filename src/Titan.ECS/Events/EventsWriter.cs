using System.Runtime.CompilerServices;
using Titan.Core;

namespace Titan.ECS.Events;

public readonly unsafe struct EventsWriter<T> where T : unmanaged, IEvent
{
    private readonly EventsRegistry* _registry;
    internal EventsWriter(EventsRegistry* registry) => _registry = registry;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send(in T @event) => _registry->Send(@event);
}
