using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Graphics.D3D11.Buffers;

namespace Titan.UI.Rendering
{
    [SkipLocalsInit]
    public readonly ref struct UIRenderables
    {
        public readonly Handle<ResourceBuffer> VertexBuffer;
        public readonly Handle<ResourceBuffer> IndexBuffer;
        public readonly ReadOnlySpan<UIElement> Elements;

        public UIRenderables(Handle<ResourceBuffer> vertexBuffer, Handle<ResourceBuffer> indexBuffer, ReadOnlySpan<UIElement> elements)
        {
            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
            Elements = elements;
        }
    }
}