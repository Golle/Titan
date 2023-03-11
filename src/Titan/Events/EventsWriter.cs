using System.Runtime.CompilerServices;
using Titan.Core;

namespace Titan.Events;

public readonly struct EventsWriter<T> where T : unmanaged, IEvent
{
    private readonly ObjectHandle<EventsManager> _manager;
    internal EventsWriter(ObjectHandle<EventsManager> manager) => _manager = manager;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send(in T @event) => _manager.Value!.Send(@event);
}
