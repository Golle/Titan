using System;
using System.Numerics;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Common;
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

            Span<int> lines = stackalloc int[100];

            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var text = ref _text.Get(entity);
                if (!text.IsDirty)
                {
                    continue;
                }

                ref readonly var font = ref _fontManager.Access(text.Font);
                ref readonly var transform = ref _transform.Get(entity);

                var width = transform.Size.Width;
                
                ref var textBlock = ref _textManager.Access(text.Handle);
                var maxCharacters = textBlock.CharacterCount;
                var xOffset = 0f;
                var lineHeight = text.LineHeight;
                var fontSize = text.FontSize;


                unsafe
                {
                    var words = GetWords(new ReadOnlySpan<char>(textBlock.Characters.AsPointer(), textBlock.CharacterCount), lines, transform.Size, font);
                }
                

                var aspectRatio = fontSize / (float)font.FontSize;
                
                for (var i = 0; i < maxCharacters; ++i)
                {
                    var character = textBlock.Characters[i];
                    ref var characterBlock = ref textBlock.VisibleCharacters[i];
                    ref readonly var glyph = ref font.Get(character);

                    var yOffset = (font.Base - glyph.YOffset);

                    characterBlock.BottomLeft = new Vector2(xOffset + glyph.XOffset, yOffset - glyph.Height) * aspectRatio;
                    characterBlock.TopRight = new Vector2(xOffset + glyph.XAdvance, yOffset) * aspectRatio;

                    xOffset += glyph.XAdvance;
                }

                var xAlignOffset = GetXOffset(text.TextAlign, textBlock.VisibleCharacters[0], textBlock.VisibleCharacters[maxCharacters - 1], width);

                if (xAlignOffset != 0.0f)
                {
                    // TODO: this can be done by calculating first + last before the loop, and then loop over characters between 1 and maxCharacters-1.
                    for (var i = 0; i < maxCharacters; ++i)
                    {
                        ref var characterBlock = ref textBlock.VisibleCharacters[i];
                        characterBlock.BottomLeft.X += xAlignOffset;
                        characterBlock.TopRight.X += xAlignOffset;
                    }
                }

                text.CachedTexture = font.Texture;
                //text.VisibleChars = maxCharacters;
                text.IsDirty = false;
            }



            static float GetXOffset(TextAlign align, in CharacterPositions first, in CharacterPositions last, int boxWidth) =>
                align switch
                {
                    TextAlign.Center => (boxWidth - (last.TopRight.X - first.BottomLeft.X)) / 2f,
                    TextAlign.Right => boxWidth - (last.TopRight.X - first.BottomLeft.X),
                    _ => 0 // Same as Left
                };



            static int GetWords(ReadOnlySpan<char> characters, Span<int> wordIndex, in Size boxSize, in Font font)
            {

                var lineHeight = 20;
                var count = 0;
                var xOffset = 0;
                var yOffset = 0;
                for (var i = 0; i < characters.Length; ++i)
                {
                    var character = characters[i];
                    ref readonly var glyph = ref font.Get(character);
                    
                    if (xOffset + glyph.XAdvance > boxSize.Width)
                    {
                        yOffset += lineHeight;
                    }

                    xOffset += glyph.XAdvance;
                }

                return count;
            }
        }
    }
}
