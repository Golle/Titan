using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Extensions;
using Titan.Windows.D3D11;

namespace Titan.Rendering
{


    internal class GeometryRenderer : IRenderer
    {
        private readonly ViewPort _viewport;
        

        public GeometryRenderer()
        {
            _viewport = new ViewPort((int) GraphicsDevice.SwapChain.Width, (int) GraphicsDevice.SwapChain.Height);


        }


        public void Render(Context context)
        {
            //Logger.Warning<GeometryRenderer>("RENDER");
        }

        public void Dispose()
        {

        }
    }

    internal class BackbufferRenderer : IRenderer
    {
        private readonly Handle<Buffer> _vertexBuffer;
        private readonly Handle<Buffer> _indexBuffer;
        public BackbufferRenderer()
        {
            _vertexBuffer = GraphicsDevice.BufferManager.CreateFullscreenVertexBuffer();
            _indexBuffer = GraphicsDevice.BufferManager.CreateFullscreenIndexBuffer();
        }
        public void Render(Context context)
        {
            context.SetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexBuffer(_vertexBuffer);
            context.SetIndexBuffer(_indexBuffer);
            context.DrawIndexed(6);
        }

        public void Dispose()
        {
            GraphicsDevice.BufferManager.Release(_vertexBuffer);
            GraphicsDevice.BufferManager.Release(_indexBuffer);
        }
    }
}
