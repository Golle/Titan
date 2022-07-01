using System;
using System.Runtime.CompilerServices;
using Titan.Core.Events;

namespace Titan.ECS.Systems;

public readonly unsafe struct EventsReader<T> where T : unmanaged
{
    private readonly Events<T>* _events;
    internal EventsReader(Events<T>* events)
    {
        _events = events;

    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<T> GetEvents() => _events->GetEvents();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasEvents() => _events->Count > 0;
}
