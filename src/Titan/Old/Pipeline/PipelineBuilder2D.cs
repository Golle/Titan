using System;
using Titan.Assets;
using Titan.Core;
using Titan.Core.Services;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.BlendStates;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.Textures;
using Titan.Graphics.Rendering;
using Titan.Graphics.Rendering.Sprites;
using Titan.Old.Helpers;
using Titan.UI.Debugging;
using Titan.Windows;
using Titan.Windows.D3D11;
using Titan.Windows.DXGI;

namespace Titan.Old.Pipeline;

internal class PipelineBuilder2D : PipelineBuilder
{
    private Handle<Asset> _uiVS;
    private Handle<Asset> _uiPS;
    private Handle<Asset> _spriteVS;
    private Handle<Asset> _spritePS;
    private Handle<Asset> _lineVS;
    private Handle<Asset> _linePS;
    private Handle<Asset> _fullscreenVS;
    private Handle<Asset> _debugPS;

    public override void LoadResources(AssetsManager assetsManager)
    {
        _uiVS = assetsManager.Load("shaders/ui_vs");
        _uiPS = assetsManager.Load("shaders/ui_ps");
        _spriteVS = assetsManager.Load("shaders/sprite_vs");
        _spritePS = assetsManager.Load("shaders/sprite_ps");

        _lineVS = assetsManager.Load("shaders/debug_line_vs");
        _linePS = assetsManager.Load("shaders/debug_line_ps");

#if STATS
        _fullscreenVS = assetsManager.Load("shaders/fullscreen_vs");
        _debugPS = assetsManager.Load("shaders/debug_ps");
#endif
    }

    public override bool IsReady(AssetsManager assetsManager) =>
        assetsManager.IsLoaded(_spritePS) &&
        assetsManager.IsLoaded(_spriteVS) &&
#if STATS
        assetsManager.IsLoaded(_debugPS) &&
        assetsManager.IsLoaded(_fullscreenVS) &&
#endif
        assetsManager.IsLoaded(_uiPS) &&
        assetsManager.IsLoaded(_uiVS) &&
        assetsManager.IsLoaded(_linePS) &&
        assetsManager.IsLoaded(_lineVS)
    ;


    public override Graphics.D3D11.Pipeline.Pipeline[] BuildPipeline(IServiceCollection services)
    {
        var assetsManager = services.Get<AssetsManager>();
        var pointSampler = GraphicsDevice.SamplerManager.Create(new SamplerCreation
        {
            Filter = TextureFilter.MinMagMipPoint,
            AddressAll = TextureAddressMode.Wrap,
        });

        var linearSampler = GraphicsDevice.SamplerManager.Create(new SamplerCreation
        {
            Filter = TextureFilter.MinMagMipLinear,
            AddressAll = TextureAddressMode.Wrap,
        });

        var blendState = GraphicsDevice.BlendStateManager.Create(new BlendStateCreation());
        var backbufferRenderTarget = GraphicsDevice.Backbuffer;
        var backbuffer = new Graphics.D3D11.Pipeline.Pipeline
        {
            ClearRenderTargets = true,
            ClearColor = Color.Black,
            BlendState = blendState,
            RenderTargets = new[] { backbufferRenderTarget },
            PixelShader = assetsManager.GetAssetHandle<PixelShader>(_spritePS),
            VertexShader = assetsManager.GetAssetHandle<VertexShader>(_spriteVS),
            PixelShaderSamplers = new[] { pointSampler, linearSampler },
            Renderer = new SpriteRenderer(services.Get<SpriteRenderQueue>())
        };
        
        var debugVerticesPipeline = new Graphics.D3D11.Pipeline.Pipeline
        {
            RenderTargets = new[] { backbufferRenderTarget },
            Renderer = new BoundingBoxRenderer(services.Get<BoundingBoxRenderQueue>()),
            VertexShader = assetsManager.GetAssetHandle<VertexShader>(_lineVS),
            PixelShader = assetsManager.GetAssetHandle<PixelShader>(_linePS),
        };

#if STATS
        /***** DEBUG Stuff *****/
        var debugTextureHandle = GraphicsDevice.TextureManager.Create(new TextureCreation
        {
            Width = GraphicsDevice.SwapChain.Width,
            Height = GraphicsDevice.SwapChain.Height,
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
        var fullscreenSampler = GraphicsDevice.SamplerManager.Create(new SamplerCreation
        {
            Filter = TextureFilter.MinMagMipPoint,
            AddressAll = TextureAddressMode.Wrap,
        });
        var debugOverlay = new Graphics.D3D11.Pipeline.Pipeline
        {
            ClearRenderTargets = false,
            RenderTargets = new[] { backbufferRenderTarget },
            Renderer = new FullscreenRenderer(),
            PixelShader = assetsManager.GetAssetHandle<PixelShader>(_debugPS),
            VertexShader = assetsManager.GetAssetHandle<VertexShader>(_fullscreenVS),
            PixelShaderResources = new[] { debugTextureHandle },
            PixelShaderSamplers = new[] { fullscreenSampler },
        };
        /***** DEBUG Stuff *****/

        return new[] { debugPipeline, backbuffer, debugVerticesPipeline, debugOverlay };
#else
        return new[] {  backbuffer, debugVerticesPipeline };
#endif
    }
}
