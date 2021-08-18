using Titan.Assets;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Components;

namespace Titan.UI.Systems
{
    public class TextLoaderSystem : EntitySystem
    {
        private readonly AssetsManager _assetsManager;
        private EntityFilter _filter;
        private MutableStorage<AssetComponent<TextComponent>> _text;

        public TextLoaderSystem(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<AssetComponent<TextComponent>>().Not<TextComponent>());
            
            _text = GetMutable<AssetComponent<TextComponent>>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var font = ref _text.Get(entity);

                if (!font.AssetHandle.IsValid())
                {
                    font.AssetHandle = _assetsManager.Load(font.ToString());
                }

                if (_assetsManager.IsLoaded(font.AssetHandle))
                {
                    var component = font.DefaultValue;
                    component.Font = _assetsManager.GetAssetHandle<Font>(font.AssetHandle);
                    entity.AddComponent(component);
                }
            }
        }
    }
}
