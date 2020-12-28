using Titan.Core.Messaging;
using Titan.ECS.Entities;

namespace Titan.ECS.Messaging.Events
{
    [ECSEvent]
    internal readonly struct EntityBeingDestroyedEvent
    {
        public static readonly short Id = EventId<EntityBeingDestroyedEvent>.Value;
        public readonly Entity Entity;
        public EntityBeingDestroyedEvent(in Entity entity) => Entity = entity;
    }
}
