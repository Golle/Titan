using Titan.Core.Events;

namespace Titan.ECS.Systems;

public readonly unsafe struct EventsWriter<T> where T : unmanaged
{
    private readonly Events<T>* _resource;
    internal EventsWriter(Events<T>* resource)
    {
        _resource = resource;
    }
    public void Send(in T @event) => _resource->Send(@event);
}
