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
        private static readonly IComparer<SortableRenderable> Comparer = new UIComparer();
        private Handle<ResourceBuffer> _vertexBuffer;
        private Handle<ResourceBuffer> _indexBuffer;
        private readonly MemoryChunk<QueuedRenderable> _renderableQueue;
        private readonly MemoryChunk<UIVertex> _vertices;
        private readonly UIElement[] _elements;
        private readonly SortableRenderable[] _sortable;
        private volatile int _count;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int NextIndex() => Interlocked.Increment(ref _count) - 1;

        public void AddNineSlice(in Vector2 position, int zIndex, in Size size, in Handle<Texture> texture, ReadOnlySpan<Vector2> coordinates, in Color color, in Margins margins)
        {
            var index = NextIndex();
            var renderable = _renderableQueue.GetPointer(index);
            fixed (Vector2* pCoordinates = coordinates)
            {
                Buffer.MemoryCopy(pCoordinates, &renderable->Coordinates, 128, 128);
            }
            renderable->Position = position;
            renderable->Texture = texture;
            renderable->Size = size;
            renderable->Color = color;
            renderable->Margins = margins;
            renderable->Slice = true; 
            _sortable[index] = new SortableRenderable(zIndex, texture, renderable);
        }

        public void Add(in Vector2 position, int zIndex, in Size size, in Handle<Texture> texture, ReadOnlySpan<Vector2> coordinates, in Color color)
        {
            var index = NextIndex();
            var renderable = _renderableQueue.GetPointer(index);
            fixed (Vector2* pCoordinates = coordinates)
            {
                Buffer.MemoryCopy(pCoordinates, &renderable->Coordinates, 128, 4 * sizeof(Vector2));
            }
            renderable->Position = position;
            renderable->Texture = texture;
            renderable->Size = size;
            renderable->Color = color;
            renderable->Slice = false;
            _sortable[index] = new SortableRenderable(zIndex, texture, renderable);
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
                
                if (renderable->Slice)
                {
                    RenderSlice(vertex, renderable);
                    vertexIndex += 4*9;
                    indexCount += 6*9;
                }
                else
                {
                    RenderNormal(vertex, renderable);
                    vertexIndex += 4;
                    indexCount += 6;
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

        private static void RenderSlice(UIVertex* vertex, QueuedRenderable* renderable)
        {
             var size = renderable->Size;
            var position = renderable->Position;
            var margins = renderable->Margins;
            var color = renderable->Color;

            var positions = stackalloc Vector2[4];
            positions[0] = position;
            positions[1] = new Vector2(positions->X + margins.Left, positions->Y + margins.Bottom);
            positions[2] = new Vector2(positions->X + size.Width - margins.Right, positions->Y + size.Height - margins.Top);
            positions[3] = new Vector2(positions->X + size.Width, positions->Y + size.Height);

            // TODO: compare this with updating and uploading Indices. This creates 36 vertices, but only 16 are required. But if we use 16 we need to update the indices on each loop.
            for (var row = 0; row < 3; ++row)
            {
                for (var col = 0; col < 3; ++col)
                {
                    var textureOffset = row * 4 + col;
                    
                    vertex->Position = new Vector2(positions[col].X, positions[row].Y);
                    vertex->Texture = renderable->Coordinates[textureOffset];
                    vertex->Color = color;

                    vertex++;
                    vertex->Position = new Vector2(positions[col].X, positions[row + 1].Y);
                    vertex->Texture = renderable->Coordinates[textureOffset + 4];
                    vertex->Color = color;

                    vertex++;
                    vertex->Position = new Vector2(positions[col + 1].X, positions[row + 1].Y);
                    vertex->Texture = renderable->Coordinates[textureOffset + 5];
                    vertex->Color = color;

                    vertex++;
                    vertex->Position = new Vector2(positions[col + 1].X, positions[row].Y);
                    vertex->Texture = renderable->Coordinates[textureOffset + 1];
                    vertex->Color = color;
                    vertex++;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining|MethodImplOptions.AggressiveOptimization)]
        private static void RenderNormal(UIVertex* vertex, QueuedRenderable* renderable)
        {
            var size = renderable->Size;
            var position = renderable->Position;
            var top = position.Y + size.Height;
            var right = position.X + size.Width;

            vertex->Position = position;
            vertex->Texture = renderable->Coordinates[0];
            vertex->Color = renderable->Color;

            vertex++;
            vertex->Position = new Vector2(position.X, top);
            vertex->Texture = renderable->Coordinates[1];
            vertex->Color = renderable->Color;

            vertex++;
            vertex->Position = new Vector2(right, top);
            vertex->Texture = renderable->Coordinates[2];
            vertex->Color = renderable->Color;

            vertex++;
            vertex->Position = new Vector2(right, position.Y);
            vertex->Texture = renderable->Coordinates[3];
            vertex->Color = renderable->Color;
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
