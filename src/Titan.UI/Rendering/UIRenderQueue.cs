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
using Titan.UI.Common;
using Titan.Windows.D3D11;

namespace Titan.UI.Rendering
{
    public unsafe class UIRenderQueue : IDisposable
    {
        private readonly SpriteBatch _spriteBatch;
        private readonly NineSliceSpriteBatch _nineSliceSprite;

        private static readonly IComparer<SortableRenderable> Comparer = new UIComparer();
        private Handle<ResourceBuffer> _vertexBuffer;
        private Handle<ResourceBuffer> _indexBuffer;
        private readonly MemoryChunk<UIVertex> _vertices;
        private readonly UIElement[] _elements;
        private readonly SortableRenderable[] _sortable;
        private volatile int _count;

        private int _elementCount;

        public UIRenderQueue(uint maxSprites)
        {
            var maxVertices = maxSprites * 4;
            _spriteBatch = new SpriteBatch(maxSprites);
            _nineSliceSprite = new NineSliceSpriteBatch(maxSprites);

            _vertices = MemoryUtils.AllocateBlock<UIVertex>(maxVertices);
            _elements = new UIElement[maxSprites];
            _sortable = new SortableRenderable[maxSprites];
            static uint[] CreateIndices(uint maxSprites)
            {
                var indices = new uint[6 * maxSprites];
                var vertexIndex = 0u;
                for (var i = 0u; i < indices.Length; i += 6)
                {
                    indices[i] = vertexIndex;
                    indices[i + 1] = 1 + vertexIndex;
                    indices[i + 2] = 2 + vertexIndex;
                    indices[i + 3] = 0 + vertexIndex;
                    indices[i + 4] = 2 + vertexIndex;
                    indices[i + 5] = 3 + vertexIndex;
                    vertexIndex += 4;
                }
                return indices;
            }

            _vertexBuffer = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Count = maxVertices,
                Stride = (uint)sizeof(UIVertex),
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                Type = BufferTypes.VertexBuffer,
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            });

            var indices = CreateIndices(maxSprites);
            fixed (uint* pIndicies = indices)
            {
                _indexBuffer = GraphicsDevice.BufferManager.Create(new BufferCreation
                {
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                    Type = BufferTypes.IndexBuffer,
                    Count = (uint)indices.Length,
                    Stride = sizeof(uint),
                    InitialData = pIndicies,
                    Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
                });
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

        public void Begin()
        {
            _count = 0;
            _elementCount = 0;
            _spriteBatch.Clear();
            _nineSliceSprite.Clear();
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
                        // Noop
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
            _vertices.Free();
        }
    }
}
