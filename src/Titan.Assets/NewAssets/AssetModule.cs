using System;
using System.IO;
using System.Linq;
using Titan.Core.Logging;
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
    public static void Build(AppBuilder builder)
    {

        var s = sizeof(AssetContext);
        var b = s * 1000;
        var allocator = builder.GetResourcePointer<PlatformAllocator>();
        var fileApi = builder.GetResourcePointer<FileSystemApi>();
        var assetConfigs = builder
            .GetConfigurations<AssetsConfiguration>()
            ;


        //set up paths where the assets will be loaded from
#if !SHIPPING
        // never use paths for shipping config
        var devSettings = builder.GetConfiguration<AssetsDevConfiguration>();
        if (devSettings != null)
        {
            assetConfigs = assetConfigs.Select(c => c with
            {
                ManifestFile = Path.Combine(devSettings.AssetsFolder, c.ManifestFile),
                TitanPakFile = Path.Combine(devSettings.TitanPakFolder, c.TitanPakFile)
            });
        }

        //
#else 
    
#endif
        var configs = assetConfigs.ToArray();
        if (!AssetRegistry.Create(allocator, configs, out var registry))
        {
            Logger.Error<AssetsModule>("Failed to initialize the asset registry");
            return;
        }

        if (!AssetFileAccessor.Create(allocator, fileApi, configs, out var accessor))
        {
            Logger.Error<AssetsModule>($"Failed to initialize the {nameof(AssetFileAccessor)}");
            return;
        }

        builder
            .AddResource(registry)
            .AddResource(accessor)
            .AddSystemToStage<AssetSystem>(Stage.PreUpdate)
            .AddShutdownSystem<AssetModuleTearDown>(RunCriteria.Always);
    }



    private struct AssetModuleTearDown : IStructSystem<AssetModuleTearDown>
    {
        private AssetRegistry* Registry;
        private AssetFileAccessor* Accessor;

        public static void Init(ref AssetModuleTearDown system, in SystemsInitializer init)
        {
            system.Registry = init.GetResourcePointer<AssetRegistry>();
            system.Accessor = init.GetResourcePointer<AssetFileAccessor>();
        }

        public static void Update(ref AssetModuleTearDown system)
        {
            system.Registry->Release();
            system.Accessor->Release();
        }

        public static bool ShouldRun(in AssetModuleTearDown system)
            => throw new InvalidOperationException($"The {nameof(AssetModuleTearDown)} should be marked as {RunCriteria.Once}");
    }
}
