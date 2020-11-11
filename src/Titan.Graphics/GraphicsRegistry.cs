using Titan.Graphics.Camera;
using Titan.Graphics.Materials;
using Titan.Graphics.Meshes;
using Titan.Graphics.Pipeline.Configuration;
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
                .Register<IShaderCompiler, ShaderCompiler>()

                .Register<IImagingFactory, ImagingFactory>()
                .Register<ITextureLoader, TextureLoader>()

                .Register<IMeshLoader, MeshLoader>()
                .Register<IMeshRenderQueue, NaiveMeshRenderQueue>()
                .Register<ILigthRenderQueue, NaiveLightRenderQueue>()
                .Register<IPipelineConfigurationLoader, PipelineConfigurationLoader>()

                .Register<IMaterialsLoader, MaterialsLoader>()
                .Register<IMaterialsManager, MaterialsManager>()

                .Register<ICameraManager, CameraManager>()

                ;
        }
    }
}
