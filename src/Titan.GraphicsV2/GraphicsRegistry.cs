using Titan.GraphicsV2.Rendering.Pipepline;
using Titan.GraphicsV2.Rendering.Queue;
using Titan.GraphicsV2.Resources.Materials;
using Titan.GraphicsV2.Resources.Models;
using Titan.GraphicsV2.Resources.Textures;
using Titan.IOC;

namespace Titan.GraphicsV2
{
    public class GraphicsRegistry : IRegistry
    {
        public void Register(IContainer container)
        {
            container
                .Register<GraphicsSystem>()
                .Register<RenderPipelineReader>()
                .Register<ModelLoader>()
                .Register<TextureLoader>()
                .Register<MaterialsLoader>()
                .Register<IImageLoader, WICImageLoader>()

                .Register<ModelRenderQueue>()
            ;
        }
    }
}
