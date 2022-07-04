using Titan.Core.Events;

namespace Titan.ECS.Systems;

public readonly unsafe struct EventsWriter<T> where T : unmanaged
{
    private readonly EventCollection<T>* _resource;
    internal EventsWriter(EventCollection<T>* resource)
    {
        _resource = resource;
    }
    public void Send(in T @event) => _resource->Send(@event);
}
