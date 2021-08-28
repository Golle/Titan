using Titan.Assets;
using Titan.Components;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Models;

namespace Titan.Systems
{
    public class ModelLoaderSystem : EntitySystem
    {
        private readonly AssetsManager _assetsManager;
        private EntityFilter _filter;
        private MutableStorage<AssetComponent<Model>> _asset;

        public ModelLoaderSystem(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<AssetComponent<Model>>().Not<ModelComponent>());
            _asset = GetMutable<AssetComponent<Model>>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var asset = ref _asset.Get(entity);
                if (!asset.AssetHandle.IsValid())
                {
                    asset.AssetHandle = _assetsManager.Load(asset.ToString());
                }

                if (_assetsManager.IsLoaded(asset.AssetHandle))
                {
                    var assetHandle = _assetsManager.GetAssetHandle<Model>(asset.AssetHandle);
                    entity.AddComponent(new ModelComponent
                    {
                        Handle = assetHandle
                    });
                }
            }
        }
    }
}
