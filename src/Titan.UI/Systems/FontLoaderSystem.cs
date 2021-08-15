using Titan.Assets;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Components;

namespace Titan.UI.Systems
{
    public class FontLoaderSystem : EntitySystem
    {
        private readonly AssetsManager _assetsManager;
        private EntityFilter _filter;
        private MutableStorage<AssetComponent<FontComponent>> _fonts;

        public FontLoaderSystem(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<AssetComponent<FontComponent>>().Not<FontComponent>());
            
            _fonts = GetMutable<AssetComponent<FontComponent>>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var font = ref _fonts.Get(entity);

                if (!font.AssetHandle.IsValid())
                {
                    font.AssetHandle = _assetsManager.Load(font.ToString());
                }

                if (_assetsManager.IsLoaded(font.AssetHandle))
                {
                    var component = font.DefaultValue;
                    component.Font= _assetsManager.GetAssetHandle<Font>(font.AssetHandle);
                    entity.AddComponent(component);
                }
            }
        }
    }
}
