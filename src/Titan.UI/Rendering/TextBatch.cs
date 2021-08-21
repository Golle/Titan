using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics;
using Titan.Graphics.D3D11.Textures;
using Titan.UI.Text;

namespace Titan.UI.Rendering
{

    internal struct TextBatchSprite
    {
        public Vector2 Position;
        public Handle<TextBlock> Handle;
    }
    internal class TextBatch : IDisposable
    {
        private readonly TextManager _textManager;
        private readonly MemoryChunk<TextBatchSprite> _textBatches;
        private int _count;

        public TextBatch(uint maxTextBlocks, TextManager textManager)
        {
            _textManager = textManager;
            _textBatches = MemoryUtils.AllocateBlock<TextBatchSprite>(maxTextBlocks);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining|MethodImplOptions.AggressiveOptimization)]
        public unsafe (int index, Handle<Texture> texture) Add(in Vector2 position, in Handle<TextBlock> handle)
        {
            var index = NextIndex();
            var text = _textBatches.GetPointer(index);
            text->Handle = handle;
            text->Position = position;

            var texture = _textManager.FontManager.Access(_textManager.Access(handle).Font).Texture;

            return (index, texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int NextIndex() => Interlocked.Increment(ref _count) - 1;


        [MethodImpl(MethodImplOptions.AggressiveInlining|MethodImplOptions.AggressiveOptimization)]

        public unsafe void Render(int index, ref UIVertex* vertex, ref uint vertexIndex, ref uint indexCount)
        {
            var batch = _textBatches.GetPointer(index);
            ref readonly var text = ref _textManager.Access(batch->Handle);
            ref readonly var positions = ref text.Positions;
            ref readonly var characters = ref text.Characters;
            ref readonly var font = ref _textManager.FontManager.Access(text.Font);

            for (var i = 0; i < text.VisibleChars; ++i)
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
                //coordinates[0] = new Vector2(x / textureWidth, (y + height) / textureHeight);
                //coordinates[1] = new Vector2(x / textureWidth, y / textureHeight);
                //coordinates[2] = new Vector2((x + width) / textureWidth, y / textureHeight);
                //coordinates[3] = new Vector2((x + width) / textureWidth, (y + height) / textureHeight);
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
