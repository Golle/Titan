using Titan.Core.Messaging;
using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    public readonly struct EntityBeingDestroyedEvent
    {
        public static readonly short Id = EventId<EntityBeingDestroyedEvent>.Value;

        public readonly uint WorldId;
        public readonly uint EntityId;
        public EntityBeingDestroyedEvent(in Entity entity)
        {
            WorldId = entity.WorldId;
            EntityId = entity.Id;
        }
    }
}
