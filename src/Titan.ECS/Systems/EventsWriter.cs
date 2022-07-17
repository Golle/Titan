using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Events;

namespace Titan.ECS.Systems;

public readonly struct EventsWriter<T> where T : unmanaged, IEvent
{
    private readonly Events<T> _events;
    internal EventsWriter(Events<T> events) => _events = events;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send(in T @event) => _events.Send(@event);
}
