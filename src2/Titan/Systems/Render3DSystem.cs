using Titan.Assets;
using Titan.Assets.Models;
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
        private Handle<Asset> _treeHandle;
        private bool _loaded;
        private Model _tree;

        public Render3DSystem(AssetsManager assetsManager, SimpleRenderQueue queue)
        {
            _assetsManager = assetsManager;
            _queue = queue;
        }

        protected override void Init()
        {
            _transform = GetReadOnly<Transform3D>();
            _treeHandle = _assetsManager.Load("models/tree");
            _filter = CreateFilter(new EntityFilterConfiguration().With<Transform3D>());

        }

        protected override void OnUpdate(in Timestep timestep)
        {
            if (_loaded)
            {
                foreach (ref readonly var entity in _filter.GetEntities())
                {
                    ref readonly var transform = ref _transform.Get(entity);

                    _queue.Push(transform.WorldMatrix, _tree);
                }
            }
            else
            {
                if (_loaded = _assetsManager.IsLoaded(_treeHandle))
                {
                    _tree = _assetsManager.GetAssetHandle<Model>(_treeHandle);
                }
            }
        }
    }
}
