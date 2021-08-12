using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Extensions;
using Titan.Windows.D3D11;

namespace Titan.Rendering
{
    internal sealed class BackbufferRenderer : Renderer
    {
        private readonly Handle<ResourceBuffer> _vertexBuffer;
        private readonly Handle<ResourceBuffer> _indexBuffer;
        public BackbufferRenderer()
        {
            _vertexBuffer = GraphicsDevice.BufferManager.CreateFullscreenVertexBuffer();
            _indexBuffer = GraphicsDevice.BufferManager.CreateFullscreenIndexBuffer();
        }

        public override void Render(Context context)
        {
            context.SetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

            context.SetVertexBuffer(_vertexBuffer);
            context.SetIndexBuffer(_indexBuffer);
            context.DrawIndexed(6);
        }

        public override void Dispose()
        {
            GraphicsDevice.BufferManager.Release(_vertexBuffer);
            GraphicsDevice.BufferManager.Release(_indexBuffer);
        }
    }
}
