using System;
using System.Net.Http;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Common;
using Titan.UI.Components;
using Titan.UI.Text;

namespace Titan.UI.Systems
{

   

    internal class TextUpdateSystem2: EntitySystem
    {
        private readonly TextManager _textManager;
        private readonly FontManager _fontManager;
        private EntityFilter _filter;
        private MutableStorage<TextComponent> _text;
        private ReadOnlyStorage<RectTransform> _transform;

        public TextUpdateSystem2(TextManager textManager, FontManager fontManager) 
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
            //Span<TextLine> s = stackalloc TextLine[10];
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var text = ref _text.Get(entity);
                if (!text.IsDirty)
                {
                    continue;
                }

                ref readonly var font = ref _fontManager.Access(text.Font);
                ref readonly var transform = ref _transform.Get(entity);
                ref var textBlock = ref _textManager.Access(text.Handle);
                var maxCharacters = textBlock.CharacterCount;
                var lineHeight = text.LineHeight;
                var fontSize = text.FontSize;
                var multiplier = fontSize / (float)font.FontSize;
                ref readonly var boxSize = ref transform.Size;
                var offset = new Vector2(0, boxSize.Height - lineHeight);

                for (var i = 0; i < maxCharacters; ++i)
                {
                    var c = textBlock.Characters[i];
                    ref readonly var glyph = ref font.Get(c);
                    var xAdvance = glyph.XAdvance * multiplier;
                    if (offset.X + xAdvance > boxSize.Width)
                    {
                        offset.Y -= lineHeight;
                        offset.X = 0f;
                    }

                    WriteGlyphPositions(ref textBlock.Positions[i], offset, glyph, font.Base, multiplier, xAdvance);
                    offset.X += xAdvance;
                }

                // get the length of characters


                //Split the text into lines that will fit horizontally
                //    Compute the vertical position of the first line
                //for each line
                //    Compute its width and the X position
                //    Display it at the Y position
                //    Add the line height to the current Y position



                text.CachedTexture = font.Texture;
                //text.VisibleChars = maxCharacters;
                text.IsDirty = false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void WriteGlyphPositions(ref CharacterPositions positions, in Vector2 offset, in Glyph glyph, int fontBase, float multiplier, float xAdvance)
            {
                var y = (fontBase - glyph.YOffset) * multiplier + offset.Y;  // TODO: move this to pre-processing so it's not needed
                positions.TopRight = new Vector2(offset.X + xAdvance, y);
                positions.BottomLeft = new Vector2(offset.X + glyph.XOffset * multiplier, y - glyph.Height * multiplier);
            }
        }
    }
}
