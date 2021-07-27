using System;
using System.Diagnostics;
using Titan.Assets;
using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Pipeline;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;
using Titan.UI;
using Titan.UI.Rendering;
using Titan.Windows;
using Titan.Windows.D3D11;

namespace Titan.Rendering
{
    internal class PipelineBuilder
    {
        private Handle<Asset> _fullscreenPixelShaderHandle;
        private Handle<Asset> _lambertianPixelShaderHandle;
        private Handle<Asset> _fullscreenVertexShaderHandle;
        private Handle<Asset> _lambertianVertexShaderHandle;
        private Handle<Asset> _uiPixelShaderHandle;
        private Handle<Asset> _uiVertexShaderHandle;

        private readonly AssetsManager _assetsManager;
        private readonly SimpleRenderQueue _simpleRenderQueue;
        private readonly UIRenderQueue2 _uiRenderQueue;
        private GeometryRenderer _geometryRenderer;
        private BackbufferRenderer _backbufferRenderer;
        private UIRenderer2 _uiRenderer;
        private DeferredShadingRenderer _deferredShadingRenderer;

        public PipelineBuilder(AssetsManager assetsManager, SimpleRenderQueue simpleRenderQueue, UIRenderQueue2 uiRenderQueue)
        {
            _assetsManager = assetsManager;
            _simpleRenderQueue = simpleRenderQueue;
            _uiRenderQueue = uiRenderQueue;
        }
        public void LoadResources()
        {
            _lambertianVertexShaderHandle = _assetsManager.Load("shaders/default_vs");
            _lambertianPixelShaderHandle = _assetsManager.Load("shaders/default_ps");
            
            _fullscreenVertexShaderHandle = _assetsManager.Load("shaders/fullscreen_vs");
            //_fullscreenPixelShaderHandle = _assetsManager.Load("shaders/fullscreen_ps");
            _fullscreenPixelShaderHandle = _assetsManager.Load("shaders/fullscreen_debug_ps");

            _uiPixelShaderHandle = _assetsManager.Load("shaders/ui_ps");
            _uiVertexShaderHandle = _assetsManager.Load("shaders/ui_vs");

            _geometryRenderer = new GeometryRenderer(_simpleRenderQueue);
            _backbufferRenderer = new BackbufferRenderer();
            _deferredShadingRenderer = new DeferredShadingRenderer();
            _uiRenderer = new UIRenderer2(_uiRenderQueue);
        }

        public bool IsReady()
        {
            return _assetsManager.IsLoaded(_fullscreenVertexShaderHandle) &&
                   _assetsManager.IsLoaded(_lambertianVertexShaderHandle) &&
                   _assetsManager.IsLoaded(_fullscreenPixelShaderHandle) &&
                   _assetsManager.IsLoaded(_lambertianPixelShaderHandle) &&
                   _assetsManager.IsLoaded(_uiPixelShaderHandle) &&
                   _assetsManager.IsLoaded(_uiVertexShaderHandle)
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

            
            
            
            /***** DEBUG Stuff *****/
            var debugTextureHandle = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Width = swapchain.Width,
                Height = swapchain.Height,
                Binding = TextureBindFlags.FrameBuffer,
                Format = TextureFormats.BGRA8U,
                Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                MiscFlags = D3D11_RESOURCE_MISC_FLAG.D3D11_RESOURCE_MISC_GDI_COMPATIBLE
            });

            using ComPtr<IDXGISurface1> surface = default;
            unsafe
            {
                fixed (Guid* uuid = &D3D11Common.DXGISurface1)
                {
                    Common.CheckAndThrow(GraphicsDevice.TextureManager.Access(debugTextureHandle).D3DTexture->QueryInterface(uuid, (void**)surface.GetAddressOf()), nameof(ID3D11Texture2D.QueryInterface));
                }
            }

            var debugPipeline = new Pipeline
            {
                ClearColor = Color.Zero,
                ClearRenderTargets = true,
                RenderTargets = new[] { debugTextureHandle },
                Renderer = new DebugRenderer(surface)
            };
            /***** DEBUG Stuff *****/


            var backbufferRenderTarget = GraphicsDevice.TextureManager.CreateBackbufferRenderTarget();
            var backbuffer = new Pipeline
            {
                ClearDepthBuffer = false,
                ClearRenderTargets = true,
                ClearColor = Color.Black,
                RenderTargets = new[] { backbufferRenderTarget },
                PixelShader = _assetsManager.GetAssetHandle<PixelShader>(_fullscreenPixelShaderHandle),
                VertexShader = _assetsManager.GetAssetHandle<VertexShader>(_fullscreenVertexShaderHandle),
                PixelShaderResources = new[] { deferredShadingTarget, debugTextureHandle },
                PixelShaderSamplers = new[] { fullscreenSampler },
                Renderer = _backbufferRenderer
            };

            //var uiRenderTarget = GraphicsDevice.TextureManager.Create(new TextureCreation
            //{
            //    Format = TextureFormats.RGBA32F,
            //    Width = swapchain.Width,
            //    Height = swapchain.Height,
            //    Binding = TextureBindFlags.FrameBuffer
            //});

            var uiDepthBuffer = GraphicsDevice.TextureManager.Create(new TextureCreation
            {
                Binding = TextureBindFlags.DepthBuffer,
                DepthStencilFormat = DepthStencilFormats.D24S8,
                Format = TextureFormats.R24G8TL
            });

            // TODO: should we render it to an offscreen buffer to support multi-threaded rendering or directly to the backbuffer?
            var ui = new Pipeline
            {
                RenderTargets = new[] { backbufferRenderTarget },
                //DepthBuffer = uiDepthBuffer,
                //ClearDepthBuffer = true,
                //DepthBufferClearValue = 1f,
                Renderer = _uiRenderer,
                PixelShaderSamplers = new []{ fullscreenSampler },
                VertexShader = _assetsManager.GetAssetHandle<VertexShader>(_uiVertexShaderHandle),
                PixelShader = _assetsManager.GetAssetHandle<PixelShader>(_uiPixelShaderHandle)
            };

            return new[] {gBuffer, deferredShading, debugPipeline, backbuffer, ui };
        }
    }
}
