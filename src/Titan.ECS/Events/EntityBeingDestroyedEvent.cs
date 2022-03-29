using Titan.Core.Messaging;
using Titan.ECS.Entities;

namespace Titan.ECS.Events;

public readonly struct EntityBeingDestroyedEvent
{
    public static readonly short Id = EventId<EntityBeingDestroyedEvent>.Value;

    public readonly Entity Entity;
    public EntityBeingDestroyedEvent(in Entity entity) => Entity = entity;
}
