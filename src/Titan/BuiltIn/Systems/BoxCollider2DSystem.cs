using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Events;
using Titan.ECS.Entities;
using Titan.ECS.Queries;
using Titan.Events;
using Titan.Systems;

namespace Titan.BuiltIn.Systems;

//NOTE(Jens): The collider system send events, this could be an issue since the collision will happen on the next frame. If the "entity" is destroyed when a collision happens, it could still be active and another event is triggered before it's actually removed.
//NOTE(Jens): I think we want to move this to something else,maybe in Pre-Update ? and send anohter type of "event" that can be processed in the same frame it happens, similar to how we handle Audio.
internal struct BoxCollider2DSystem : ISystem
{
    private ReadOnlyStorage<Transform2D> _transform;
    private MutableStorage<BoxCollider2D> _boxCollider;
    private EventsWriter<Collision2DLeave> _leave;
    private EventsWriter<Collision2DEnter> _enter;
    private EntityQuery _entites;

    public void Init(in SystemInitializer init)
    {
        _transform = init.GetReadOnlyStorage<Transform2D>();
        _boxCollider = init.GetMutableStorage<BoxCollider2D>();

        _entites = init.CreateQuery(new EntityQueryArgs().With<Transform2D>().With<BoxCollider2D>());
        _leave = init.GetEventsWriter<Collision2DLeave>();
        _enter = init.GetEventsWriter<Collision2DEnter>();
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void Update()
    {
        foreach (ref readonly var entity in _entites)
        {
            ref readonly var transform = ref _transform[entity];
            ref var collider = ref _boxCollider[entity];

            var offset = collider.Pivot * (Vector2)collider.Size;

            collider.BottomLeft = transform.WorldPosition - offset * transform.Scale;
            collider.TopRight = collider.BottomLeft + transform.Scale * (Vector2)collider.Size;
        }

        foreach (ref readonly var outer in _entites)
        {
            foreach (ref readonly var inner in _entites)
            {
                if (outer.Id == inner.Id)
                {
                    continue;
                }

                ref var outerCollider = ref _boxCollider[outer];
                ref readonly var innerCollider = ref _boxCollider[inner];

                if (!outerCollider.CollidesWith.IsMatch(innerCollider.Category))
                {
                    continue;
                }

                var wasOverlapping = IsOverlapping(outerCollider, inner);
                var isOverlapping = Overlaps(outerCollider, innerCollider);
                if (isOverlapping && !wasOverlapping)
                {
                    SetAsOverlapping(ref outerCollider, inner);
                    _enter.Send(new Collision2DEnter(new(outer, outerCollider.Category), new(inner, innerCollider.Category)));
                }
                else if (!isOverlapping && wasOverlapping)
                {
                    RemoveFromOverlapping(ref outerCollider, inner);
                    _leave.Send(new Collision2DLeave(new(outer, outerCollider.Category), new(inner, innerCollider.Category)));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static unsafe void SetAsOverlapping(ref BoxCollider2D collider, in Entity entity)
        {
            Debug.Assert(collider.OverlapCount < BoxCollider2D.MaxOverlappingEntityCount);
            collider.Entities[collider.OverlapCount++] = entity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static unsafe void RemoveFromOverlapping(ref BoxCollider2D collider, in Entity entity)
        {
            for (var i = 0; i < collider.OverlapCount; i++)
            {
                if (collider.Entities[i] == entity)
                {
                    collider.Entities[i] = collider.Entities[collider.OverlapCount--];
                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static bool Overlaps(in BoxCollider2D lhs, in BoxCollider2D rhs)
        {
            if (lhs.BottomLeft.X > rhs.TopRight.X || lhs.TopRight.X < rhs.BottomLeft.X)
            {
                return false;
            }

            if (lhs.BottomLeft.Y > rhs.TopRight.Y || lhs.TopRight.Y < rhs.BottomLeft.Y)
            {
                return false;
            }
            return true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool IsOverlapping(in BoxCollider2D collider, in Entity entity)
    {
        foreach (var overlappingEntity in collider.GetOverlappingEntities())
        {
            if (overlappingEntity.Id == entity.Id)
            {
                return true;
            }
        }
        return false;
    }

    public bool ShouldRun() => _entites.Count >= 2;
}
