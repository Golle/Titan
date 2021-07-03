using System.ComponentModel;
using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    public readonly struct EntityAttachedEvent
    {
        public readonly Entity Parent;
        public readonly Entity Entity;
        public EntityAttachedEvent(in Entity parent, in Entity entity)
        {
            Parent = parent;
            Entity = entity;
        }
    }
}
