using Titan.Core.Messaging;
using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    public readonly struct EntityDestroyedEvent
    {
        public static readonly short Id = EventId<EntityDestroyedEvent>.Value;
        public readonly Entity Entity;
        public EntityDestroyedEvent(in Entity entity) => Entity = entity;
    }
}
