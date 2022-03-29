using Titan.Core.Messaging;
using Titan.ECS.Entities;

namespace Titan.ECS.Events;

public readonly struct EntityCreatedEvent
{
    public readonly Entity Entity;
    public static readonly short Id = EventId<EntityCreatedEvent>.Value;
    public EntityCreatedEvent(in Entity entity)
    {
        Entity = entity;
    }
}
