using Titan.Core.Logging;
using Titan.Core.Threading2;
using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.FileSystem;
using Titan.Memory;

namespace Titan.Assets.NewAssets;

public static unsafe class SystemInitializerExtensions
{
    public static AssetManager GetAssetManager(in this SystemsInitializer initializer) => new(
        initializer.GetResourcePointer<AssetRegistry>(),
        initializer.GetEventsWriter<AssetLoadRequested>(),
        initializer.GetEventsWriter<AssetUnloadRequested>()
    );
}

public unsafe struct AssetsModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        Logger.Info<AssetsModule>($"Init started");

        var assetConfigs = builder.GetConfigurations<AssetsConfiguration>();
        //set up paths where the assets will be loaded from
#if !SHIPPING
        // never use paths for shipping config
        var devSettings = builder.GetConfiguration<AssetsDevConfiguration>();
        if (devSettings != null)
        {
            Logger.Trace<AssetsModule>($"Development Assets Folder: {devSettings.AssetsFolder}");
            Logger.Trace<AssetsModule>($"Development TitanPak Folder: {devSettings.TitanPakFolder}");
            assetConfigs = assetConfigs.Select(c => c with
            {
                ManifestFile = Path.Combine(devSettings.AssetsFolder, c.ManifestFile),
                TitanPakFile = Path.Combine(devSettings.TitanPakFolder, c.TitanPakFile)
            });
        }
#endif

        var configs = assetConfigs.ToArray();

        // Register the resources needed by this module
        builder
            .AddResource<AssetFileAccessor>()
            .AddResource<AssetRegistry>()
            .AddResource<AssetLoader>()
            .AddResource<ResourceCreatorRegistry>()
            ;


        // Get pointers to the resources
        var memoryManager = builder.GetResourcePointer<MemoryManager>();
        var jobApi = builder.GetResourcePointer<JobApi>();
        var fileSystemApi = builder.GetResourcePointer<FileSystemApi>();
        var assetRegistry = builder.GetResourcePointer<AssetRegistry>();
        var fileAccessor = builder.GetResourcePointer<AssetFileAccessor>();
        var resourceCreatorRegistry = builder.GetResourcePointer<ResourceCreatorRegistry>();
        var loader = builder.GetResourcePointer<AssetLoader>();

        // Initialize the resources assosicated with this module
        if (!assetRegistry->Init(memoryManager, configs))
        {
            Logger.Error<AssetsModule>($"Failed to initialize the {nameof(AssetRegistry)}");
            return false;
        }
;
        if (!fileAccessor->Init(memoryManager, fileSystemApi, configs))
        {
            Logger.Error<AssetsModule>($"Failed to initialize the {nameof(AssetFileAccessor)}");
            return false;
        }

        if (!resourceCreatorRegistry->Init(memoryManager, (uint)AssetDescriptorType.Count))
        {
            Logger.Error<AssetsModule>($"Failed to initialize the {nameof(ResourceCreatorRegistry)}");
            return false;
        }

        if (!loader->Init(memoryManager, assetRegistry, jobApi, fileAccessor, resourceCreatorRegistry))
        {
            Logger.Error<AssetsModule>($"Failed to initialize the {nameof(AssetLoader)}");
            return false;
        }


        // add the systems
        builder
            .AddSystemToStage<AssetSystem>(Stage.PreUpdate)
            .AddShutdownSystem<AssetModuleTearDown>(RunCriteria.Always);

        Logger.Info<AssetsModule>($"Init complete");
        return true;
    }
    private struct AssetModuleTearDown : IStructSystem<AssetModuleTearDown>
    {
        private AssetRegistry* Registry;
        private AssetFileAccessor* Accessor;
        private ResourceCreatorRegistry* Creator;
        private AssetLoader* Loader;

        public static void Init(ref AssetModuleTearDown system, in SystemsInitializer init)
        {
            system.Registry = init.GetResourcePointer<AssetRegistry>();
            system.Accessor = init.GetResourcePointer<AssetFileAccessor>();
            system.Creator = init.GetResourcePointer<ResourceCreatorRegistry>();
            system.Loader = init.GetResourcePointer<AssetLoader>();
        }

        public static void Update(ref AssetModuleTearDown system)
        {
            system.Registry->Release();
            system.Accessor->Release();
            system.Creator->Release();
            //system.Loader->Release();
        }

        public static bool ShouldRun(in AssetModuleTearDown system)
            => throw new InvalidOperationException($"The {nameof(AssetModuleTearDown)} should be marked as {RunCriteria.Once}");
    }
}
