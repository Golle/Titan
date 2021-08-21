using System;
using System.Numerics;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Components;
using Titan.UI.Text;

namespace Titan.UI.Systems
{
    internal class TextUpdateSystem : EntitySystem
    {
        private readonly TextManager _textManager;
        private readonly FontManager _fontManager;
        private EntityFilter _filter;
        private MutableStorage<TextComponent> _text;
        private ReadOnlyStorage<RectTransform> _transform;

        public TextUpdateSystem(TextManager textManager, FontManager fontManager) 
            : base(int.MinValue)
        {
            _textManager = textManager;
            _fontManager = fontManager;
        }

        protected override void Init()
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<TextComponent>().With<RectTransform>());
            _text = GetMutable<TextComponent>();
            _transform = GetReadOnly<RectTransform>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var text = ref _text.Get(entity);
                if (!text.IsDirty)
                {
                    continue;
                }

                ref readonly var font = ref _fontManager.Access(text.Font);
                ref var textBlock = ref _textManager.Access(text.Handle);

                var maxCharacters = textBlock.CharacterCount;
                var xOffset = 0;
                var lineHeight = text.LineHeight;

                for (var i = 0; i < maxCharacters; ++i)
                {
                    var character = textBlock.Characters[i];
                    ref var characterBlock = ref textBlock.Positions[i];
                    ref readonly var glyph = ref font.Get(character);
                    characterBlock.BottomLeft = new Vector2(xOffset + glyph.XOffset, lineHeight - glyph.YOffset - glyph.Height);
                    characterBlock.TopRight = new Vector2(xOffset + glyph.XAdvance, lineHeight - glyph.YOffset);
                    xOffset += glyph.XAdvance;
                }

                text.CachedTexture = font.Texture;
                text.VisibleChars = maxCharacters;
                text.IsDirty = false;
            }
        }
    }
}
