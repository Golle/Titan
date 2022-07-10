using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.SystemsV2;

public readonly record struct EntityDestroyed(Entity Entity) : IEvent;
public readonly record struct ComponentDestroyed(ComponentId Id, Entity Entity) : IEvent;
