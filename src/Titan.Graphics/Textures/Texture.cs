using System;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Textures
{
    public record Texture : IDisposable
    {
        public Texture2D Texture2D { get; init; }
        public ShaderResourceView ResourceView { get; init; }

        public void Dispose()
        {
            Texture2D.Dispose();
            ResourceView.Dispose();
        }
    }
}
