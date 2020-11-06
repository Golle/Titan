using System.Numerics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Renderers
{
    internal unsafe class DefaultFullscreenRenderer : IRenderer
    {
        private readonly IShaderManager _shaderManager;
        private readonly IVertexBufferManager _vertexBufferManager;
        private readonly IndexBuffer<ushort> _indexBuffer;

        private readonly VertexBufferHandle _handle;
        public DefaultFullscreenRenderer(IGraphicsDevice device, IShaderManager shaderManager, IVertexBufferManager vertexBufferManager)
        {
            _shaderManager = shaderManager;
            _vertexBufferManager = vertexBufferManager;

            var vertices = stackalloc FullscreenVertex[4];
            vertices[0] = new FullscreenVertex {Position = new Vector2(-1, -1), UV = new Vector2(0, 1)};
            vertices[1] = new FullscreenVertex {Position = new Vector2(-1, 1), UV = new Vector2(0, 0)};
            vertices[2] = new FullscreenVertex {Position = new Vector2(1, 1), UV = new Vector2(1, 0)};
            vertices[3] = new FullscreenVertex {Position = new Vector2(1, -1), UV = new Vector2(1, 1)};
            _handle = _vertexBufferManager.CreateVertexBuffer(4u, (uint) sizeof(FullscreenVertex), vertices);

            _indexBuffer = new IndexBuffer<ushort>(device, new ushort[] { 0, 1, 2, 0, 2, 3});
        }

        public void Render(IRenderContext context)
        {
            context.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            
            context.SetIndexBuffer(_indexBuffer);
            context.SetVertexBuffer(_vertexBufferManager[_handle]);


            _shaderManager.Get(_shaderManager.GetHandle("FullscreenDefault")).Bind(context);

            context.DrawIndexed(6);
        }

        public void Dispose()
        {
            _vertexBufferManager.DestroyBuffer(_handle);
            _indexBuffer.Dispose();
        }

        private struct FullscreenVertex
        {
            public Vector2 Position;
            public Vector2 UV;
        }
    }
}
