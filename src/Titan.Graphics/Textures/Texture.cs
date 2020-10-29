using System;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Textures
{
    public record Texture(Texture2D Texture2D, ShaderResourceView ResourceView) : IDisposable
    {
        public void Dispose()
        {
            Texture2D.Dispose();
            ResourceView.Dispose();
        }
    }
}
