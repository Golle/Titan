using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    [TitanEvent]
    internal readonly struct ComponentRemovedEvent
    {
        public static readonly short Id = EventId<ComponentRemovedEvent>.Value;
        public readonly Entity Entity;
        public readonly ComponentId Component;
        public ComponentRemovedEvent(Entity entity, ComponentId component)
        {
            Entity = entity;
            Component = component;
        }
    }
}
