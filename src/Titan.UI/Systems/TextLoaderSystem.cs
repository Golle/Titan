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
        private MutableStorage<AssetComponent<TextComponent>> _asset;
        private MutableStorage<TextComponent> _text;

        public TextLoaderSystem(AssetsManager assetsManager, TextManager textManager)
        {
            _assetsManager = assetsManager;
            _textManager = textManager;
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<AssetComponent<TextComponent>>().Not<TextComponent>());
            
            _asset = GetMutable<AssetComponent<TextComponent>>();
            _text = GetMutable<TextComponent>();
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
