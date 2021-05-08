using System.Runtime.InteropServices;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct ComponentRemovedEvent
    {
        public readonly ComponentId Component;
        public readonly Entity Entity;
        public ComponentRemovedEvent(in Entity entity, in ComponentId component)
        {
            Component = component;
            Entity = entity;
        }
    }
}
