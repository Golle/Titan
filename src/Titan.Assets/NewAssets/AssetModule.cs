using System.Linq;
using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Scheduler;
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
        var allocator = builder.GetResourcePointer<PlatformAllocator>();
        var fileApi = builder.GetResourcePointer<FileSystemApi>();
        var assetConfigs = builder
            .GetConfigurations<AssetsConfiguration>()
            .ToArray();


        //set up paths where the assets will be loaded from
#if !SHIPPING
        // never use paths for shipping config
        var devSettings = builder.GetConfiguration<AssetsDevConfiguration>();
        if (devSettings != null)
        {


        }

        //
#else 
    
#endif
        if (!AssetRegistry.Create(allocator, assetConfigs, out var registry))
        {
            Logger.Error<AssetsModule>("Failed to initialize the asset registry");
            return;
        }

        if (AssetFile.Open(@"F:\Git\Titan\samples\Titan.Sandbox\assets\bin\data001.titanpak", fileApi, out var file))
        {
            Logger.Trace<AssetsModule>($"File opened! Size: {file.GetLength()}.");
        }

        file.Close();

        builder
            .AddResource(registry)
            .AddSystemToStage<AssetSystem>(Stage.PreUpdate);
    }
}
