using Titan.Core.Messaging;

namespace Titan.ECS.Events
{
    public readonly struct EntityDestroyedEvent
    {
        public static readonly short Id = EventId<EntityDestroyedEvent>.Value;

        public readonly uint WorldId;
        public readonly uint EntityId;
        public EntityDestroyedEvent(uint worldId, uint entityId)
        {
            WorldId = worldId;
            EntityId = entityId;
        }
    }
}
