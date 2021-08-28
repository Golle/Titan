using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Loaders.Atlas;
using Titan.Graphics.Loaders.Fonts;
using Titan.UI.Common;
using Titan.UI.Text;
using Titan.Windows.D3D11;

namespace Titan.UI.Rendering
{
    public record UIRenderQueueConfiguration(uint MaxSprites = 500, uint MaxNinePatchSprites = 100, uint MaxTextBlocks = 100);

    public unsafe class UIRenderQueue : IDisposable
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly NineSliceSpriteBatch _nineSliceSprite;
        private readonly TextBatch _textBatch;

        private static readonly IComparer<SortableRenderable> Comparer = new UIComparer();
        private Handle<ResourceBuffer> _vertexBuffer;
        private Handle<ResourceBuffer> _indexBuffer;
        private readonly MemoryChunk<UIVertex> _vertices;
        private readonly UIElement[] _elements;
        private readonly SortableRenderable[] _sortable;
        private volatile int _count;

        private int _elementCount;

        public UIRenderQueue(UIRenderQueueConfiguration config, TextManager textManager, FontManager fontManager)
        {
            var maxVertices = config.MaxSprites * 4 + config.MaxNinePatchSprites * 4 * 9 + config.MaxTextBlocks * 4 * 100; // 100 characters per block (TODO: change this at some point)
            var maxIndices = maxVertices * 6;

            _spriteBatch = new SpriteBatch(config.MaxSprites);
            _nineSliceSprite = new NineSliceSpriteBatch(config.MaxNinePatchSprites);
            _textBatch = new TextBatch(config.MaxTextBlocks, textManager, fontManager);

            _vertices = MemoryUtils.AllocateBlock<UIVertex>(maxVertices);
            _elements = new UIElement[100]; // TODO: Hardcoded for now, not sure what an optimal number would be. UIElements will be created for each texture change
            _sortable = new SortableRenderable[1_000]; // TODO: hardcoded for now. This is the amount of "items" added to the queue. 1 for each text, sprite or nine patch.  

            _vertexBuffer = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Count = maxVertices,
                Stride = (uint)sizeof(UIVertex),
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                Type = BufferTypes.VertexBuffer,
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            });

            var indices = maxIndices < 800_000 ? stackalloc uint[(int)maxIndices] : new uint[(int)maxIndices];
            InitInidices(indices);
            fixed (uint* pIndicies = indices)
            {
                _indexBuffer = GraphicsDevice.BufferManager.Create(new BufferCreation
                {
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                    Type = BufferTypes.IndexBuffer,
                    Count = maxIndices,
                    Stride = sizeof(uint),
                    InitialData = pIndicies,
                    Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
                });
            }

            static void InitInidices(Span<uint> indices)
            {
                var vertexIndex = 0u;
                for (var i = 0; i < indices.Length; i += 6)
                {
                    indices[i] = vertexIndex;
                    indices[i + 1] = 1 + vertexIndex;
                    indices[i + 2] = 2 + vertexIndex;
                    indices[i + 3] = 0 + vertexIndex;
                    indices[i + 4] = 2 + vertexIndex;
                    indices[i + 5] = 3 + vertexIndex;
                    vertexIndex += 4;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int NextIndex() => Interlocked.Increment(ref _count) - 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddNineSlice(in Vector2 position, int zIndex, in Size size, in Handle<Texture> texture, ReadOnlySpan<Vector2> coordinates, in Color color, in Margins margins)
        {
            var spriteIndex = _nineSliceSprite.AddNineSlice(position, size, coordinates, color, margins);
            _sortable[NextIndex()] = new SortableRenderable(zIndex, texture, spriteIndex, RenderableType.NineSlice);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in Vector2 position, int zIndex, in Size size, in Handle<Texture> texture, ReadOnlySpan<Vector2> coordinates, in Color color)
        {
            var spriteIndex = _spriteBatch.Add(position, size, color, coordinates);
            _sortable[NextIndex()] = new SortableRenderable(zIndex, texture, spriteIndex, RenderableType.Sprite);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddText(in Vector2 position, int zIndex, Handle<Texture> texture, Handle<Font> font, Handle<TextBlock> text, ushort start, ushort end, in Color color)
        {
            var spriteIndex = _textBatch.Add(position, text, font, start, end, color);
            _sortable[NextIndex()] = new SortableRenderable(zIndex, texture, spriteIndex, RenderableType.Text);
        }

        public void Begin()
        {
            _count = 0;
            _elementCount = 0;
            _spriteBatch.Clear();
            _nineSliceSprite.Clear();
            _textBatch.Clear();
        }

        public void End()
        {
            if (_count == 0)
            {
                return;
            }

            Array.Sort(_sortable, 0, _count, Comparer);
            var lastStartIndex = 0u;
            var indexCount = 0u;
            var vertexIndex = 0u;
            var texture = _sortable[0].Texture;
            for (var i = 0; i < _count; ++i)
            {
                ref readonly var renderable = ref _sortable[i];
                if (renderable.Texture.Value != texture.Value)
                {
                    // new renderable
                    _elements[_elementCount++] = new UIElement
                    {
                        Count = indexCount,
                        StartIndex = lastStartIndex,
                        Texture = texture
                    };
                    texture = renderable.Texture;
                    lastStartIndex += indexCount;
                    indexCount = 0;
                }

                var vertex = _vertices.GetPointer(vertexIndex);

                switch (renderable.Type)
                {
                    case RenderableType.Sprite:
                        _spriteBatch.Render(renderable.Index, ref vertex);
                        vertexIndex += 4; // TODO: where do these belong?
                        indexCount += 6;
                        break;
                    case RenderableType.NineSlice:
                        _nineSliceSprite.Render(renderable.Index, ref vertex);
                        vertexIndex += 4 * 9;
                        indexCount += 6 * 9;
                        break;
                    case RenderableType.Text:
                        _textBatch.Render(renderable.Index, ref vertex, ref vertexIndex, ref indexCount);
                        break;
                }
            }

            _elements[_elementCount++] = new UIElement
            {
                Count = indexCount,
                StartIndex = lastStartIndex,
                Texture = texture
            };
            GraphicsDevice.ImmediateContext.Map(_vertexBuffer, _vertices.GetPointer(0), (uint)(vertexIndex * sizeof(UIVertex)));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public UIRenderables GetRenderables() => new(_vertexBuffer, _indexBuffer, new ReadOnlySpan<UIElement>(_elements, 0, _elementCount));

        public void Dispose()
        {
            GraphicsDevice.BufferManager.Release(_vertexBuffer);;
            GraphicsDevice.BufferManager.Release(_indexBuffer);
            _vertexBuffer = _indexBuffer = 0;

            _nineSliceSprite.Dispose();
            _spriteBatch.Dispose();
            _textBatch.Dispose();
            _vertices.Free();

        }
    }
}
