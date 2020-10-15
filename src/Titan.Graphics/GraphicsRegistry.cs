using Titan.Graphics.Textures;
using Titan.IOC;

namespace Titan.Graphics
{
    public class GraphicsRegistry : IRegistry
    {
        public void Register(IContainer container)
        {
            container
                .Register<IImagingFactory, ImagingFactory>()
                .Register<ITextureLoader, TextureLoader>()
                ;
        }
    }
}
