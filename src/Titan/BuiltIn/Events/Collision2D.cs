using Titan.BuiltIn.Components;
using Titan.ECS.Entities;
using Titan.Events;

namespace Titan.BuiltIn.Events;

public record struct Collision2DEnter(Collision2D Source, Collision2D Target) : IEvent;
public record struct Collision2DLeave(Collision2D Source, Collision2D Target) : IEvent;
public record struct Collision2D(Entity Entity, CollisionMask Category);
