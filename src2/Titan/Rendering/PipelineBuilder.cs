using System.Diagnostics;
using Titan.Assets;
using Titan.Assets.Shaders;
using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Pipeline;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Rendering
{
    internal class PipelineBuilder
    {
        private Handle<Asset> _gBufferHandle;

        private readonly AssetsManager _assetsManager;

        public PipelineBuilder(AssetsManager assetsManager)
        {
            _assetsManager = assetsManager;
        }
        public void LoadResources()
        {
            _gBufferHandle = _assetsManager.Load("shaders/gbuffer");
            
        }
        public bool IsReady()
        {

            return _assetsManager.IsLoaded(_gBufferHandle);
        }

        public void Create()
        {
            Debug.Assert(GraphicsDevice.IsInitialized, $"{nameof(GraphicsDevice)} must be initialized before the {nameof(GraphicsSystem)} is created.");

            var swapchain = GraphicsDevice.SwapChain;

            // Create the framebuffers
            var gBufferAlbedo = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Format = TextureFormats.RGBA32F,
                Width = swapchain.Width,
                Height = swapchain.Height,
                Binding = TextureBindFlags.FrameBuffer
            });
            var gBufferNormals = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Format = TextureFormats.RGBA32F,
                Width = swapchain.Width,
                Height = swapchain.Height,
                Binding = TextureBindFlags.FrameBuffer
            });
            var gBufferProperties = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Format = TextureFormats.RGBA32F,
                Width = swapchain.Width,
                Height = swapchain.Height,
                Binding = TextureBindFlags.FrameBuffer
            });

            var gbufferShaders = _assetsManager.GetAssetHandle<ShaderProgram>(_gBufferHandle);

            var gBuffer = new Pipeline
            {
                RenderTargets = new[] {gBufferAlbedo, gBufferNormals, gBufferProperties},
                PixelShader = gbufferShaders.PixelShader,
                VertexShader =  gbufferShaders.VertexShader,
                ClearColor = Color.Black,
                ClearRenderTargets = true
            };

            var backbufferColor = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Format = TextureFormats.RGBA32F,
                Width = swapchain.Width,
                Height = swapchain.Height,
                Binding = TextureBindFlags.FrameBuffer
            });








            //return new GraphicsSystem();


        }
    }
}
