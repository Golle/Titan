using System;
using System.Numerics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Meshes;
using Titan.Graphics.Textures;

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

    public struct Renderable
    {
        public Texture Texture;
        public Mesh Mesh;
        public Matrix4x4 World;
    }

    public interface IRenderPass : IDisposable
    {

    }

}
