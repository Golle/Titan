using Titan.Assets;
using Titan.Components;
using Titan.Core;
using Titan.ECS.Systems;
using Titan.Rendering;

namespace Titan.Systems
{
    internal class Render3DSystem : EntitySystem
    {
        private readonly AssetsManager _assetsManager;
        private readonly SimpleRenderQueue _queue;
        private EntityFilter _filter;
        private ReadOnlyStorage<Transform3D> _transform;
        private ReadOnlyStorage<ModelComponent> _model;

        public Render3DSystem(AssetsManager assetsManager, SimpleRenderQueue queue)
        {
            _assetsManager = assetsManager;
            _queue = queue;
        }

        protected override void Init()
        {
            _transform = GetReadOnly<Transform3D>();
            _model = GetReadOnly<ModelComponent>();
            _filter = CreateFilter(new EntityFilterConfiguration().With<Transform3D>().With<ModelComponent>());

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
}
