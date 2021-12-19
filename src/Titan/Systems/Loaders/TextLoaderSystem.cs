using Titan.Assets;
using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Components;

namespace Titan.Systems
{
    public class TextLoaderSystem : EntitySystem
    {
        private AssetsManager _assetsManager;
        private EntityFilter _filter;
        private MutableStorage<AssetComponent<TextComponent>> _asset;
        private MutableStorage<TextComponent> _text;

        protected override void Init(IServiceCollection services)
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<AssetComponent<TextComponent>>().Not<TextComponent>());
            
            _asset = GetMutable<AssetComponent<TextComponent>>();
            _text = GetMutable<TextComponent>();

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
                    var component = asset.DefaultValue;
                    component.Font = _assetsManager.GetAssetHandle<Font>(asset.AssetHandle);
                    component.IsDirty = true;
                    _text.Create(entity) = component;
                }
            }
        }
    }
}
