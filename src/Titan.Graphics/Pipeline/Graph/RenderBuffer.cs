using System;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Pipeline.Graph
{
    public record RenderBuffer(RenderTargetView RenderTargetView, ShaderResourceView ShaderResourceView, Texture2D Texture) : IDisposable
    {
        public void Dispose()
        {
            RenderTargetView?.Dispose();
            ShaderResourceView?.Dispose();
            Texture?.Dispose();
        }
    }
}
