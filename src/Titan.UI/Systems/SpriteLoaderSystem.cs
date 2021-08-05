using Titan.Assets;
using Titan.ECS.Systems;
using Titan.Graphics.D3D11.Textures;
using Titan.UI.Components;

namespace Titan.UI.Systems
{
    public class SpriteLoaderSystem : EntitySystem
    {
        private readonly AssetsManager _assetsManager;
        private EntityFilter _filter;
        private MutableStorage<AssetComponent<SpriteComponent>> _sprite;

        public SpriteLoaderSystem(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<AssetComponent<SpriteComponent>>().Not<SpriteComponent>());
            
            _sprite = GetMutable<AssetComponent<SpriteComponent>>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var sprite = ref _sprite.Get(entity);

                if (!sprite.AssetHandle.IsValid())
                {
                    sprite.AssetHandle = _assetsManager.Load(sprite.ToString());
                }

                if (_assetsManager.IsLoaded(sprite.AssetHandle))
                {
                    entity.AddComponent(new SpriteComponent
                    {
                        Texture = _assetsManager.GetAssetHandle<Texture>(sprite.AssetHandle)
                    });
                }
            }
        }
    }
}
