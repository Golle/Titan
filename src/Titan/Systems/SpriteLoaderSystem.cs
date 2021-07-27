using Titan.Assets;
using Titan.Components;
using Titan.ECS.Systems;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Systems
{
    internal class SpriteLoaderSystem : EntitySystem
    {
        private readonly AssetsManager _assetsManager;
        private EntityFilter _filter;
        private MutableStorage<AssetComponent<Sprite>> _sprite;

        public SpriteLoaderSystem(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<AssetComponent<Sprite>>().Not<Sprite>());
            
            _sprite = GetMutable<AssetComponent<Sprite>>();
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
                    entity.AddComponent(new Sprite
                    {
                        Texture = _assetsManager.GetAssetHandle<Texture>(sprite.AssetHandle)
                    });
                }
            }
        }
    }
}
