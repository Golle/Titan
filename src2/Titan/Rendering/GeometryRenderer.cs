using System.Runtime.CompilerServices;
using Titan.Graphics.D3D11;

namespace Titan.Rendering
{
    internal interface IRenderer
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Render(Context context);
    }

    internal class GeometryRenderer : IRenderer
    {
        private readonly ViewPort _viewport;

        public GeometryRenderer()
        {
            _viewport = new ViewPort((int) GraphicsDevice.SwapChain.Width, (int) GraphicsDevice.SwapChain.Height);
        }

        public void Render(Context context)
        {
            throw new System.NotImplementedException();
        }
    }

    internal class BackbufferRenderer : IRenderer
    {
        public void Render(Context context)
        {
            throw new System.NotImplementedException();
        }
    }
}
