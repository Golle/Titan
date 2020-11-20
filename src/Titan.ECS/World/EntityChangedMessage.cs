using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.World
{
    internal readonly struct EntityChangedMessage
    {
        public readonly Entity Entity;
        public readonly ComponentMask Components;
        public EntityChangedMessage(in Entity entity, in ComponentMask components)
        {
            Entity = entity;
            Components = components;
        }
    }
}
