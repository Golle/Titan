using Titan.Assets;
using Titan.Core;
using Titan.Core.Services;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.BlendStates;
using Titan.Graphics.D3D11.Pipeline;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Rendering.Sprites;
using Titan.UI.Rendering;
using SpriteRenderer = Titan.UI.Rendering.SpriteRenderer;
using SpriteRenderQueue = Titan.UI.Rendering.SpriteRenderQueue;

namespace Titan.Rendering;

internal class PipelineBuilder2D
{
    private Handle<Asset> _uiVS;
    private Handle<Asset> _uiPS;
    private Handle<Asset> _spriteVS;
    private Handle<Asset> _spritePS;

    public void LoadResources(AssetsManager assetsManager)
    {
        _uiVS = assetsManager.Load("shaders/ui_vs");
        _uiPS = assetsManager.Load("shaders/ui_ps");
        _spriteVS = assetsManager.Load("shaders/sprite_vs");
        _spritePS = assetsManager.Load("shaders/sprite_ps");
    }


    public bool IsReady(AssetsManager assetsManager) =>
        assetsManager.IsLoaded(_spritePS) &&
        assetsManager.IsLoaded(_spriteVS) &&
        assetsManager.IsLoaded(_uiPS) &&
        assetsManager.IsLoaded(_uiVS);


    public Pipeline[] BuildPipeline(IServiceCollection services)
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

        var backbufferRenderTarget = GraphicsDevice.TextureManager.CreateBackbufferRenderTarget();
        var backbuffer = new Pipeline
        {
            ClearRenderTargets = true,
            ClearColor = Color.Red,
            RenderTargets = new[] { backbufferRenderTarget },
            PixelShader = assetsManager.GetAssetHandle<PixelShader>(_spritePS),
            VertexShader = assetsManager.GetAssetHandle<VertexShader>(_spriteVS),
            PixelShaderSamplers = new[] { pointSampler },
            Renderer = new Sprites.SpriteRenderer(services.Get<Sprites.SpriteRenderQueue>())
        };

        var uiBlendState = GraphicsDevice.BlendStateManager.Create(new BlendStateCreation());
        var ui = new Pipeline
        {
            RenderTargets = new[] { backbufferRenderTarget },
            //DepthBuffer = uiDepthBuffer,
            //ClearDepthBuffer = true,
            //DepthBufferClearValue = 1f,
            Renderer = new SpriteRenderer(services.Get<SpriteRenderQueue>()),
            PixelShaderSamplers = new[] { linearSampler }, // TODO: text must be rendered with a different sampler :O
            VertexShader = assetsManager.GetAssetHandle<VertexShader>(_uiVS),
            PixelShader = assetsManager.GetAssetHandle<PixelShader>(_uiPS),
            BlendState = uiBlendState,
            //RasterizerState = uiRasterizerState
        };

        return new[] { backbuffer, ui };
    }
}
