using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Graphics.D3D12;
using Titan.Graphics.Rendering.PostProcessing;
using Titan.Graphics.Rendering.Sprites;
using Titan.Modules;
using Titan.Platform.Win32.D3D12;
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
    public static bool Init(IApp app)
    {
        var device = app.GetManagedResource<IGraphicsDevice>() as D3D12GraphicsDevice;
        Debug.Assert(device != null);

        if (device.TryCheckFeatureSupport<D3D12_FEATURE_DATA_D3D12_OPTIONS5>(D3D12_FEATURE.D3D12_FEATURE_D3D12_OPTIONS5, out var support))
        {
            Logger.Trace<D3D12RenderingModule>($"{nameof(D3D12_FEATURE_DATA_D3D12_OPTIONS5)}. {nameof(D3D12_FEATURE_DATA_D3D12_OPTIONS5.RenderPassesTier)}={support.RenderPassesTier}, {nameof(D3D12_FEATURE_DATA_D3D12_OPTIONS5.RaytracingTier)}={support.RaytracingTier}, {nameof(D3D12_FEATURE_DATA_D3D12_OPTIONS5.SRVOnlyTiledResourceTier3)}={support.SRVOnlyTiledResourceTier3}");
        }
        return true;
    }

    public static bool Shutdown(IApp app) => true;
}
