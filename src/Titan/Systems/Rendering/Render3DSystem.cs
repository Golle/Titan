using Titan.Components;
using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Graphics.Rendering.Geometry;

namespace Titan.Systems;

internal class Render3DSystem : EntitySystem
{
    private SimpleRenderQueue _queue;
    private EntityFilter _filter;
    private ReadOnlyStorage<Transform3D> _transform;
    private ReadOnlyStorage<ModelComponent> _model;

    protected override void Init(IServiceCollection services)
    {
        _transform = GetReadOnly<Transform3D>();
        _model = GetReadOnly<ModelComponent>();
        _filter = CreateFilter(new EntityFilterConfiguration().With<Transform3D>().With<ModelComponent>());
            
        _queue = services.Get<SimpleRenderQueue>();
    }

    protected override void OnUpdate(in Timestep timestep)
    {
        var entities = _filter.GetEntities();
        foreach (ref readonly var entity in entities)
        {
            ref readonly var transform = ref _transform.Get(entity);
            ref readonly var model = ref _model.Get(entity);

            _queue.Push(transform.WorldMatrix, model.Handle);
        }
    }
}
