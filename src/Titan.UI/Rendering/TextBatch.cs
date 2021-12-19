using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Text;

namespace Titan.UI.Rendering;

internal struct TextBatchSprite
{
    public Vector2 Position;
    public Handle<TextBlock> Handle;
    public Handle<Font> Font;
    public ushort Count;
    public Color Color;
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
    public unsafe int Add(in Vector2 position, in Handle<TextBlock> handle, in Handle<Font> font, ushort count, in Color color)
    {
        var index = NextIndex();
        var text = _textBatches.GetPointer(index);
        text->Handle = handle;
        text->Font = font;
        text->Count = count;
        text->Position = position;
        text->Color = color;

        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int NextIndex() => Interlocked.Increment(ref _count) - 1;


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]

    public unsafe void Render(int index, ref SpriteVertex* vertex, ref uint vertexIndex, ref uint indexCount)
    {
        var batch = _textBatches.GetPointer(index);
        ref readonly var text = ref _textManager.Access(batch->Handle);
        ref readonly var visibleCharacters = ref text.VisibleCharacters;
        ref readonly var font = ref _fontManager.Access(batch->Font);

        for (var i = 0; i < batch->Count; ++i)
        {
            var character = visibleCharacters.GetPointer(i);
            ref readonly var glyph = ref font.Get(character->Value);

            var bottomLeft = character->BottomLeft + batch->Position;
            var topRight = character->TopRight + batch->Position;

            vertex->Position = bottomLeft;
            vertex->Texture = new Vector2(glyph.TopLeft.X, glyph.BottomRight.Y);
            vertex->Color = batch->Color;
            vertex++;

            vertex->Position = new Vector2(bottomLeft.X, topRight.Y);
            vertex->Texture = glyph.TopLeft;
            vertex->Color = batch->Color;
            vertex++;

            vertex->Position = topRight;
            vertex->Texture = new Vector2(glyph.BottomRight.X, glyph.TopLeft.Y);
            vertex->Color = batch->Color;
            vertex++;

            vertex->Position = new Vector2(topRight.X, bottomLeft.Y);
            vertex->Texture = glyph.BottomRight;
            vertex->Color = batch->Color;
            vertex++;
        }

        vertexIndex += (uint)(4 * batch->Count);
        indexCount += (uint)(6 * batch->Count);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => _count = 0;

    public void Dispose()
    {
        _textBatches.Free();
    }
}
