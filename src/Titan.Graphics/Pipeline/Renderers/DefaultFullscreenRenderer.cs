using System.Numerics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Shaders;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Renderers
{
    internal class DefaultFullscreenRenderer : IRenderer
    {
        private readonly IShaderManager _shaderManager;
        private readonly VertexBuffer<FullscreenVertex> _vertexBuffer;
        private readonly IndexBuffer<ushort> _indexBuffer;

        public DefaultFullscreenRenderer(IGraphicsDevice device, IShaderManager shaderManager)
        {
            _shaderManager = shaderManager;
            _vertexBuffer = new VertexBuffer<FullscreenVertex>(device, new[]
            {
                new FullscreenVertex {Position = new Vector2(-1, -1), UV = new Vector2(0, 1)},
                new FullscreenVertex {Position = new Vector2(-1, 1), UV = new Vector2(0, 0)},
                new FullscreenVertex {Position = new Vector2(1, 1), UV = new Vector2(1, 0)},
                new FullscreenVertex {Position = new Vector2(1, -1), UV = new Vector2(1, 1)},
            });
            _indexBuffer = new IndexBuffer<ushort>(device, new ushort[] { 0, 1, 2, 0, 2, 3});
        }

        public void Render(IRenderContext context)
        {
            context.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexBuffer(_vertexBuffer);
            context.SetIndexBuffer(_indexBuffer);
            _shaderManager.Get(_shaderManager.GetHandle("FullscreenDefault")).Bind(context);

            context.DrawIndexed(6);
        }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }

        private struct FullscreenVertex
        {
            public Vector2 Position;
            public Vector2 UV;
        }
    }
}
