using System.Runtime.InteropServices;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct ComponentAddedEvent
    {
        public readonly ComponentId Component;
        public readonly Entity Entity;
        public ComponentAddedEvent(in Entity entity, in ComponentId component)
        {
            Component = component;
            Entity = entity;
        }
    }
}
