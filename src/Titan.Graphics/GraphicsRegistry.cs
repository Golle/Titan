using Titan.Graphics.Camera;
using Titan.Graphics.D3D11;
using Titan.Graphics.Materials;
using Titan.Graphics.Meshes;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Shaders;
using Titan.Graphics.Textures;
using Titan.IOC;

namespace Titan.Graphics
{
    public class GraphicsRegistry : IRegistry
    {
        public void Register(IContainer container)
        {
            container
                .Register<IGraphicsDevice, GraphicsDevice>()
                .Register<IGraphicsPipeline, GraphicsPipeline>()
                .Register<IShaderCompiler, ShaderCompiler>()

                .Register<IImagingFactory, ImagingFactory>()
                .Register<ITextureLoader, TextureLoader>()

                .Register<IMeshLoader, MeshLoader>()
                .Register<IMeshRenderQueue, NaiveMeshRenderQueue>()
                .Register<ILigthRenderQueue, NaiveLightRenderQueue>()

                .Register<IMaterialsLoader, MaterialsLoader>()
                .Register<IMaterialsManager, MaterialsManager>()

                .Register<ICameraManager, CameraManager>()

                ;
        }
    }
}
