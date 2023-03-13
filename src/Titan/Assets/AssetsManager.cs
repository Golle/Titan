using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Events;

namespace Titan.Assets;

public readonly struct AssetsManager
{
    private readonly ObjectHandle<AssetsRegistry> _registry;
    private readonly EventsWriter<AssetLoadRequested> _load;
    private readonly EventsWriter<AssetUnloadRequested> _unload;
    private readonly EventsWriter<AssetReloadRequested> _reload;

    internal AssetsManager(ObjectHandle<AssetsRegistry> registry, EventsWriter<AssetLoadRequested> load, EventsWriter<AssetUnloadRequested> unload, EventsWriter<AssetReloadRequested> reload)
    {
        _registry = registry;
        _load = load;
        _unload = unload;
        _reload = reload;
    }

    public Handle<Asset> Load(in AssetDescriptor descriptor)
    {
        var handle = _registry.Value.GetHandleFromDescriptor(descriptor);
        _load.Send(new AssetLoadRequested(handle));
        return handle;
    }

    public void Reload(Handle<Asset> handle) => _reload.Send(new AssetReloadRequested(handle));
    public void Unload(Handle<Asset> handle) => _unload.Send(new AssetUnloadRequested(handle));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLoaded(Handle<Asset> handle) => _registry.Value.IsLoaded(handle);

    public Handle<T> GetAssetHandle<T>(Handle<Asset> handle) where T : unmanaged
    {
        ref readonly var asset = ref _registry.Value.Get(handle);
        Debug.Assert(asset.State == AssetState.Loaded, $"Trying to access an asset that is not loaded. Use {nameof(IsLoaded)}({nameof(Handle<Asset>)}) to check fs an asset is available.");
        return (Handle<T>)asset.AssetHandle;
    }
}
