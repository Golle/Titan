using System.Numerics;
using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Old.Components;

namespace Titan.Old.Systems.Transforms;

internal class Transform3DSystem : EntitySystem
{
    private MutableStorage<Transform3D> _transform;
    private EntityFilter _filter;

    protected override void Init(IServiceCollection services)
    {
        _transform = GetMutable<Transform3D>();
        _filter = CreateFilter(new EntityFilterConfiguration().With<Transform3D>());
    }

    protected override void OnUpdate(in Timestep timestep)
    {
        // TODO: implement sorting and IsDirty flag

        // TODO: pack them so that we can use intrinsics to calculate (SSE2)

        foreach (ref readonly var entity in _filter.GetEntities())
        {
            ref var transform = ref _transform.Get(entity);
            transform.ModelMatrix =
                Matrix4x4.CreateScale(transform.Scale) *
                Matrix4x4.CreateFromQuaternion(transform.Rotation) *
                Matrix4x4.CreateTranslation(transform.Position)
                ;

            if (EntityManager.TryGetParent(entity, out var parent))
            {
                transform.WorldMatrix = transform.ModelMatrix * _transform.Get(parent).WorldMatrix;
            }
            else
            {
                transform.WorldMatrix = transform.ModelMatrix;
            }
        }
    }
}
