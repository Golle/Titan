using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.Events;

namespace Titan.ECS;
public record struct ComponentBeingRemoved(ComponentId Id, uint Pool, Entity Entity) : IEvent;
public record struct ComponentAdded(ComponentId Id, uint Pool, Entity Entity) : IEvent;
public record struct EntityCreated(Entity Entity) : IEvent;
public record struct EntityAttached(Entity Parent, Entity Child) : IEvent;
public record struct EntityDetached(Entity Parent, Entity Child) : IEvent;
public record struct EntityBeingDestroyed(Entity Entity) : IEvent;
