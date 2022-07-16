using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.AnotherTry;

namespace Titan.ECS.Systems;

public readonly struct EventsReader<T> where T : unmanaged, IEvent
{
    private readonly Events<T> _events;
    internal EventsReader(Events<T> events) => _events = events;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<T> GetEvents() => _events.GetEvents();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEvents() => _events.HasEvents();
}
