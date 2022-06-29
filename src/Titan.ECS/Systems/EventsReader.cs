using System;
using System.Runtime.CompilerServices;
using Titan.Core.Events;

namespace Titan.ECS.Systems;

public readonly unsafe struct EventsReader<T> where T : unmanaged
{
    private readonly Events<T>* _resource;
    internal EventsReader(Events<T>* resource)
    {
        _resource = resource;

    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ReadOnlySpan<T> GetEvents() => _resource->GetEvents();
}
