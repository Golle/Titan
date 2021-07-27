using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Buffer = Titan.Graphics.D3D11.Buffers.Buffer;

namespace Titan.UI.Rendering
{
    [SkipLocalsInit]
    public readonly ref struct UIRenderables
    {
        public readonly Handle<Buffer> VertexBuffer;
        public readonly Handle<Buffer> IndexBuffer;
        public readonly ReadOnlySpan<UIElement> Elements;

        public UIRenderables(Handle<Buffer> vertexBuffer, Handle<Buffer> indexBuffer, ReadOnlySpan<UIElement> elements)
        {
            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
            Elements = elements;
        }
    }
}