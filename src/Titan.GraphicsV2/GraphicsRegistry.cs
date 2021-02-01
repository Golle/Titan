using Titan.IOC;

namespace Titan.GraphicsV2
{
    public class GraphicsRegistry : IRegistry
    {
        public void Register(IContainer container)
        {
            container
                .Register<GraphicsSystem>()
                ;
        }
    }
}
