using Titan.Graphics.D3D11;
using Titan.Graphics.Meshes;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Textures;
using Titan.IOC;

namespace Titan.Graphics
{
    public class GraphicsRegistry : IRegistry
    {
        public void Register(IContainer container)
        {
            container
                .Register<IShaderCompiler, ShaderCompiler>()

                .Register<IImagingFactory, ImagingFactory>()
                .Register<ITextureLoader, TextureLoader>()

                .Register<IMeshLoader, MeshLoader>()
                .Register<IMeshRenderQueue, NaiveMeshRenderQueue>()
                .Register<IBufferManager, BufferManager>()
                .Register<IShaderManager, ShaderManager>()
                ;
        }
    }
}
