using Titan.Core.Messaging;
using Titan.ECS.Entities;

namespace Titan.ECS.Messaging.Events
{
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
}
