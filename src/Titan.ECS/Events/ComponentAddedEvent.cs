using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    [TitanEvent]
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
}
