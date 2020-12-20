using Titan.Core.Messaging;

namespace Titan.ECS.Events
{
    [TitanEvent]
    internal readonly struct EntityDestroyedEvent
    {
        public static readonly short Id = EventId<EntityDestroyedEvent>.Value;
        public readonly uint EntityId; // not sure what to do here since the ID may be re-used before this event is processed.
        public EntityDestroyedEvent(uint id)
        {
            EntityId = id;
        }
    }
}
