using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Input;
using Titan.Platform.Win32.D3D;

namespace Titan.UI.Debugging
{
    public class BoundingBoxRenderer : Renderer
    {
        private readonly BoundingBoxRenderQueue _renderQueue;
        private bool _enabled = true;

        public BoundingBoxRenderer(BoundingBoxRenderQueue renderQueue)
        {
            _renderQueue = renderQueue;
        }

        public override void Render(Context context)
        {
            if (InputManager.IsKeyPressed(KeyCode.F9))
            {
                _enabled = !_enabled;
            }

            if (!_enabled)
            {
                return;
            }
            context.SetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_LINELIST);
            context.SetVertexBuffer(_renderQueue.VertexBuffer);
            context.Draw((uint)_renderQueue.NumberOfVertices);
        }
    }
}
