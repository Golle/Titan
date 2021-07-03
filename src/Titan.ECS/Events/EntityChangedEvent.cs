using System.Runtime.InteropServices;
using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public readonly struct EntityChangedEvent
    {
        public static readonly short Id = EventId<EntityChangedEvent>.Value;
        public readonly ComponentId Components;
        public readonly Entity Entity;
        public EntityChangedEvent(in Entity entity, in ComponentId components)
        {
            Components = components;
            Entity = entity;
        }
    }
}
