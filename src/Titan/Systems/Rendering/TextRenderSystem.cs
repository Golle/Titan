using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Graphics.Rendering.Sprites;
using Titan.UI.Components;

namespace Titan.Systems
{
    public class TextRenderSystem : EntitySystem
    {
        private SpriteRenderQueue _renderQueue;
        private EntityFilter _filter;
        private ReadOnlyStorage<TextComponent> _text;
        private ReadOnlyStorage<RectTransform> _transform;
        

        protected override void Init(IServiceCollection services)
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<TextComponent>().With<RectTransform>());
            _text = GetReadOnly<TextComponent>();
            _transform = GetReadOnly<RectTransform>();

            _renderQueue = services.Get<SpriteRenderQueue>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var transform = ref _transform.Get(entity);
                ref readonly var text = ref _text.Get(entity);

                _renderQueue.AddText(transform.AbsolutePosition, transform.AbsoluteZIndex, text.CachedTexture, text.Font, text.Handle, text.VisibleChars, text.Color);
            }
        }
    }
}
