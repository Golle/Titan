using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Text;

namespace Titan.UI.Rendering
{

    internal struct TextBatchSprite
    {
        public Vector2 Position;
        public Handle<TextBlock> Handle;
        public Handle<Font> Font;
        public ushort Count;
    }
    internal class TextBatch : IDisposable
    {
        private readonly TextManager _textManager;
        private readonly FontManager _fontManager;
        private readonly MemoryChunk<TextBatchSprite> _textBatches;
        private int _count;

        public TextBatch(uint maxTextBlocks, TextManager textManager, FontManager fontManager)
        {
            _textManager = textManager;
            _fontManager = fontManager;
            _textBatches = MemoryUtils.AllocateBlock<TextBatchSprite>(maxTextBlocks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining|MethodImplOptions.AggressiveOptimization)]
        public unsafe int Add(in Vector2 position, in Handle<TextBlock> handle, in Handle<Font> font, ushort count)
        {
            var index = NextIndex();
            var text = _textBatches.GetPointer(index);
            text->Handle = handle;
            text->Font = font;
            text->Count = count;
            text->Position = position;

            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int NextIndex() => Interlocked.Increment(ref _count) - 1;


        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]

        public unsafe void Render(int index, ref UIVertex* vertex, ref uint vertexIndex, ref uint indexCount)
        {
            var batch = _textBatches.GetPointer(index);
            ref readonly var text = ref _textManager.Access(batch->Handle);
            ref readonly var positions = ref text.Positions;
            ref readonly var characters = ref text.Characters;
            ref readonly var font = ref _fontManager.Access(batch->Font);

            for (var i = 0; i < batch->Count; ++i)
            {
                var position = positions.GetPointer(i);
                ref readonly var glyph = ref font.Get(characters[i]);

                var bottomLeft = position->BottomLeft + batch->Position;
                var topRight = position->TopRight + batch->Position;

                vertex->Position = bottomLeft;
                vertex->Texture = new Vector2(glyph.TopLeft.X, glyph.BottomRight.Y);
                vertex->Color = Color.Black;
                vertex++;

                vertex->Position = new Vector2(bottomLeft.X, topRight.Y);
                vertex->Texture = glyph.TopLeft;
                vertex->Color = Color.Black;
                vertex++;

                vertex->Position = topRight;
                vertex->Texture = new Vector2(glyph.BottomRight.X, glyph.TopLeft.Y);
                vertex->Color = Color.Black;
                vertex++;

                vertex->Position = new Vector2(topRight.X, bottomLeft.Y);
                vertex->Texture = glyph.BottomRight;
                vertex->Color = Color.Black;
                vertex++;

                vertexIndex += 4;
                indexCount += 6;
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => _count = 0;

        public void Dispose()
        {
            _textBatches.Free();
        }
    }
}
