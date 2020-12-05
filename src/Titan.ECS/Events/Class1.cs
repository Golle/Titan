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
        public static readonly short Id = EventId<EntityCreatedEvent>.Value;
        public readonly Entity Entity;
        public EntityCreatedEvent(in Entity entity) => Entity = entity;
    }

    [ECSEvent]
    internal readonly struct EntityAttachedEvent
    {
        public static readonly short Id = EventId<EntityAttachedEvent>.Value;
        public readonly Entity Entity;
        public readonly Entity Parent;
        public EntityAttachedEvent(in Entity parent, in Entity entity)
        {
            Parent = parent;
            Entity = entity;
        }
    }
    
    [ECSEvent]
    internal readonly struct EntityDetachedEvent
    {
        public static readonly short Id = EventId<EntityDetachedEvent>.Value;
        public readonly Entity Entity;
        public readonly Entity Parent;
        public EntityDetachedEvent(in Entity parent, in Entity entity)
        {
            Parent = parent;
            Entity = entity;
        }
    }

    [ECSEvent]
    internal readonly struct EntityBeingDestroyedEvent
    {
        public static readonly short Id = EventId<EntityBeingDestroyedEvent>.Value;
        public readonly Entity Entity;
        public EntityBeingDestroyedEvent(in Entity entity) => Entity = entity;
    }

    [ECSEvent]
    internal readonly struct EntityDestroyedEvent
    {
        public static readonly short Id = EventId<EntityDestroyedEvent>.Value;
        public readonly uint EntityId; // not sure what to do here since the ID may be re-used before this event is processed.
        public EntityDestroyedEvent(uint id)
        {
            EntityId = id;
        }
    }

    [ECSEvent]
    internal readonly struct ComponentAddedEvent
    {
        public static readonly short Id = EventId<ComponentAddedEvent>.Value;

        public readonly Entity Entity;
        public readonly ComponentId Component;
        public ComponentAddedEvent(in Entity entity, in ComponentId component)
        {
            Entity = entity;
            Component = component;
        }
    }
    
    [ECSEvent]
    internal readonly struct EntityChangedEvent
    {
        public static readonly short Id = EventId<EntityChangedEvent>.Value;

        public readonly Entity Entity;
        public readonly ComponentMask Components;
        public EntityChangedEvent(in Entity entity, in ComponentMask components)
        {
            Entity = entity;
            Components = components;
        }
    }

    [ECSEvent]
    internal readonly struct ComponentRemovedEvent
    {
        public static readonly short Id = EventId<ComponentRemovedEvent>.Value;
        public readonly Entity Entity;
        public readonly ComponentId Component;
        public ComponentRemovedEvent(Entity entity, ComponentId component)
        {
            Entity = entity;
            Component = component;
        }
    }
}
