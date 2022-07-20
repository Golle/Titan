using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Systems;


public readonly record struct EntityCreated(Entity Entity) : IEvent;
public readonly record struct EntityBeingDestroyed(Entity Entity) : IEvent;
public readonly record struct EntityDestroyed(Entity Entity) : IEvent;
public readonly record struct ComponentBeingDestroyed(ComponentId Id, Entity Entity) : IEvent;
public readonly record struct ComponentDestroyed(ComponentId Id, Entity Entity) : IEvent;
public readonly record struct ComponentAdded(ComponentId Id, Entity Entity) : IEvent;
