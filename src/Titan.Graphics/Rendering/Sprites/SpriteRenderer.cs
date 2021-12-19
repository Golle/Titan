using Titan.Graphics.D3D11;
using Titan.Windows.D3D;

namespace Titan.Graphics.Rendering.Sprites
{
    public sealed class SpriteRenderer : Renderer
    {
        private readonly SpriteRenderQueue _renderQueue;
        public SpriteRenderer(SpriteRenderQueue renderQueue) => _renderQueue = renderQueue;

        public override void Render(Context context)
        {
            var renderables = _renderQueue.GetRenderables();

            context.SetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexBuffer(renderables.VertexBuffer);
            context.SetIndexBuffer(renderables.IndexBuffer);

            foreach (ref readonly var element in renderables.Elements)
            {
                context.SetPixelShaderResource(element.Texture);
                context.DrawIndexed(element.Count, element.StartIndex);
            }
        }
    }
}
