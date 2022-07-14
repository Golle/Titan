using Titan.Core;
using Titan.ECS.TheNew;

namespace Titan.ECS.AnotherTry;

internal struct EventsDescriptor
{
    public readonly EventId Id;
    public readonly uint Size;
    public readonly uint MaxEvents;

    private EventsDescriptor(EventId id, uint size, uint maxEvents)
    {
        Id = id;
        Size = size;
        MaxEvents = maxEvents;
    }

    public static unsafe EventsDescriptor Create<T>(uint maxEvents) where T : unmanaged, IEvent
    {
        var id = EventId.Id<T>();
        var size = sizeof(T);
        return new EventsDescriptor(id, (uint)size, maxEvents);
    }
}
