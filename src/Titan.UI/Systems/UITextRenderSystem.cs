using System;
using System.Numerics;
using Titan.ECS.Systems;
using Titan.Graphics;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Components;
using Titan.UI.Rendering;

namespace Titan.UI.Systems
{
    public class UITextRenderSystem : EntitySystem
    {
        private readonly UIRenderQueue _renderQueue;
        private readonly FontManager _fontManager;
        private EntityFilter _filter;
        private ReadOnlyStorage<TextComponent> _text;
        private ReadOnlyStorage<RectTransform> _transform;

        public UITextRenderSystem(UIRenderQueue renderQueue, FontManager fontManager)
        {
            _renderQueue = renderQueue;
            _fontManager = fontManager;
        }
        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<TextComponent>().With<RectTransform>());
            _text = GetReadOnly<TextComponent>();
            _transform = GetReadOnly<RectTransform>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            const string Characters = "ABCDEFGH";
            Span<Vector2> coordinates1 = stackalloc Vector2[4];
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var transform = ref _transform.Get(entity);
                ref readonly var text = ref _text.Get(entity);
                ref readonly var font = ref _fontManager.Access(text.Font);
                var i = 0;

                foreach (var character in Characters)
                {
                    ref readonly var glyph = ref font.Get(character);

                    coordinates1[0] = new Vector2(glyph.TopLeft.X, glyph.BottomRight.Y);
                    coordinates1[1] = glyph.TopLeft;
                    coordinates1[2] = new Vector2(glyph.BottomRight.X, glyph.TopLeft.Y);
                    coordinates1[3] = glyph.BottomRight;

                    var color = Color.Magenta;
                    var position = new Vector2(transform.AbsolutePosition.X + i++ * transform.Size.Width, transform.AbsolutePosition.Y);
                    _renderQueue.Add(position, transform.AbsoluteZIndex, transform.Size, font.Texture, coordinates1, color);
                }


                //_renderQueue.AddText(text.Text, text.Font, text.Overflow, transform.AbsoluteZIndex, transform.Size, transform.)
            }
        }
    }
}
