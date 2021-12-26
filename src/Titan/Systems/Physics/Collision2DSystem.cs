using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Components;
using Titan.Core.Logging;
using Titan.Core.Services;
using Titan.ECS.Entities;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Graphics;
using Titan.Physics;
using Titan.UI.Components;
using Titan.UI.Debugging;

namespace Titan.Systems.Physics;

internal class BoxCollision2DSystem : EntitySystem
{
    private EntityFilter _filter;
    private ReadOnlyStorage<RectTransform> _transform;
    private MutableStorage<BoxColliderComponent> _collider;
    
    private BoundingBoxRenderQueue _renderQueue;

    private Dictionary<uint, uint> _matrix;

    protected override void Init(IServiceCollection services)
    {
        _transform = GetReadOnly<RectTransform>();
        _collider = GetMutable<BoxColliderComponent>();

        _filter = CreateFilter(new EntityFilterConfiguration().With<RectTransform>().With<BoxColliderComponent>());

        _renderQueue = services.Get<BoundingBoxRenderQueue>();
        var config = services.Get<CollisionMatrixConfiguration>();
        if (config == null)
        {
            Logger.Warning<BoxCollision2DSystem>($"{nameof(BoxCollision2DSystem)} was registered but no CollisionMatrix was configured.");
        }
        else
        {
            _matrix = config.colliders;
        }
    }

    protected override void OnPreUpdate()
    {
        Span<Vector3> lines = stackalloc Vector3[4];
        foreach (ref readonly var entity in _filter.GetEntities())
        {
            ref readonly var transform = ref _transform.Get(entity);
            ref var collider = ref _collider.Get(entity);
         
            collider.BottomLeft = transform.AbsolutePosition + new Vector2(collider.Margins.Left, collider.Margins.Bottom);
            collider.TopRight = transform.AbsolutePosition + new Vector2(transform.Size.Width - collider.Margins.Right, transform.Size.Height - collider.Margins.Top);

#if DEBUG
            lines[0] = new Vector3(collider.BottomLeft, 1f);
            lines[1] = new Vector3(collider.BottomLeft.X, collider.TopRight.Y, 1f);
            lines[2] = new Vector3(collider.TopRight, 1f);
            lines[3] = new Vector3(collider.TopRight.X, collider.BottomLeft.Y, 1f);
            
            _renderQueue.Add(lines, Color.Magenta);
#endif
        }
    }

    protected override void OnUpdate(in Timestep timestep)
    {
        var entities = _filter.GetEntities();
        foreach (ref readonly var entity in entities)
        {
            ref var collider = ref _collider.Get(entity);
            foreach (ref readonly var innerEntity in entities)
            {
                if (entity.Id == innerEntity.Id)
                {
                    continue;
                }

                ref readonly var innerCollider = ref _collider.Get(innerEntity);
                if ((_matrix[collider.ColliderMask] & innerCollider.ColliderMask) == 0)
                {
                    continue;
                }

                var wasOverlapping = IsOverlapping(collider, innerEntity);
                var isOverlapping = Overlaps(collider, innerCollider);
                if (isOverlapping && !wasOverlapping)
                {
                    Logger.Error<BoxCollision2DSystem>($"OnEnter: Entity {entity.Id} overlaps with {innerEntity.Id}");
                    SetAsOverlapping(ref collider, innerEntity);
                }
                else if(!isOverlapping && wasOverlapping)
                {
                    Logger.Error<BoxCollision2DSystem>($"OnLeave: Entity {entity.Id} overlaps with {innerEntity.Id}");
                    UnSetAsOverlapping(ref collider, innerEntity);
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


        [MethodImpl(MethodImplOptions.AggressiveInlining|MethodImplOptions.AggressiveOptimization)]
        static unsafe bool IsOverlapping(in BoxColliderComponent box, in Entity entity)
        {
            for (var i = 0; i < box.OverlappingEntites; ++i)
            {
                if (box.Entities[i] == entity.Id)
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static unsafe void SetAsOverlapping(ref BoxColliderComponent box, in Entity entity)
        {
            if (box.OverlappingEntites >= BoxColliderComponent.MaxOverlappingEntities)
            {
                Logger.Warning<BoxCollision2DSystem>($"Max overlapping boxes { BoxColliderComponent.MaxOverlappingEntities} has been reached.");
            }
            else
            {
                box.Entities[box.OverlappingEntites++] = entity.Id;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static unsafe void UnSetAsOverlapping(ref BoxColliderComponent box, in Entity entity)
        {
            for (var i = 0; i < box.OverlappingEntites; ++i)
            {
                var id = box.Entities[i];
                if (id == entity.Id)
                {
                    box.OverlappingEntites--;
                    if (i != box.OverlappingEntites)
                    {
                        box.Entities[i] = box.Entities[box.OverlappingEntites];
                    }
                }
            }
        }
    }
}
