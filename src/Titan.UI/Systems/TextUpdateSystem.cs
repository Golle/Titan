using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core.Services;
using Titan.ECS.Systems;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Common;
using Titan.UI.Components;
using Titan.UI.Text;

namespace Titan.UI.Systems
{
    public unsafe class TextUpdateSystem : EntitySystem
    {
        private TextManager _textManager;
        private FontManager _fontManager;
        private EntityFilter _filter;
        private MutableStorage<TextComponent> _text;
        private ReadOnlyStorage<RectTransform> _transform;

        public TextUpdateSystem()
            : base(int.MinValue)
        {
        }

        protected override void Init(IServiceCollection services)
        {
            _filter = CreateFilter(new EntityFilterConfiguration().With<TextComponent>().With<RectTransform>());
            _text = GetMutable<TextComponent>();
            _transform = GetReadOnly<RectTransform>();

            _textManager = services.Get<TextManager>();
            _fontManager = services.Get<FontManager>();
        }

        protected override void OnUpdate(in Timestep timestep)
        {
            Span<TextLine> linesBuffer = stackalloc TextLine[100];
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

                var numberOfLines = CalculateLines(linesBuffer, new ReadOnlySpan<char>(textBlock.Characters, textBlock.CharacterCount), font, boxSize, multiplier, text.HorizontalOverflow);

                // TODO: handle numberof lines == 0

                var lines = linesBuffer[..numberOfLines];
                var yStartOffset = GetYOffset(boxSize, text, lines);
                var offset = new Vector2(0, yStartOffset);

                ushort characterCount = 0;
                foreach (ref readonly var line in lines)
                {
                    var shouldRender = (offset.Y + lineHeight <= boxSize.Height) || text.VerticalOverflow == VerticalOverflow.Overflow;
                    if (shouldRender)
                    {
                        offset.X = GetXOffset(line, boxSize, text.TextAlign, multiplier);
                        for (var i = line.Start; i <= line.End; ++i)
                        {
                            var c = textBlock.Characters[i];
                            ref readonly var glyph = ref font.Get(c);
                            var xAdvance = glyph.XAdvance * multiplier;
                            WriteGlyph(ref textBlock.VisibleCharacters[characterCount++], c, offset, glyph, font.Base, multiplier, xAdvance);
                            offset.X += xAdvance;
                        }
                    }
                    offset.Y -= lineHeight;
                    if (offset.Y < 0.0f && text.VerticalOverflow == VerticalOverflow.Truncate)
                    {
                        break;
                    }
                }

                text.CachedTexture = font.Texture;
                text.VisibleChars = characterCount;
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

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static float GetYOffset(in Size boxSize, in TextComponent text, in ReadOnlySpan<TextLine> lines) =>
                text.VerticalAlign switch
                {
                    VerticalAlign.Top => boxSize.Height - text.LineHeight,
                    VerticalAlign.Middle => boxSize.Height/2f + (lines.Length * text.LineHeight)/2f - text.LineHeight/2f,
                    VerticalAlign.Bottom or _ => text.LineHeight * (lines.Length - 1)
                };

            static float GetXOffset(in TextLine line, in Size boxSize, TextAlign align, float multiplier) =>
                align switch
                {
                    TextAlign.Left => 0f,
                    TextAlign.Center => (boxSize.Width/2f) - (line.Width * multiplier/2f),
                    TextAlign.Right or _ => boxSize.Width - line.Width * multiplier
                };
        }

        private static int CalculateLines(Span<TextLine> lines, ReadOnlySpan<char> characters, in Font font, in Size box, float multiplier, HorizontalOverflow overflow)
        {
            var multipliedBoxWidth = (int)(box.Width / multiplier + 0.5f); // Easier to multiply the box width than every glyph size
            var lineCount = 0;
            var currentWidth = 0;
            var widthBeforeLastWhitespace = 0;
            var startIndex = 0;
            var lastWhitespace = -1;
            for (var index = 0; index < characters.Length; ++index)
            {
                var c = characters[index];
                // Handle special characters
                if (c == '\r') // Ignore carriage return
                {
                    continue;
                }
                if (c == ' ')
                {
                    lastWhitespace = index;
                    // Store the current width since it might be needed if there's a line break
                    widthBeforeLastWhitespace = currentWidth;
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
                if (overflow == HorizontalOverflow.Wrap && width > multipliedBoxWidth && lastWhitespace >= 0)
                {
                    lines[lineCount++] = new TextLine
                    {
                        Start = (ushort)startIndex,
                        End = (ushort)(lastWhitespace - 1),
                        Width = widthBeforeLastWhitespace
                    };
                    // step back to the last whitespace found and set the startindex to the next character (TODO: what happens if the last character is a whitespace?)
                    index = lastWhitespace;
                    startIndex = lastWhitespace + 1;
                    // reset the last whitespace, so we don't step back to previous words
                    lastWhitespace = -1;
                    widthBeforeLastWhitespace = currentWidth = 0;
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

