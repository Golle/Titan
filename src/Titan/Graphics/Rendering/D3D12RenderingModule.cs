using Titan.Graphics.Rendering.PostProcessing;
using Titan.Graphics.Rendering.Sprites;
using Titan.Modules;
using Titan.Setup;

namespace Titan.Graphics.Rendering;

internal struct D3D12RenderingModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddAssetsManifest<AssetRegistry.BuiltIn>(builtinAsset: true)
            .AddModule<D3D12SpriteRenderingModule>()
            .AddModule<D3D12PostProcessingModule>()
            ;

        return true;
    }
    public static bool Init(IApp app) => true;
    public static bool Shutdown(IApp app) => true;
}
