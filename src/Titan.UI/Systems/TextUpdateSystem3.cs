using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Common;
using Titan.UI.Components;
using Titan.UI.Text;

namespace Titan.UI.Systems
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct TextLine
    {
        public ushort Start;
        public ushort End;
        public float Width;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TextLine(int start, int end, float width)
        {
            Start = (ushort)start;
            End = (ushort)end;
            Width = width;
        }
    }

    internal unsafe class TextUpdateSystem3 : EntitySystem
    {
        private readonly TextManager _textManager;
        private readonly FontManager _fontManager;
        private EntityFilter _filter;
        private MutableStorage<TextComponent> _text;
        private ReadOnlyStorage<RectTransform> _transform;

        public TextUpdateSystem3(TextManager textManager, FontManager fontManager)
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
            Span<TextLine> lines = stackalloc TextLine[100];
            
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref var text = ref _text.Get(entity);
                if (!text.IsDirty)
                {
                    //continue;
                }

                ref readonly var font = ref _fontManager.Access(text.Font);
                ref readonly var transform = ref _transform.Get(entity);
                ref readonly var boxSize = ref transform.Size;
                ref var textBlock = ref _textManager.Access(text.Handle);
                var lineHeight = text.LineHeight;
                var multiplier = text.FontSize / (float)font.FontSize;

                var numberOfLines = CalculateLines(lines, new ReadOnlySpan<char>(textBlock.Characters, textBlock.CharacterCount), font, boxSize, multiplier);
                // TODO: handle numberof lines == 0

                var offset = new Vector2(0, boxSize.Height - lineHeight);
                var characterCount = 0;
                foreach (ref readonly var line in lines.Slice(0, numberOfLines))
                {
                    for (var i = line.Start; i <= line.End; ++i)
                    {
                        var c = textBlock.Characters[i];
                        ref readonly var glyph = ref font.Get(c);
                        var xAdvance = glyph.XAdvance * multiplier;
                        WriteGlyph(ref textBlock.VisibleCharacters[characterCount++],c, offset, glyph, font.Base, multiplier, xAdvance);
                        offset.X += xAdvance;
                    }
                    
                    offset.Y -= lineHeight;
                    offset.X = 0f;

                    if (offset.Y < 0.0f && text.VerticalOverflow == VerticalOverflow.Truncate)
                    {
                        break;
                    }
                }

                

                text.CachedTexture = font.Texture;
                //text.StartIndex = lines[0].Start;
                //text.EndIndex = lines[numberOfLines - 1].End;
                text.VisibleChars = (ushort)characterCount;
                text.IsDirty = false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static void WriteGlyph(ref CharacterPositions character, char c, in Vector2 offset, in Glyph glyph, int fontBase, float multiplier, float xAdvance)
            {
                var y = (fontBase - glyph.YOffset) * multiplier + offset.Y;  // TODO: move this to pre-processing so it's not needed
                character.Value = c;
                character.TopRight = new Vector2(offset.X + xAdvance, y);
                character.BottomLeft = new Vector2(offset.X + glyph.XOffset * multiplier, y - glyph.Height * multiplier);
            }
        }

        private static int CalculateLines(Span<TextLine> lines, ReadOnlySpan<char> characters, in Font font, in Size box, float multiplier)
        {
            var multipliedBoxWidth = (int)(box.Width / multiplier + 0.5f); // Easier to multiply the box width than every glyph size
            var lineCount = 0;
            var currentWidth = 0;
            var startIndex = 0;
            var lastWhitespace = -1;
            for (var index = 0; index < characters.Length; ++index)
            {
                var c = characters[index];
                // Handle special characters
                if (c == '\r') // Ignore carriage return
                {
                    // might cause issues, maybe this should be stripped when the string is created?
                    continue;
                }
                if (c == ' ')
                {
                    lastWhitespace = index;
                }
                else if (c == '\n')
                {
                    lines[lineCount++] = new TextLine(startIndex, index - 1, currentWidth);
                    startIndex = index + 1;
                    lastWhitespace = -1;
                    currentWidth = 0;
                    continue;
                }


                ref readonly var glyph = ref font.Get(c);
                // Calculate the new width and check if it's in the box, if it's not, create a new line and reset
                var width = currentWidth + glyph.XAdvance;
                if (width > multipliedBoxWidth && lastWhitespace >= 0)
                {
                    //var start = startIndex == 0 ? startIndex : startIndex + 1; // If the startindex is not the first character, increase the startindex with 1
                    lines[lineCount++] = new TextLine
                    {
                        Start = (ushort)startIndex,
                        End = (ushort)(lastWhitespace - 1),
                        Width = currentWidth
                    };
                    // step back to the last whitespace found and set the startindex to the next character (TODO: what happens if the last character is a whitespace?)
                    index = lastWhitespace;
                    startIndex = lastWhitespace + 1;
                    // reset the last whitespace, so we don't step back to previous words
                    lastWhitespace = -1;
                    currentWidth = 0;
                }
                else
                {
                    currentWidth = width;
                }
            }

            lines[lineCount++] = new TextLine
            {
                Start = (ushort)startIndex,
                End = (ushort)(characters.Length - 1),
                Width = currentWidth
            };
            return lineCount;
        }
    }
}

