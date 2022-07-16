using System;
using System.Runtime.CompilerServices;
using Titan.Core;

namespace Titan.ECS.AnotherTry;

public readonly unsafe struct Events<T> where T : unmanaged, IEvent
{
    private readonly EventsRegistry.EventsInternal<T>* _events;
    internal Events(EventsRegistry.EventsInternal<T>* events) => _events = events;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> GetEvents() => _events->GetEvents();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Send(in T @event) => _events->Send(@event);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEvents() => _events->Count > 0;
}
