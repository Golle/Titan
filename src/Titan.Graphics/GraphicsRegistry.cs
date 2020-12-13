using Titan.Graphics.Camera;
using Titan.Graphics.D3D11;
using Titan.Graphics.Materials;
using Titan.Graphics.Meshes;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Resources;
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
                
                // Graphics pipeline
                .Register<IGraphicsPipeline, GraphicsPipeline>()
                .Register<IRenderPassFactory, RenderPassFactory>()
                
                // Shaders
                .Register<IShaderCompiler, ShaderCompiler>()
                
                
                // D3D11 Managers
                .Register<ITextureManager, TextureManager>()
                .Register<IShaderResourceViewManager, ShaderResourceViewManager>()
                .Register<IVertexBufferManager, VertexBufferManager>()
                .Register<IIndexBufferManager, IndexBufferManager>()

                
                // Image loading
                .Register<IImagingFactory, ImagingFactory>()
                .Register<ITextureLoader, TextureLoader>()

                // Model loading
                .Register<IMeshLoader, MeshLoader>()
                
                
                // Default render queues
                .Register<IMeshRenderQueue, NaiveMeshRenderQueue>()
                .Register<ILigthRenderQueue, NaiveLightRenderQueue>()

                
                // Materials
                .Register<IMaterialsLoader, MaterialsLoader>()
                .Register<IMaterialsManager, MaterialsManager>()

                // Temp camera
                .Register<ICameraManager, CameraManager>()

                ;
        }
    }
}
