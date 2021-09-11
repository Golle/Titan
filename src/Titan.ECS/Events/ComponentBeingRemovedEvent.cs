using System.Runtime.InteropServices;
using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct ComponentBeingRemovedEvent
    {
        public static readonly short Id = EventId<ComponentBeingRemovedEvent>.Value;
        public readonly ComponentId Component;
        public readonly Entity Entity;
        public ComponentBeingRemovedEvent(in Entity entity, in ComponentId component)
        {
            Component = component;
            Entity = entity;
        }
    }
}
