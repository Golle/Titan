using System;

namespace Titan.Graphics.D3D11.Textures
{
    [Flags]
    internal enum TextureBindFlags
    {
        None = 0,
        ShaderResource = 1,
        RenderTarget = 2,
        DepthBuffer = 4
    }
}
