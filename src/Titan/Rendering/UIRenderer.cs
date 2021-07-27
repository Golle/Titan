using System;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Textures;
using Titan.Windows.D3D11;
using Buffer = Titan.Graphics.D3D11.Buffers.Buffer;

namespace Titan.Rendering
{


    //[StructLayout(LayoutKind.Explicit, Size = sizeof(uint) + sizeof(float) * 2 + sizeof(float) * 8)]
    //internal unsafe struct UIRenderable
    //{
    //    [FieldOffset(0)] 
    //    public uint TextureIndex;
    //    [FieldOffset(4)]
    //    public Vector2 TextureCoords;
    //    [FieldOffset(12)]
    //    private int _positions;

    //    public Vector2* Positions => ((Vector2*)Unsafe.AsPointer(ref _positions));
    //}


    internal unsafe class UIRenderQueue : IDisposable
    {
        public uint Count => (uint)(_count * 6) / 4;
        private MemoryChunk<UIVertex> _vertices;
        private int _count;
        private readonly Handle<Buffer> _vertexBuffer;
        private readonly Handle<Buffer> _indexBuffer;

        private uint _textureCount;
        private readonly Handle<Texture>[] _textures = new Handle<Texture>[16];

        public UIRenderQueue(uint maxSprites)
        {
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

            var maxVertices = maxSprites * 4;
            _vertices = MemoryUtils.AllocateBlock<UIVertex>(maxVertices);

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
        
        public void Add(in Vector2 position, in Vector2 size, in Handle<Texture> texture)
        {


            // TODO: put all vertex data in the VB
            // store the TextureHandle + count
            // sort by texture handle to reduce the amount of draw calls (best is 1 draw call)


            var textureIndex = Interlocked.Increment(ref _textureCount) - 1;
            _textures[textureIndex] = texture;

            var index = Interlocked.Add(ref _count, 4) - 4;

            var vertex = _vertices.GetPointer(index);

            vertex->TextureId = textureIndex;
            vertex->Position = new Vector2(-1, -1) / 2;
            vertex->Texture = new Vector2(0, 1);

            vertex++;
            vertex->TextureId = textureIndex;
            vertex->Position = new Vector2(-1, 1) / 2;
            vertex->Texture = new Vector2(0, 0);

            vertex++;
            vertex->TextureId = textureIndex;
            vertex->Position = new Vector2(1, 1) / 2;
            vertex->Texture = new Vector2(1, 0);

            vertex++;
            vertex->TextureId = textureIndex;
            vertex->Position = new Vector2(1, -1) / 2;
            vertex->Texture = new Vector2(1, 1);

            //vertices[0] = new FullscreenVertex { Position = new Vector2(-1, -1), Texture = new Vector2(0, 1) };
            //vertices[1] = new FullscreenVertex { Position = new Vector2(-1, 1), Texture = new Vector2(0, 0) };
            //vertices[2] = new FullscreenVertex { Position = new Vector2(1, 1), Texture = new Vector2(1, 0) };
            //vertices[3] = new FullscreenVertex { Position = new Vector2(1, -1), Texture = new Vector2(1, 1) };


            return;
            //vertex->TextureId = textureIndex;
            //vertex->Position = position;
            //vertex->Texture = new Vector2(0, 0);

            //vertex++;
            //vertex->TextureId = textureIndex;
            //vertex->Position = new Vector2(position.X + size.X, position.Y);
            //vertex->Texture = new Vector2(0, 1);

            //vertex++;
            //vertex->TextureId = textureIndex;
            //vertex->Position = new Vector2(position.X, position.Y + size.Y);
            //vertex->Texture = new Vector2(1, 0);

            //vertex++;
            //vertex->TextureId = textureIndex;
            //vertex->Position = new Vector2(position.X + size.X, position.Y + size.Y); ;
            //vertex->Texture = new Vector2(1, 1);
            
        }

        public void Update()
        {
            _count = 0;
            _textureCount = 0;
        }


        public Handle<Buffer> IndexBuffer => _indexBuffer;
        public Handle<Buffer> VertexBuffer => _vertexBuffer;

        public void Prepare(Context c) => c.Map(_vertexBuffer, _vertices.GetPointer(0), (uint)(_count*sizeof(UIVertex)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<Handle<Texture>> GetTextures() => new(_textures, 0, (int)_textureCount);
        public void Dispose()
        {
            GraphicsDevice.BufferManager.Release(_indexBuffer);
            GraphicsDevice.BufferManager.Release(_vertexBuffer);

            _vertices.Free();
            _vertices = default;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct UIVertex
    {
        public Vector2 Position;
        public Vector2 Texture;
        public uint TextureId;
    }

    
}
