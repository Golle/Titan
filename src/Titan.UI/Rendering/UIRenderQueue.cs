using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Textures;
using Titan.UI.Common;
using Titan.Windows.D3D11;
using Buffer = Titan.Graphics.D3D11.Buffers.Buffer;

namespace Titan.UI.Rendering
{
    public unsafe class UIRenderQueue : IDisposable
    {
        private static readonly IComparer<SortableRenderable> Comparer = new UIComparer();
        private Handle<Buffer> _vertexBuffer;
        private Handle<Buffer> _indexBuffer;
        private readonly MemoryChunk<QueuedRenderable> _renderableQueue;
        private readonly MemoryChunk<UIVertex> _vertices;
        private readonly UIElement[] _elements;
        private readonly SortableRenderable[] _sortable;
        private int _count;

        private int _elementCount;

        public UIRenderQueue(uint maxSprites)
        {
            var maxVertices = maxSprites * 4;
            _renderableQueue = MemoryUtils.AllocateBlock<QueuedRenderable>(maxSprites);
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

        public void Add(in Vector2 position, int zIndex, in Size size, in Handle<Texture> texture)
        {
            var index = _count++;
            var renderable = _renderableQueue.GetPointer(index);
            renderable->Position = position;
            renderable->Texture = texture;
            renderable->Size = size;
            _sortable[index] = new SortableRenderable(zIndex, texture.Value, renderable); // Add 0.5f to Z to prevent floating position errors before casting it to an int. for example 5 could be 4.999998, and casting it to an int would return 4 instead of 5.
        }

        public void Begin()
        {
            _count = 0;
            _elementCount = 0;
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
            var texture = _sortable[0].Renderable->Texture;
            for (var i = 0; i < _count; ++i)
            {
                var renderable = _sortable[i].Renderable;

                if (renderable->Texture.Value != texture.Value)
                {
                    // new renderable
                    _elements[_elementCount++] = new UIElement
                    {
                        Count = indexCount,
                        StartIndex = lastStartIndex,
                        Texture = texture
                    };
                    texture = renderable->Texture;
                    lastStartIndex += indexCount;
                    indexCount = 0;
                }

                var vertex = _vertices.GetPointer(vertexIndex);
                
                var size = renderable->Size;
                var position = renderable->Position;

                var offsetY = position.Y + size.Height;
                var offsetX = position.X + size.Width;


                vertex->Position = position;
                vertex->Texture = new Vector2(0, 1);

                vertex++;
                vertex->Position = new Vector2(position.X, offsetY);
                vertex->Texture = new Vector2(0, 0);

                vertex++;
                vertex->Position = new Vector2(offsetX, offsetY);
                vertex->Texture = new Vector2(1, 0);

                vertex++;
                vertex->Position = new Vector2(offsetX, position.Y);
                vertex->Texture = new Vector2(1, 1);
                vertexIndex += 4;

                indexCount += 6;
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

            _renderableQueue.Free();
            _vertices.Free();
        }
    }
}
