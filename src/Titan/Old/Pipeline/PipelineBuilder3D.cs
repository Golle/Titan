using System;
using System.Diagnostics;
using Titan.Assets;
using Titan.Core;
using Titan.Core.Services;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.BlendStates;
using Titan.Graphics.D3D11.Rasterizer;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Rendering;
using Titan.Graphics.Rendering.Geometry;
using Titan.Graphics.Rendering.Sprites;
using Titan.Old.Helpers;
using Titan.UI.Debugging;
using Titan.Windows;
using Titan.Windows.D3D11;
using Titan.Windows.DXGI;

namespace Titan.Old.Pipeline;

internal class PipelineBuilder3D : PipelineBuilder
{
    private Handle<Asset> _fullscreenPixelShaderHandle;
    private Handle<Asset> _lambertianPixelShaderHandle;
    private Handle<Asset> _fullscreenVertexShaderHandle;
    private Handle<Asset> _lambertianVertexShaderHandle;
    private Handle<Asset> _uiPixelShaderHandle;
    private Handle<Asset> _uiVertexShaderHandle;

    private Handle<Asset> _debugPixelShaderHandle;
    private Handle<Asset> _debugLinePixelShaderHandle;
    private Handle<Asset> _debugLineVertexShaderHandle;

    public override void LoadResources(AssetsManager assetsManager)
    {
        _lambertianVertexShaderHandle = assetsManager.Load("shaders/default_vs");
        _lambertianPixelShaderHandle = assetsManager.Load("shaders/default_ps");
            
        _fullscreenVertexShaderHandle = assetsManager.Load("shaders/fullscreen_vs");
        //_fullscreenPixelShaderHandle = _assetsManager.Load("shaders/fullscreen_ps");
        _fullscreenPixelShaderHandle = assetsManager.Load("shaders/fullscreen_debug_ps");

        _uiPixelShaderHandle = assetsManager.Load("shaders/ui_ps");
        _uiVertexShaderHandle = assetsManager.Load("shaders/ui_vs");
        _debugPixelShaderHandle = assetsManager.Load("shaders/debug_ps");


        _debugLinePixelShaderHandle = assetsManager.Load("shaders/debug_line_ps");
        _debugLineVertexShaderHandle = assetsManager.Load("shaders/debug_line_vs");
    }

    public override bool IsReady(AssetsManager assetsManager) =>
        assetsManager.IsLoaded(_fullscreenVertexShaderHandle) &&
        assetsManager.IsLoaded(_lambertianVertexShaderHandle) &&
        assetsManager.IsLoaded(_fullscreenPixelShaderHandle) &&
        assetsManager.IsLoaded(_lambertianPixelShaderHandle) &&
        assetsManager.IsLoaded(_uiPixelShaderHandle) &&
        assetsManager.IsLoaded(_uiVertexShaderHandle) &&
        assetsManager.IsLoaded(_debugPixelShaderHandle) &&
        assetsManager.IsLoaded(_debugLineVertexShaderHandle) &&
        assetsManager.IsLoaded(_debugLinePixelShaderHandle);

