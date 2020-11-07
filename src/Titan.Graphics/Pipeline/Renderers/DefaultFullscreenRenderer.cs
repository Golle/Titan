using System.Numerics;
using Titan.Graphics.D3D11;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Renderers
{
    internal unsafe class DefaultFullscreenRenderer : IRenderer
    {
        private readonly IShaderManager _shaderManager;
        private readonly IVertexBufferManager _vertexBufferManager;
        private readonly IIndexBufferManager _indexBufferManager;

        private readonly VertexBufferHandle _vertexBufferHandle;
        private readonly IndexBufferHandle _indexBufferHandle;
        public DefaultFullscreenRenderer(IGraphicsDevice device, IShaderManager shaderManager)
        {
            _shaderManager = shaderManager;
            _vertexBufferManager = device.VertexBufferManager;
            _indexBufferManager = device.IndexBufferManager;

            var vertices = stackalloc FullscreenVertex[4];
            vertices[0] = new FullscreenVertex {Position = new Vector2(-1, -1), UV = new Vector2(0, 1)};
            vertices[1] = new FullscreenVertex {Position = new Vector2(-1, 1), UV = new Vector2(0, 0)};
            vertices[2] = new FullscreenVertex {Position = new Vector2(1, 1), UV = new Vector2(1, 0)};
            vertices[3] = new FullscreenVertex {Position = new Vector2(1, -1), UV = new Vector2(1, 1)};
            _vertexBufferHandle = _vertexBufferManager.CreateVertexBuffer(4u, (uint) sizeof(FullscreenVertex), vertices);

            var indices = stackalloc ushort[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;
            _indexBufferHandle = _indexBufferManager.CreateIndexBuffer<ushort>(6, indices);
        }

        public void Render(IRenderContext context)
        {
            context.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            
            context.SetIndexBuffer(_indexBufferManager[_indexBufferHandle]);
            context.SetVertexBuffer(_vertexBufferManager[_vertexBufferHandle]);

            _shaderManager.Get(_shaderManager.GetHandle("FullscreenDefault")).Bind(context);

            context.DrawIndexed(6);
        }

        public void Dispose()
        {
            _vertexBufferManager.DestroyBuffer(_vertexBufferHandle);
            _indexBufferManager.DestroyBuffer(_indexBufferHandle);
        }

        private struct FullscreenVertex
        {
            public Vector2 Position;
            public Vector2 UV;
        }
    }
}
