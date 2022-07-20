using Titan.Assets;
using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Graphics.Loaders.Models;
using Titan.Old.Components;

namespace Titan.Old.Systems.Loaders;

public class ModelLoaderSystem : EntitySystem
{
    private AssetsManager _assetsManager;
    private EntityFilter _filter;
    private MutableStorage<AssetComponent<Model>> _asset;
    private MutableStorage<ModelComponent> _model;

    protected override void Init(IServiceCollection services)
    {
        _filter = CreateFilter(new EntityFilterConfiguration().With<AssetComponent<Model>>().Not<ModelComponent>());
        _asset = GetMutable<AssetComponent<Model>>();
        _model = GetMutable<ModelComponent>();

        _assetsManager = services.Get<AssetsManager>();
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
                _model.Create(entity) = new ModelComponent
                {
                    Handle = assetHandle
                };
            }
        }
    }
}
