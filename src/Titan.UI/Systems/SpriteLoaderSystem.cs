using Titan.Assets;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Atlas;
using Titan.UI.Components;

namespace Titan.UI.Systems
{
    public class SpriteLoaderSystem : EntitySystem
    {
        private readonly AssetsManager _assetsManager;
        private EntityFilter _filter;
        private MutableStorage<AssetComponent<SpriteComponent>> _asset;
        private MutableStorage<SpriteComponent> _sprite;

        public SpriteLoaderSystem(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<AssetComponent<SpriteComponent>>().Not<SpriteComponent>());
            
            _asset = GetMutable<AssetComponent<SpriteComponent>>();
            _sprite = GetMutable<SpriteComponent>();

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
