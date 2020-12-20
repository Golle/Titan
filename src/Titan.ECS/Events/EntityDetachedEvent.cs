using Titan.Core.Messaging;
using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    [TitanEvent]
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
}
