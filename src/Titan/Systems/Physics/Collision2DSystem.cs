using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Graphics;
using Titan.UI.Components;
using Titan.UI.Debugging;

namespace Titan.Systems.Physics;

public unsafe struct BoxColliderComponent
{
    private const int MaxOverlappingEntities = 10;
    public bool IsTrigger;

    //private fixed Entity Entities[MaxOverlappingEntities];


    //internal Vector4 Box;

    internal Vector2 BottomLeft;
    internal Vector2 TopRight;
}

internal class BoxCollision2DSystem : EntitySystem
{
    private EntityFilter _filter;
    private ReadOnlyStorage<RectTransform> _transform;
    private MutableStorage<BoxColliderComponent> _collider;


    private BoundingBoxRenderQueue _renderQueue;

    protected override void Init(IServiceCollection services)
    {
        _transform = GetReadOnly<RectTransform>();
        _collider = GetMutable<BoxColliderComponent>();

        _filter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>().With<BoxColliderComponent>());

        _renderQueue = services.Get<BoundingBoxRenderQueue>();
    }

    protected override void OnPreUpdate()
    {
        foreach (ref readonly var entity in _filter.GetEntities())
        {
            ref readonly var transform = ref _transform.Get(entity);
            ref var collider = ref _collider.Get(entity);
         
            collider.BottomLeft = transform.AbsolutePosition;
            collider.TopRight = transform.AbsolutePosition + new Vector2(transform.Size.Width, transform.Size.Height);
        }


    }

    protected override void OnUpdate(in Timestep timestep)
    {
        Span<Vector3> lines = stackalloc Vector3[4];

        foreach (ref readonly var entity in _filter.GetEntities())
        {
            ref readonly var collider = ref _collider.Get(entity);

            lines[0] = new Vector3(collider.BottomLeft, 1f);
            lines[1] = new Vector3(collider.BottomLeft.X, collider.TopRight.Y, 1f);
            lines[2] = new Vector3(collider.TopRight, 1f);
            lines[3] = new Vector3(collider.TopRight.X, collider.BottomLeft.Y, 1f);
            _renderQueue.Add(lines, Color.Magenta);
            foreach (ref readonly var entityInner in _filter.GetEntities())
            {
                if (entityInner == entity) // Dont compare with itself
                {
                    continue;
                }

                ref readonly var colliderInner = ref _collider.Get(entity);

                if (Overlaps(collider, colliderInner))
                {
                    //Logger.Error<BoxCollision2DSystem>($"Entity {entity.Id} overlaps with {entityInner.Id}");
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool Overlaps(in BoxColliderComponent box1, in BoxColliderComponent box2)
        {
            if (box1.BottomLeft.X > box2.TopRight.X || box1.TopRight.X < box2.BottomLeft.X)
            {
                return false;
            }

            if (box1.BottomLeft.Y > box2.TopRight.Y || box1.TopRight.Y < box2.BottomLeft.Y)
            {
                return false;
            }
            return true;
        }
    }
}
