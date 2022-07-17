using Titan.Core.Messaging;
using Titan.ECS.Entities;

namespace Titan.ECS.Events;

public readonly struct EntityDestroyedEventOld
{
    public static readonly short Id = EventId<EntityDestroyedEventOld>.Value;
    public readonly Entity Entity;
    public EntityDestroyedEventOld(in Entity entity) => Entity = entity;
}
