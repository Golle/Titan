using Titan.Components;
using Titan.Core.Services;
using Titan.ECS.Systems;

namespace Titan.Systems;

internal class Transform2DSystem : EntitySystem
{
    private MutableStorage<Transform2D> _transform;
    private EntityFilter _filter;
        
    protected override void Init(IServiceCollection services)
    {
        _transform = GetMutable<Transform2D>();
        _filter = CreateFilter(new EntityFilterConfiguration().With<Transform2D>());
    }

    protected override void OnUpdate(in Timestep timestep)
    {
        // TODO: implement sorting and IsDirty flag

        // TODO: pack them so that we can use intrinsics to calculate (SSE2)

        foreach (ref readonly var entity in _filter.GetEntities())
        {
            ref var transform = ref _transform.Get(entity);
            // TODO: add calculations for parent transforms
            if (EntityManager.TryGetParent(entity, out var parent))
            {
                //transform.WorldMatrix = transform.ModelMatrix * _transform.Get(parent).WorldMatrix;
            }
            else
            {
                //transform.WorldMatrix = transform.ModelMatrix;
            }
        }
    }
}
