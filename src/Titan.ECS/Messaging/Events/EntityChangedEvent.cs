using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Messaging.Events
{
    [ECSEvent]
    internal readonly struct EntityChangedEvent
    {
        public static readonly short Id = EventId<EntityChangedEvent>.Value;

        public readonly Entity Entity;
        public readonly ComponentId Components;
        public EntityChangedEvent(in Entity entity, in ComponentId components)
        {
            Entity = entity;
            Components = components;
        }
    }
}
