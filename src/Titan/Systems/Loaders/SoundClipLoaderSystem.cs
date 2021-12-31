using Titan.Assets;
using Titan.Components;
using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Sound.Loaders;

namespace Titan.Systems.Loaders;

internal class SoundClipLoaderSystem : EntitySystem
{
    private EntityFilter _filter;
    private AssetsManager _assetsManager;
    private MutableStorage<AssetComponent<SoundClipComponent>> _asset;
    private MutableStorage<SoundClipComponent> _soundClip;

    protected override void Init(IServiceCollection services)
    {
        _filter = CreateFilter(new EntityFilterConfiguration().With<AssetComponent<SoundClipComponent>>().Not<SoundClipComponent>());

        _asset = GetMutable<AssetComponent<SoundClipComponent>>();
        _soundClip = GetMutable<SoundClipComponent>();

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
                var assetHandle = _assetsManager.GetAssetHandle<SoundClip>(asset.AssetHandle);
                
                ref var soundComponent = ref _soundClip.Create(entity, asset.DefaultValue);
                soundComponent.Handle = assetHandle;
            }
        }
    }
}
