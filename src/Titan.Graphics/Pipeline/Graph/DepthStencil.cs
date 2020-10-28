using System;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Pipeline.Graph
{
    public record DepthStencil(DepthStencilView View, ShaderResourceView ResourceView, Texture2D Resource) : IDisposable
    {
        public void Dispose()
        {
            View?.Dispose();
            ResourceView?.Dispose();
            Resource?.Dispose();
        }
    }
}
