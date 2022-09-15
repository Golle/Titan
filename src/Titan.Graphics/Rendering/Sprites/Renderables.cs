using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Graphics.D3D11.Buffers;

namespace Titan.Graphics.Rendering.Sprites
{
    [SkipLocalsInit]
    public readonly ref struct Renderables
    {
        public readonly Handle<ResourceBuffer> VertexBuffer;
        public readonly Handle<ResourceBuffer> IndexBuffer;
        public readonly ReadOnlySpan<SpriteElement> Elements;

        public Renderables(Handle<ResourceBuffer> vertexBuffer, Handle<ResourceBuffer> indexBuffer, ReadOnlySpan<SpriteElement> elements)
        {
            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
            Elements = elements;
        }
    }
}
