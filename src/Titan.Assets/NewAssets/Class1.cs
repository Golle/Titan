using System;
using System.IO;
using Titan.Core;
using Titan.Core.IO.NewFileSystem;
using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Memory;
using Titan.Windows.D3D11;

namespace Titan.Assets.NewAssets;



/*
 * Asset Registry
 *  AssetBundle or merge them?
 *
 * 
 */

public interface IAssetLoader<T> where T : unmanaged
{
    static abstract Handle<T> Load(ReadOnlySpan<byte> data);
    static abstract void Unload(Handle<T> handle);
}



public struct AssetHandle<T> where T : unmanaged
{
    public Handle<Asset> Value;
    public Handle<T> Asset;


    public static implicit operator Handle<T>(in AssetHandle<T> assetHandle) => assetHandle.Asset;
}

public unsafe struct AssetsModule : IModule
{
    public static void Build(AppBuilder builder)
    {
        var fileApi = builder.GetResourcePointer<FileSystemApi>();
        var assetConfigs = builder.GetConfigurations<AssetsConfiguration>();


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


    }
}

public struct AssetManagerApi : IApi
{
    // the "public" api to handle assets
    public Handle<Asset> Load(in AssetDescriptor descriptor)
    {
        return Handle<Asset>.Null;
    }

    public void Unload(ref Handle<Asset> handle)
    {

    }

    public bool IsLoaded(in Handle<Asset> handle)
    {
        return true;
    }

    public Handle<T> GetAssetHandle<T>(in Handle<Asset> assetHandle) where T : unmanaged
    {

        return Handle<T>.Null;
    }
}


public enum AssetState
{
    Unloaded,
    LoadRequested,
    ReadingFile,
    Loading,
    Loaded,
    /*
     * Do we need more states?
     */
    UnloadRequested
}
public struct Asset
{
    public AssetState State;
}

internal unsafe struct AssetRegistry
{
    public bool Initialize(Allocator* allocator, FileSystemApi* fileSystem, ReadOnlySpan<AssetsConfiguration> configs)
    {

        var maxAssetCount = 0u;
        foreach (var config in configs)
        {
            maxAssetCount += config.AssetCount;
        }
        //_databases = allocator->Allocate(configs.Length * sizeof(AssetDatabase));

        return true;
    }

    public void Shutdown()
    {

    }

}


internal struct AssetIndex
{
    private nint _fileHandle;
    public static bool Create(string path, out AssetIndex index)
    {
        index = default;

        var handle = File.OpenHandle(path, FileMode.Open, FileAccess.Read, FileShare.None, FileOptions.RandomAccess);
        if (!handle.IsInvalid)
        {
            return false;
        }
        handle.Close();
        index._fileHandle = handle.DangerousGetHandle();

        return true;
    }

    public void Release()
    {
    }
}

internal struct AssetSystem : IStructSystem<AssetSystem>
{
    public static void Init(ref AssetSystem system, in SystemsInitializer init)
    {
        throw new System.NotImplementedException();
    }

    public static void Update(ref AssetSystem system)
    {
        throw new System.NotImplementedException();
    }

    public static bool ShouldRun(in AssetSystem system)
    {
        //NOTE(Jens): add check if there are any assets being processesd
        return true;
    }
}

/*
 *Asset management system (V1, load .titanpak without support for hotreload)
 *
 * Register bundle (this will add the manifest (or titanpak) to the assets registry. (we could use reflection, but that might cause issues with nativeAOT)
 *
 *
 *
 */
