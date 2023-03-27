using Titan.Assets;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Graphics.D3D12;
using Titan.Graphics.D3D12.Memory;
using Titan.Graphics.Resources;
using Titan.Modules;
using Titan.Setup;
using Titan.Systems;

namespace Titan.Graphics.Rendering.Sprites;
internal struct SpriteRendererAssets
{
    public Handle<Shader> VertexShader;
    public Handle<Shader> PixelShader;
}

internal struct D3D12SpriteRenderingModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddResource<SpriteRendererAssets>()
            .AddManagedResource(new D3D12SpriteRenderer())
            .AddSystem<BatchSpriteRenderSystem>(RunCriteria.Always)
            ;

        return true;
    }

    public static bool Init(IApp app)
    {
        var resourceManager = (D3D12ResourceManager)app.GetManagedResource<IResourceManager>();
        var assetLoader = app.GetManagedResource<AssetLoader>();
        var spriteRenderer = app.GetManagedResource<D3D12SpriteRenderer>();
        var spriteConfig = app.GetConfigOrDefault<SpriteRenderingConfig>();
        var allocator = app.GetManagedResource<D3D12Allocator>();

        ref var spriteRenderAssets = ref app.GetResource<SpriteRendererAssets>();
        //spriteRenderAssets.PixelShader = assetLoader.LoadSynchronous<Shader>(AssetRegistry.BuiltIn.Textures.SpritePS);
        //spriteRenderAssets.VertexShader = assetLoader.LoadSynchronous<Shader>(AssetRegistry.BuiltIn.Textures.SpriteVS);
        spriteRenderAssets.PixelShader = assetLoader.LoadSynchronous<Shader>(AssetRegistry.BuiltIn.Textures.Sprite2PS);
        spriteRenderAssets.VertexShader = assetLoader.LoadSynchronous<Shader>(AssetRegistry.BuiltIn.Textures.Sprite2VS);

        if (spriteRenderAssets.PixelShader.IsInvalid || spriteRenderAssets.VertexShader.IsInvalid)
        {
            Logger.Error<D3D12SpriteRenderingModule>("Failed to load sprite pixel shaders.");
            return false;
        }

        if (!spriteRenderer.Init(resourceManager, allocator, spriteConfig, spriteRenderAssets))
        {
            Logger.Error<D3D12SpriteRenderingModule>($"Failed to init the {nameof(D3D12SpriteRenderer)}.");
            return false;
        }
        return true;
    }

    public static bool Shutdown(IApp app)
    {
        var assetLoader = app.GetManagedResource<AssetLoader>();
        ref var assets = ref app.GetResource<SpriteRendererAssets>();
        //NOTE(Jens): Implement unload if needed. Requires some changes in AssetLoader since we don't have an api for Handle<T>.

        var spriteRenderer = app.GetManagedResource<D3D12SpriteRenderer>();

        spriteRenderer.Shutdown();

        return true;
    }
}
