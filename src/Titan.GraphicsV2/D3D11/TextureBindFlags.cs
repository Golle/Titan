using System;

namespace Titan.GraphicsV2.D3D11
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
