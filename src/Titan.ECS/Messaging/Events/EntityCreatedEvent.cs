using Titan.Core.Messaging;
using Titan.ECS.Entities;

namespace Titan.ECS.Messaging.Events
{
    [ECSEvent]
    internal readonly struct EntityCreatedEvent
    {
        public static readonly short Id = EventId<EntityCreatedEvent>.Value;
        public readonly Entity Entity;
        public EntityCreatedEvent(in Entity entity) => Entity = entity;
    }
}
