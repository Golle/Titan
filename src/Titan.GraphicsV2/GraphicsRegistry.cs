using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Shaders;
using Titan.GraphicsV2.Rendering;
using Titan.GraphicsV2.Resources;
using Titan.IOC;

namespace Titan.GraphicsV2
{
    public class GraphicsRegistry : IRegistry
    {
        public void Register(IContainer container)
        {
            container
                .Register<GraphicsSystem>()

                .Register<ShaderCompiler>()
                .Register<ShaderFactory>()
                .Register<InputLayoutFactory>()


                .Register<FrameBufferFactory>()
                .Register<ShaderResourceViewFactory>()
                .Register<RenderTargetViewFactory>()
                .Register<Texture2DFactory>()
                .Register<DepthStencilViewFactory>()
                .Register<ShaderManager>()
                ;
        }
    }
}
