using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Windows.D3D;

namespace Titan.UI.Rendering
{
    public sealed class UIRenderer : Renderer
    {
        private readonly UIRenderQueue _renderQueue;
        public UIRenderer(UIRenderQueue renderQueue) => _renderQueue = renderQueue;

        public override void Render(Context context)
        {
            var uiComponents = _renderQueue.GetRenderables();

            context.SetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexBuffer(uiComponents.VertexBuffer);
            context.SetIndexBuffer(uiComponents.IndexBuffer);

            foreach (ref readonly var element in uiComponents.Elements)
            {
                context.SetPixelShaderResource(element.Texture);
                context.DrawIndexed(element.Count, element.StartIndex);
            }
        }
    }
}
