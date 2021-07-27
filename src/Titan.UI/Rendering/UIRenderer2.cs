using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Windows.D3D11;
using Buffer = Titan.Graphics.D3D11.Buffers.Buffer;

namespace Titan.UI.Rendering
{

    /*
    Requirements

    Get unsorted list of renderables that are "active/enabled"
    Sort by Z-index (hiearchy?)
    Sort by TextureHandle (most cases only a single texture)
    */


    public class UIRenderer2 : IRenderer
    {
        private readonly UIRenderQueue2 _renderQueue2;

        public UIRenderer2(UIRenderQueue2 renderQueue2)
        {
            _renderQueue2 = renderQueue2;
            
        }

        public unsafe void Render(Context context)
        {
            var uiComponents = _renderQueue2.GetRenderables();

            context.SetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexBuffer(uiComponents.VertexBuffer);
            context.SetIndexBuffer(uiComponents.IndexBuffer);

            foreach (ref readonly var element in uiComponents.Elements)
            {
                context.SetPixelShaderResource(element.Texture);
                context.DrawIndexed(element.Count, element.StartIndex, 0);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
