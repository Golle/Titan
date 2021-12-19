using Titan.ECS.Entities;

namespace Titan.ECS.Events
{
    public readonly struct EntityDetachedEvent
    {
        public readonly Entity Parent;
        public readonly Entity Entity;

        public EntityDetachedEvent(in Entity parent, in Entity entity)
        {
            Parent = parent;
            Entity = entity;
        }
    }
}
