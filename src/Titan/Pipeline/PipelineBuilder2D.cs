using Titan.Assets;
using Titan.Core;
using Titan.Core.Services;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.BlendStates;
using Titan.Graphics.D3D11.Samplers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.Rendering.Sprites;
using Titan.UI.Debugging;

namespace Titan.Pipeline;

internal class PipelineBuilder2D : PipelineBuilder
{
    private Handle<Asset> _uiVS;
    private Handle<Asset> _uiPS;
    private Handle<Asset> _spriteVS;
    private Handle<Asset> _spritePS;
    private Handle<Asset> _lineVS;
    private Handle<Asset> _linePS;

    public override void LoadResources(AssetsManager assetsManager)
    {
        _uiVS = assetsManager.Load("shaders/ui_vs");
        _uiPS = assetsManager.Load("shaders/ui_ps");
        _spriteVS = assetsManager.Load("shaders/sprite_vs");
        _spritePS = assetsManager.Load("shaders/sprite_ps");

        _lineVS = assetsManager.Load("shaders/debug_line_vs");
        _linePS = assetsManager.Load("shaders/debug_line_ps");
    }


    public override bool IsReady(AssetsManager assetsManager) =>
        assetsManager.IsLoaded(_spritePS) &&
        assetsManager.IsLoaded(_spriteVS) &&
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
        var backbufferRenderTarget = GraphicsDevice.TextureManager.CreateBackbufferRenderTarget();
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

        return new[] { backbuffer, debugVerticesPipeline };
    }
}
