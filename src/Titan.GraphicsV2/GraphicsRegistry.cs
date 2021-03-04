using Titan.GraphicsV2.Rendering;
using Titan.GraphicsV2.Rendering.Pipepline;
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
            ;
        }
    }
}
