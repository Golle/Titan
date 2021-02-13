using System;

namespace Titan.GraphicsV2.Rendering
{
    internal class FrameBuffer : IDisposable
    {
        public FrameBufferTexture[] Textures { get; }
        public DepthStencil DepthStencil { get; }

        public FrameBuffer(FrameBufferTexture[] textures, DepthStencil depthStencil = null)
        {
            Textures = textures;
            DepthStencil = depthStencil;
        }

        public void Dispose()
        {
            foreach (var texture in Textures)
            {
                texture.Dispose();
            }
            DepthStencil?.Dispose();
        }
    }
}
