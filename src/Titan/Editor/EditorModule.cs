using Titan.Assets;
using Titan.Core.Logging;
using Titan.Modules;
using Titan.Setup;
using Titan.Systems;

namespace Titan.Editor;

internal class EditorModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddManagedResource(new RawAssetFileReader())
            .AddManagedResource(new AssetChangeDetector())
            .AddSystemToStage<AssetHotReloadSystem>(SystemStage.PostUpdate);
        return true;
    }

    public static bool Init(IApp app)
    {
        var devConfig = app.GetConfig<AssetsDevConfiguration>();
        if (devConfig == null)
        {
            Logger.Warning<EditorModule>($"No {nameof(AssetsDevConfiguration)} set, can't use any editor features.");
            return true;
        }

        if (!devConfig.UseRawAssets)
        {
            Logger.Trace<EditorModule>($"{nameof(AssetsDevConfiguration.UseRawAssets)} is set to false. Hot reloading raw assets is disabled.");
            return true;
        }

        var configs = app.GetConfigs<AssetsConfiguration>()
            .Select(c => c.WithDevPaths(devConfig))
            .ToArray();

        var assetFileReader = app.GetManagedResource<RawAssetFileReader>();
        if (!assetFileReader.Init(configs, devConfig))
        {
            Logger.Error<EditorModule>($"Failed to init the {nameof(RawAssetFileReader)}.");
            return false;
        }

        var assetRegistry = app.GetManagedResource<AssetsRegistry>();
        var changeDetector = app.GetManagedResource<AssetChangeDetector>();
        if (!changeDetector.Init(configs, assetRegistry))
        {
            Logger.Error<EditorModule>($"Failed to init the {nameof(AssetChangeDetector)}.");

            return false;
        }

        return true;
    }

    public static bool Shutdown(IApp app)
    {
        return true;
    }
}
