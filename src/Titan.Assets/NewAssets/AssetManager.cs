using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Events;

namespace Titan.Assets.NewAssets;


public record struct AssetLoadRequested(Handle<Asset> Handle) : IEvent;
public record struct AssetUnloadRequested(Handle<Asset> Handle) : IEvent;


/// <summary>
/// Public API for handling assets
/// </summary>
public readonly unsafe struct AssetManager : IApi
{
    private readonly AssetRegistry* _registry;
    private readonly EventsWriter<AssetLoadRequested> _assetLoad;
    private readonly EventsWriter<AssetUnloadRequested> _assetUnload;

    internal AssetManager(AssetRegistry* registry, EventsWriter<AssetLoadRequested> assetLoad, EventsWriter<AssetUnloadRequested> assetUnload)
    {
        _registry = registry;
        _assetLoad = assetLoad;
        _assetUnload = assetUnload;
    }

    public Handle<Asset> Load(in AssetDescriptor descriptor)
    {
        var handle = _registry->GetHandleFromDescriptor(descriptor);
        _assetLoad.Send(new AssetLoadRequested(handle));
        return handle;
    }

    public void Unload(ref Handle<Asset> handle)
    {
        if (handle.IsValid())
        {
            _assetUnload.Send(new AssetUnloadRequested(handle));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLoaded(in Handle<Asset> handle)
        => _registry->IsLoaded(handle);

    public Handle<T> GetAssetHandle<T>(in Handle<Asset> assetHandle) where T : unmanaged
    {
        ref readonly var asset = ref _registry->Get(assetHandle);
        Debug.Assert(asset.State == AssetState.Loaded, $"Trying to access an asset that is not loaded. Use {nameof(IsLoaded)}(Handle) to check is an asset is available.");
        return asset.AssetHandle;
    }

    public Handle<T> GetAssetHandleFromDescriptor<T>(in AssetDescriptor descriptor) where T : unmanaged
    {
        //NOTE(Jens): This is a synchronous call to support getting the handle to an asset that has already been loaded. 
        //NOTE(Jens): Need to figure out how to handle reference counting in this case
        //NOTE(Jens): what happens if the asset has not been loaded? just return Invalid?
        throw new NotImplementedException("Implement this to support reading the handle of already loaded asset.");
    }
}
