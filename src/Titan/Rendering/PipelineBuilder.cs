using System.Diagnostics;
using Titan.Assets;
using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Pipeline;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;

namespace Titan.Rendering
{
    internal class PipelineBuilder
    {
        private Handle<Asset> _fullscreenPixelShaderHandle;
        private Handle<Asset> _lambertianPixelShaderHandle;
        private Handle<Asset> _fullscreenVertexShaderHandle;
        private Handle<Asset> _lambertianVertexShaderHandle;

        private readonly AssetsManager _assetsManager;
        private readonly SimpleRenderQueue _simpleRenderQueue;
        private GeometryRenderer _geometryRenderer;
        private BackbufferRenderer _backbufferRenderer;
        private UIRenderer _uiRenderer;
        private DeferredShadingRenderer _deferredShadingRenderer;

        public PipelineBuilder(AssetsManager assetsManager, SimpleRenderQueue simpleRenderQueue)
        {
            _assetsManager = assetsManager;
            _simpleRenderQueue = simpleRenderQueue;
        }
        public void LoadResources()
        {
            _lambertianVertexShaderHandle = _assetsManager.Load("shaders/default_vs");
            _fullscreenVertexShaderHandle = _assetsManager.Load("shaders/fullscreen_vs");
            _lambertianPixelShaderHandle = _assetsManager.Load("shaders/default_ps");
            _fullscreenPixelShaderHandle = _assetsManager.Load("shaders/fullscreen_ps");

            _geometryRenderer = new GeometryRenderer(_simpleRenderQueue);
            _backbufferRenderer = new BackbufferRenderer();
            _deferredShadingRenderer = new DeferredShadingRenderer();
            _uiRenderer = new UIRenderer();
        }

        public bool IsReady()
        {
            return _assetsManager.IsLoaded(_fullscreenVertexShaderHandle) &&
                   _assetsManager.IsLoaded(_lambertianVertexShaderHandle) &&
                   _assetsManager.IsLoaded(_fullscreenPixelShaderHandle) &&
                   _assetsManager.IsLoaded(_lambertianPixelShaderHandle)
                   ;
        }

        public Pipeline[] Create()
        {
            Debug.Assert(GraphicsDevice.IsInitialized, $"{nameof(GraphicsDevice)} must be initialized before the {nameof(GraphicsSystem)} is created.");

            var swapchain = GraphicsDevice.SwapChain;

            // Create the framebuffers
            var gBufferPosition = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Format = TextureFormats.RGBA32F,
                Width = swapchain.Width,
                Height = swapchain.Height,
                Binding = TextureBindFlags.FrameBuffer
            });
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
            var depthBuffer = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Binding = TextureBindFlags.DepthBuffer,
                DepthStencilFormat = DepthStencilFormats.D24S8,
                Format = TextureFormats.R24G8TL
            });

            var fullscreenSampler = GraphicsDevice.SamplerManager.Create(new SamplerCreation
            {
                Filter = TextureFilter.MinMagMipPoint,
                AddressAll = TextureAddressMode.Wrap,
            });

            var gBuffer = new Pipeline
            {
                RenderTargets = new[] {gBufferPosition, gBufferAlbedo, gBufferNormals},
                ClearDepthBuffer = true,
                DepthBufferClearValue = 1f,
                DepthBuffer = depthBuffer,
                ClearColor = Color.White,
                ClearRenderTargets = true,
                Renderer = _geometryRenderer
            };

            var deferredShadingTarget = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Format = TextureFormats.RGBA32F,
                Width = swapchain.Width,
                Height = swapchain.Height,
                Binding = TextureBindFlags.FrameBuffer
            });

            var deferredShading = new Pipeline
            {
                ClearRenderTargets = true,
                ClearColor = Color.Black,
                PixelShader = _assetsManager.GetAssetHandle<PixelShader>(_lambertianPixelShaderHandle),
                VertexShader = _assetsManager.GetAssetHandle<VertexShader>(_lambertianVertexShaderHandle),
                RenderTargets = new []{deferredShadingTarget},
                PixelShaderResources = new []{gBufferPosition, gBufferAlbedo, gBufferNormals},
                PixelShaderSamplers = new []{fullscreenSampler},
                Renderer = _deferredShadingRenderer
            };

            var uiRenderTarget = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Format = TextureFormats.RGBA32F,
                Width = swapchain.Width,
                Height = swapchain.Height,
                Binding = TextureBindFlags.FrameBuffer
            });

            var ui = new Pipeline
            {
                ClearRenderTargets = true,
                ClearColor = new Color(0.1f, 0, 0, 0.1f),
                RenderTargets = new[] { uiRenderTarget },
                Renderer = _uiRenderer
            };
            
            var backbufferRenderTarget = GraphicsDevice.TextureManager.CreateBackbufferRenderTarget();
            var backbuffer = new Pipeline
            {
                ClearDepthBuffer = false,
                ClearRenderTargets = true,
                ClearColor = Color.Green,
                RenderTargets = new[] {backbufferRenderTarget},
                PixelShader = _assetsManager.GetAssetHandle<PixelShader>(_fullscreenPixelShaderHandle),
                VertexShader = _assetsManager.GetAssetHandle<VertexShader>(_fullscreenVertexShaderHandle),
                PixelShaderResources = new[] { deferredShadingTarget, uiRenderTarget },
                PixelShaderSamplers = new []{fullscreenSampler},
                Renderer = _backbufferRenderer
            };

            return new[] {gBuffer, deferredShading, ui, backbuffer};
        }
    }
}
