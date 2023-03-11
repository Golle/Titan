using Titan.Assets.Creators;
using Titan.Core.IO;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Jobs;
using Titan.Modules;
using Titan.Resources;
using Titan.Setup;
using Titan.Systems;

namespace Titan.Assets;


internal struct AssetsModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        //#if DEBUG
        //        var assetFileReader = new Editor.RawAssetFileReader();
        //#else 
        //        var assetFileReader = new AssetFileReader();
        //#endif
        builder
            .AddManagedResource(new AssetsRegistry())
            .AddManagedResource(new AssetLoader())
            .AddManagedResource(new AssetFileReader())
            .AddManagedResource(new ResourceCreatorRegistry())
            .AddSystemToStage<AssetLoaderSystem>(SystemStage.PreUpdate);

        return true;
    }

    public static bool Init(IApp app)
    {
        var assetsRegistry = app.GetManagedResource<AssetsRegistry>();
        var assetLoader = app.GetManagedResource<AssetLoader>();

        var resourceCreatorRegistry = app.GetManagedResource<ResourceCreatorRegistry>();
        var resourceConfigs = app.GetConfigs<ResourceCreatorConfiguration>();
        var resourceCollection = app.GetManagedResource<ResourceCollection>();

        var fileSystem = app.GetManagedResource<IFileSystem>();
        var memoryManager = app.GetManagedResource<IMemoryManager>();
        var fileBufferSize = MemoryUtils.MegaBytes(64); // Replace this with configuration.
        var devConfig = app.GetConfig<AssetsDevConfiguration>();

#if DEBUG
        IAssetFileReader assetFileReader = (devConfig?.UseRawAssets ?? false)
            ? app.GetManagedResource<Editor.RawAssetFileReader>()
            : app.GetManagedResource<AssetFileReader>();
#else 
        IAssetFileReader assetFileReader = app.GetManagedResource<AssetFileReader>();
#endif

        Logger.Trace<AssetsModule>($"Using {assetFileReader.GetType().Name} for reading asset files.");

        var configs = app
            .GetConfigs<AssetsConfiguration>()
#if DEBUG
            //NOTE(Jens): Only set dev paths in Debug configuration.
            .Select(c => c.WithDevPaths(devConfig))
#endif
            .ToArray();

        if (configs.Length == 0)
        {
            Logger.Warning<AssetsModule>("No asset manifests have been registered.");
        }

        if (!assetsRegistry.Init(memoryManager, configs))
        {
            Logger.Error<AssetsModule>($"Failed to init the {nameof(AssetsRegistry)}.");
            return false;
        }

        //NOTE(Jens): Only call init when the file reader is of type AssetFileReader. The RawAssetFileReader is initialized in the editor module.
        if (assetFileReader is AssetFileReader fileReader && !fileReader.Init(memoryManager, fileSystem, configs))
        {
            Logger.Error<AssetsModule>($"Failed to init the {nameof(AssetFileReader)}.");
            return false;
        }

        if (!resourceCreatorRegistry.Init(memoryManager, resourceConfigs.ToArray(), resourceCollection))
        {
            Logger.Error<AssetsModule>($"Failed to init the {nameof(ResourceCreatorRegistry)}.");
            return false;
        }

        if (!assetLoader.Init(memoryManager, assetsRegistry, resourceCreatorRegistry, assetFileReader, app.GetManagedResource<IJobApi>(), fileBufferSize))
        {
            Logger.Error<AssetsModule>($"Failed to init the {nameof(AssetLoader)}.");
            return false;
        }

        return true;
    }

    public static bool Shutdown(IApp app)
    {
        app.GetManagedResource<AssetLoader>()
            .Shutdown();
        app.GetManagedResource<AssetsRegistry>()
            .Shutdown();
        app.GetManagedResource<AssetFileReader>()
            .Shutdown();
        app.GetManagedResource<ResourceCreatorRegistry>()
            .Shutdown();

        return true;
    }
}
