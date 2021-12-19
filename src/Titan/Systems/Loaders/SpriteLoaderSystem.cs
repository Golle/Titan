using Titan.Assets;
using Titan.Core.Logging;
using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Graphics.Loaders.Atlas;
using Titan.UI.Components;

namespace Titan.Systems
{
    public class SpriteLoaderSystem : EntitySystem
    {
        private AssetsManager _assetsManager;
        private EntityFilter _filter;
        private MutableStorage<AssetComponent<SpriteComponent>> _asset;
        private MutableStorage<SpriteComponent> _sprite;

        protected override void Init(IServiceCollection services)
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<AssetComponent<SpriteComponent>>().Not<SpriteComponent>());
            
            _asset = GetMutable<AssetComponent<SpriteComponent>>();
            _sprite = GetMutable<SpriteComponent>();

            _assetsManager = services.Get<AssetsManager>();

        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var sprite = ref _asset.Get(entity);

                if (!sprite.AssetHandle.IsValid())
                {
                    sprite.AssetHandle = _assetsManager.Load(sprite.ToString());
                }

                if (_assetsManager.IsLoaded(sprite.AssetHandle))
                {
                    var component = sprite.DefaultValue;
                    component.TextureAtlas = _assetsManager.GetAssetHandle<TextureAtlas>(sprite.AssetHandle);
                    _sprite.Create(entity) = component;
                }
            }
        }
    }
}
