using System.Threading;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    internal static class EventId
    {
        private static int _eventId;
        public static short NextId() => (short)Interlocked.Increment(ref _eventId);
    }
    internal static class EventId<T> where T : unmanaged
    {

        public static short Value { get; } = EventId.NextId();
    }


    [ECSEvent]
    internal readonly struct EntityCreatedEvent
    {
        public readonly Entity Entity;
        public EntityCreatedEvent(in Entity entity) => Entity = entity;
    }

    [ECSEvent]
    internal readonly struct EntityBeingDestroyedEvent
    {
        public readonly Entity Entity;
        public EntityBeingDestroyedEvent(in Entity entity) => Entity = entity;
    }

    [ECSEvent]
    internal readonly struct EntityDestroyedEvent
    {
        public readonly uint Id; // not sure what to do here since the ID may be re-used before this event is processed.
    }

    [ECSEvent]
    internal readonly struct ComponentAddedEvent
    {
        public readonly Entity Entity;
        public readonly ComponentId Component;
        public ComponentAddedEvent(in Entity entity, in ComponentId component)
        {
            Entity = entity;
            Component = component;
        }
    }

    [ECSEvent]
    readonly struct ComponentRemovedEvent
    {
    }
}
