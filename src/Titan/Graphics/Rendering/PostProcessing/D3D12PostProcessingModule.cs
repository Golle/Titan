using Titan.Assets;
using Titan.Core.Logging;
using Titan.Graphics.Resources;
using Titan.Modules;
using Titan.Setup;
using Titan.Systems;

namespace Titan.Graphics.Rendering.PostProcessing;


internal struct FullScreenPostProcessingSystem : ISystem
{
    public void Init(in SystemInitializer init)
    {
        
        
    }

    public void Update()
    {
        
    }

    public bool ShouldRun()
    {
        return false;
    }
}

internal struct D3D12PostProcessingModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddManagedResource(new D3D12PostProcessingRenderer())
            .AddSystemToStage<FullScreenPostProcessingSystem>(SystemStage.PostUpdate);

        return true;
    }

    public static bool Init(IApp app)
    {
        var renderer = app.GetManagedResource<D3D12PostProcessingRenderer>();
        var assetLoader = app.GetManagedResource<AssetLoader>();

        var fullscreenPixelShader = assetLoader.LoadSynchronous<Shader>(AssetRegistry.BuiltIn.Textures.FullscreenPS);
        var fullscreenVertexShader = assetLoader.LoadSynchronous<Shader>(AssetRegistry.BuiltIn.Textures.FullscreenVS);

        if (!renderer.Init())
        {
            Logger.Error<D3D12PostProcessingModule>($"Failed to init the {nameof(D3D12PostProcessingRenderer)}.");
            return false;
        }
        return true;
    }

    public static bool Shutdown(IApp app)
    {
        var renderer = app.GetManagedResource<D3D12PostProcessingRenderer>();
        renderer.Shutdown();

        return true;
    }
}


internal class D3D12PostProcessingRenderer
{
    public bool Init()
    {
        return true;
    }


    public void Shutdown()
    {

    }


}
