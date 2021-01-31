using Titan.Graphics.Camera;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Images;
using Titan.Graphics.Materials;
using Titan.Graphics.Meshes;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Graphics.States;
using Titan.Graphics.Textures;
using Titan.Graphics.Textures.NewTest;
using Titan.IOC;

namespace Titan.Graphics
{
    public class GraphicsRegistry : IRegistry
    {
        public void Register(IContainer container)
        {
            container
                .Register<GraphicsSystem>()
                .Register<IGraphicsDevice, D3D11GraphicsDevice>() // TODO: remove this, no need to have an interface for this device. All classes using it will be specific for D3D11
                //.Register<D3D11GraphicsDevice>() // Wont work
                .Register<TextureFactory>()
                .Register<Texture2DFactory>()
                .Register<ShaderResourceViewFactory>()
                
                // Graphics pipeline
                .Register<IGraphicsPipeline, GraphicsPipeline>()
                .Register<IRenderPassFactory, RenderPassFactory>()
                
                // Shaders
                .Register<IShaderCompiler, ShaderCompiler>()
                
                // D3D11 Managers
                .Register<ITexture2DManager, Texture2DManager>()
                .Register<IShaderResourceViewManager, ShaderResourceViewManager>()
                .Register<IVertexBufferManager, VertexBufferManager>()
                .Register<IIndexBufferManager, IndexBufferManager>()
                .Register<IConstantBufferManager, ConstantBufferManager>()
                .Register<IRenderTargetViewManager, RenderTargetViewManager>()
                .Register<IDepthStencilViewManager, DepthStencilViewManager>()
                .Register<ISamplerStateManager, SamplerStateManager>()
                .Register<IDepthStencilStateManager, DepthStencilStateManager>()
                .Register<IShaderManager, ShaderManager>()
                
                // Image loading
                .Register<IImageFactory, WICImageFactory>(dispose: true)
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