    public override Graphics.D3D11.Pipeline.Pipeline[] BuildPipeline(IServiceCollection services)
    {
        Debug.Assert(GraphicsDevice.IsInitialized, $"{nameof(GraphicsDevice)} must be initialized before the {nameof(GraphicsSystem)} is created.");

        var assetsManager = services.Get<AssetsManager>();

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

        var gBuffer = new Graphics.D3D11.Pipeline.Pipeline
        {
            RenderTargets = new[] {gBufferPosition, gBufferAlbedo, gBufferNormals},
            ClearDepthBuffer = true,
            DepthBufferClearValue = 1f,
            DepthBuffer = depthBuffer,
            ClearColor = Color.White,
            ClearRenderTargets = true,
            Renderer = services.Get<GeometryRenderer>()
        };

        var deferredShadingTarget = GraphicsDevice.TextureManager.Create(new TextureCreation
        {
            Format = TextureFormats.RGBA32F,
            Width = swapchain.Width,
            Height = swapchain.Height,
            Binding = TextureBindFlags.FrameBuffer
        });

        var deferredShading = new Graphics.D3D11.Pipeline.Pipeline
        {
            PixelShader = assetsManager.GetAssetHandle<PixelShader>(_lambertianPixelShaderHandle),
            VertexShader = assetsManager.GetAssetHandle<VertexShader>(_lambertianVertexShaderHandle),
            RenderTargets = new []{deferredShadingTarget},
            PixelShaderResources = new []{gBufferPosition, gBufferAlbedo, gBufferNormals},
            PixelShaderSamplers = new []{fullscreenSampler},
            Renderer = services.Get<DeferredShadingRenderer>()
        };


        var backbufferRenderTarget = GraphicsDevice.Backbuffer;
        var fullscreenRenderer = services.Get<FullscreenRenderer>();
        var backbuffer = new Graphics.D3D11.Pipeline.Pipeline
        {
            ClearDepthBuffer = false,
            //ClearRenderTargets = true,
            //ClearColor = Color.Black,
            RenderTargets = new[] { backbufferRenderTarget },
            PixelShader = assetsManager.GetAssetHandle<PixelShader>(_fullscreenPixelShaderHandle),
            VertexShader = assetsManager.GetAssetHandle<VertexShader>(_fullscreenVertexShaderHandle),
            PixelShaderResources = new[] { deferredShadingTarget },
            PixelShaderSamplers = new[] { fullscreenSampler },
            Renderer = fullscreenRenderer
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

        var uiBlendState = GraphicsDevice.BlendStateManager.Create(new BlendStateCreation());
        var uiRasterizerState = GraphicsDevice.RasterizerManager.Create(new RasterizerStateCreation(CullMode.Back));

        var uiSampler = GraphicsDevice.SamplerManager.Create(new SamplerCreation
        {
            Filter = TextureFilter.MinPointMagMipLinear,
            AddressAll = TextureAddressMode.Clamp,
        });

        // TODO: should we render it to an offscreen buffer to support multi-threaded rendering or directly to the backbuffer?
        var ui = new Graphics.D3D11.Pipeline.Pipeline
        {
            RenderTargets = new[] { backbufferRenderTarget },
            //DepthBuffer = uiDepthBuffer,
            //ClearDepthBuffer = true,
            //DepthBufferClearValue = 1f,
            Renderer = services.Get<SpriteRenderer>(),
            PixelShaderSamplers = new []{ uiSampler }, // TODO: text must be rendered with a different sampler :O
            VertexShader = assetsManager.GetAssetHandle<VertexShader>(_uiVertexShaderHandle),
            PixelShader = assetsManager.GetAssetHandle<PixelShader>(_uiPixelShaderHandle),
            BlendState = uiBlendState,
            //RasterizerState = uiRasterizerState
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

        var debugPipeline = new Graphics.D3D11.Pipeline.Pipeline
        {
            ClearColor = Color.Zero,
            ClearRenderTargets = true,
            RenderTargets = new[] { debugTextureHandle },
            Renderer = new DebugRenderer(surface)
        };
            
        var debugOverlay = new Graphics.D3D11.Pipeline.Pipeline
        {
            ClearRenderTargets =  false,
            RenderTargets = new[] { backbufferRenderTarget },
            Renderer = fullscreenRenderer,
            PixelShader = assetsManager.GetAssetHandle<PixelShader>(_debugPixelShaderHandle),
            VertexShader = assetsManager.GetAssetHandle<VertexShader>(_fullscreenVertexShaderHandle),
            PixelShaderResources = new[] { debugTextureHandle },
            PixelShaderSamplers = new[] { fullscreenSampler },
        };
        /***** DEBUG Stuff *****/


        var debugVerticesPipeline = new Graphics.D3D11.Pipeline.Pipeline
        {

            RenderTargets = new[] { backbufferRenderTarget },
            Renderer = services.Get<BoundingBoxRenderer>(),
            VertexShader = assetsManager.GetAssetHandle<VertexShader>(_debugLineVertexShaderHandle),
            PixelShader= assetsManager.GetAssetHandle<PixelShader>(_debugLinePixelShaderHandle),
        };

        return new[] {gBuffer, deferredShading, debugPipeline, backbuffer, ui, debugVerticesPipeline, debugOverlay };
    }
}
