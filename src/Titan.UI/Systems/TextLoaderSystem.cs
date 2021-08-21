using Titan.Assets;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Components;
using Titan.UI.Text;

namespace Titan.UI.Systems
{
    public class TextLoaderSystem : EntitySystem
    {
        private readonly AssetsManager _assetsManager;
        private readonly TextManager _textManager;
        private EntityFilter _filter;
        private MutableStorage<AssetComponent<TextComponent>> _text;

        public TextLoaderSystem(AssetsManager assetsManager, TextManager textManager)
        {
            _assetsManager = assetsManager;
            _textManager = textManager;
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
                ref var text = ref _text.Get(entity);

                if (!text.AssetHandle.IsValid())
                {
                    text.AssetHandle = _assetsManager.Load(text.ToString());
                }

                if (_assetsManager.IsLoaded(text.AssetHandle))
                {
                    var component = text.DefaultValue;
                    var font = _assetsManager.GetAssetHandle<Font>(text.AssetHandle);
                    _textManager.SetFont(component.Text, font);
                    entity.AddComponent(component);
                }
            }
        }
    }
}
