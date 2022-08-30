using System;
using System.Linq;
using Titan.Core;
using Titan.Core.IO.NewFileSystem;
using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.Memory;

namespace Titan.Assets.NewAssets;

public interface IAssetLoader<T> where T : unmanaged
{
    static abstract Handle<T> Load(ReadOnlySpan<byte> data);
    static abstract void Unload(Handle<T> handle);
}

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

        builder
            .AddResource(registry)
            .AddSystemToStage<AssetSystem>(Stage.PreUpdate);
    }
}
