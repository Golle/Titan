#define TRACE_ASSET_LOADING
using System.Diagnostics;
using Titan.Assets.Creators;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;
using Titan.Events;
using Titan.Jobs;

namespace Titan.Assets;

internal unsafe class AssetLoader
{
    private AssetsRegistry _registry;
    private ResourceCreatorRegistry _creatorRegistry;
    private ObjectHandle<IAssetFileReader> _fileReader;
    private IGeneralAllocator _allocator;
    private IJobApi _jobApi;
    private IEventsManager _eventsManager;
    // This is used for optimization, so the system doesn't have to be scheduled if there are no assets.
    private int _assetsInProgress;


    public bool IsActive()
    {
        //NOTE(Jens): used for optimizations, so we can skip scheduling this system if it's not active.
        return _assetsInProgress > 0;
    }

    public bool Init(IMemoryManager memoryManager, AssetsRegistry registry, ResourceCreatorRegistry creatorRegistry, IAssetFileReader fileReader, IJobApi jobApi, IEventsManager eventsManager, uint fileBufferSize)
    {
        Debug.Assert(fileBufferSize > 0);

        if (!memoryManager.TryCreateGeneralAllocator(fileBufferSize, out var allocator))
        {
            Logger.Error<AssetLoader>($"Failed to create an allocator with {fileBufferSize} bytes.");
            return false;
        }

        _allocator = allocator;
        _registry = registry;
        _fileReader = new ObjectHandle<IAssetFileReader>(fileReader);
        _jobApi = jobApi;
        _creatorRegistry = creatorRegistry;
        _eventsManager = eventsManager;
        return true;
    }

    public void Load(Handle<Asset> handle)
    {
        Debug.Assert(handle.IsValid);
        ref var asset = ref _registry.Get(handle);
        //NOTE(Jens): Need to handle more states, for example if it's in UnloadRequested state we should let the unload finish and reload the asset.
        if (asset.State == AssetState.Unloaded)
        {
            //NOTE(Jens): we could use interlocked.compareexchange to start the job when it's requested. (skip the event)
            asset.State = AssetState.LoadRequested;
            _assetsInProgress++;
        }
        if (asset.State is AssetState.UnloadCompleted or AssetState.UnloadRequested or AssetState.Unloading)
        {
            Logger.Warning<AssetLoader>($"The asset {handle.Value} is in state {asset.State} which we're currently not handling.");
        }
        asset.ReferenceCount++;
    }

    public void Reload(Handle<Asset> handle)
    {
        Debug.Assert(handle.IsValid);
        ref var asset = ref _registry.Get(handle);
        if (asset.State != AssetState.Loaded)
        {
            Logger.Warning<AssetLoader>($"The asset {handle.Value} is ina state {asset.State} and can't be reloaded. Only assets in state {AssetState.Loaded} can be reloaded.");
        }
        else
        {
            _assetsInProgress++;
            asset.State = AssetState.ReloadRequested;
        }
    }

    public Handle<T> LoadSynchronous<T>(in AssetDescriptor descriptor) where T : unmanaged
    {
        Debug.Assert(descriptor.GetSize() != 0, "The size of the asset is 0.");

        var handle = _registry.GetHandleFromDescriptor(descriptor);
        if (handle.IsInvalid)
        {
            Logger.Error($"Failed to get asset handle for Asset {descriptor.Id} (ManifestId: {descriptor.ManifestId})");
            return 0;
        }
        var creator = _creatorRegistry.GetPointer(descriptor.Type);
        if (creator == null)
        {
            Logger.Error<AssetLoader>($"No resource creator registered for {descriptor.Type}");
        }

        ref var asset = ref _registry.Get(handle);
        asset.State = AssetState.InlineLoading;

        // Allocate a buffer
        var fileSize = _fileReader.Value.GetSizeFromDescriptor(asset.Descriptor);
        var buffer = _allocator.AllocateBuffer((uint)fileSize);
        Debug.Assert(buffer.HasData());

        // Read the file
        var bytesRead = _fileReader.Value.Read(buffer.AsSpan(), descriptor);
        // Validate the bytes read (maybe this should be an assert?)
        if (bytesRead <= 0)
        {
            Logger.Error<AssetLoader>($"Failed to read the asset {asset.Descriptor.Id} (ManifestId: {asset.Descriptor.ManifestId}). {bytesRead} returned. ({nameof(LoadSynchronous)})");
            _allocator.Free(buffer);
            asset.State = AssetState.Error;
            return 0;
        }

        if (bytesRead != (int)fileSize)
        {
            Logger.Warning<AssetLoader>($"Mismatch in bytes read for asset {descriptor.Id} (ManifestId: {descriptor.ManifestId}). Bytes read = {bytesRead}, File size = {fileSize}. This can happen when an asset is loaded from it's raw state.");
        }

        // create the resource
        asset.AssetHandle = creator->Create(descriptor, buffer.Slice((uint)bytesRead));
        // Free the file buffer
        _allocator.Free(buffer);
        if (asset.AssetHandle.IsInvalid())
        {
            Logger.Error<AssetLoader>($"Failed to create the asset with ID {asset.Descriptor.Id}(ManifestId: {asset.Descriptor.ManifestId}) Type {asset.Descriptor.Type}");
            asset.State = AssetState.Error;
            return 0;
        }
        asset.State = AssetState.Loaded;
        return asset.AssetHandle.Value;
    }

    public void Unload(Handle<Asset> handle)
    {
        Debug.Assert(handle.IsValid);
        ref var asset = ref _registry.Get(handle);
        if (asset.State != AssetState.Loaded)
        {
            Logger.Warning<AssetLoader>($"Trying to unload an asset that's in state {asset.State}. (Expected: {AssetState.Loaded})");
            return;
        }
        var currentRefCount = --asset.ReferenceCount; //Interlocked.Decrement(ref asset.ReferenceCount); 
        if (currentRefCount <= 0)
        {
            asset.State = AssetState.UnloadRequested;
            _assetsInProgress++;
        }
    }

    public void Update()
    {
        foreach (ref var asset in _registry.GetAssets())
        {
            switch (asset.State)
            {
                // async tasks that we just ignore.
                case AssetState.Unloaded:
                case AssetState.ReadingFile:
                case AssetState.CreatingResource:
                case AssetState.Unloading:
                case AssetState.Loaded:
                case AssetState.InlineLoading:
                case AssetState.Reloading:

                    // noop
                    break;
                case AssetState.LoadRequested:
                    Trace($"{asset.State} for Asset: {asset.Descriptor.Id} (Manifest: {asset.Descriptor.ManifestId})");
                    asset.State = AssetState.ReadingFile;
                    //NOTE(Jens): We can move the allocation to the AsyncRead file. but to do that we need to store the allocator on the asset context.
                    asset.FileSize = (uint)_fileReader.Value.GetSizeFromDescriptor(asset.Descriptor);
                    asset.FileBuffer = _allocator.AllocateBuffer(asset.FileSize);
                    asset.FileReader = _fileReader;
                    _jobApi.Enqueue(JobItem.Create(ref asset, &AsyncReadFile));

                    break;
                case AssetState.ReadFileCompleted:
                    Trace($"{asset.State} for Asset: {asset.Descriptor.Id} (Manifest: {asset.Descriptor.ManifestId})");
                    asset.State = AssetState.CreatingResource;
                    //NOTE(Jens): we could do this validation when the Asset is request to load instead.
                    var context = _creatorRegistry.GetPointer(asset.Descriptor.Type);
                    if (context == null)
                    {
                        Logger.Error<AssetLoader>($"No resource creator registered for {asset.Descriptor.Type}");
                        asset.State = AssetState.Error;
                    }
                    else
                    {
                        asset.ResourceContext = context;
                        _jobApi.Enqueue(JobItem.Create(ref asset, &CreateResource));
                    }
                    break;

                case AssetState.ResourceCreationCompleted:
                    Trace($"{asset.State} for Asset: {asset.Descriptor.Id} (Manifest: {asset.Descriptor.ManifestId})");
                    _allocator.Free(ref asset.FileBuffer);
                    asset.FileReader = default;
                    asset.State = AssetState.Loaded;
                    _assetsInProgress--;
                    _eventsManager.Send(new AssetLoadCompleted(asset.AssetHandle.Value, asset.Descriptor));
                    break;
                case AssetState.UnloadRequested:
                    Trace($"{asset.State} for Asset: {asset.Descriptor.Id} (Manifest: {asset.Descriptor.ManifestId})");
                    asset.State = AssetState.Unloading;
                    _jobApi.Enqueue(JobItem.Create(ref asset, &DestroyResource));
                    break;
                case AssetState.UnloadCompleted:
                    Trace($"{asset.State} for Asset: {asset.Descriptor.Id} (Manifest: {asset.Descriptor.ManifestId})");
                    asset.State = AssetState.Unloaded;
                    _assetsInProgress--;
                    //NOTE(Jens): The AssetHandle will not be "valid" since it's been destroyed. Not sure if this is a good idea.
                    _eventsManager.Send(new AssetUnloadCompleted(asset.AssetHandle.Value, asset.Descriptor));
                    break;

                case AssetState.ReloadRequested:
                    Trace($"{asset.State} for Asset: {asset.Descriptor.Id} (Manifest: {asset.Descriptor.ManifestId})");
                    asset.State = AssetState.Reloading;
                    asset.FileSize = (uint)_fileReader.Value.GetSizeFromDescriptor(asset.Descriptor);
                    asset.FileBuffer = _allocator.AllocateBuffer(asset.FileSize);
                    asset.FileReader = _fileReader;
                    asset.ResourceContext = _creatorRegistry.GetPointer(asset.Descriptor.Type);
                    _jobApi.Enqueue(JobItem.Create(ref asset, &AsyncReloadResource));
                    break;
                case AssetState.ResourceRecreationCompleted:
                    Trace($"{asset.State} for Asset: {asset.Descriptor.Id} (Manifest: {asset.Descriptor.ManifestId})");
                    _allocator.Free(ref asset.FileBuffer);
                    asset.FileReader = default;
                    asset.State = AssetState.Loaded;
                    _assetsInProgress--;
                    _eventsManager.Send(new AssetReloadCompleted(asset.AssetHandle.Value, asset.Descriptor));
                    break;

                case AssetState.Error:
                    Logger.Error<AssetLoader>("An asset is in state Error. This will make the runtime fail.");
                    Debug.Fail("Asset in failed state.");
                    break;

                default:
                    Debug.Fail($"The asset state {asset.State} is not beeing handled.");
                    break;
            }
        }
    }

    private static void DestroyResource(ref AssetContext asset)
    {
        Debug.Assert(asset.ResourceContext != null);
        Debug.Assert(asset.AssetHandle.IsValid());
        asset.ResourceContext->Destroy(asset.AssetHandle);
        asset.State = AssetState.UnloadCompleted;
    }

    private static void CreateResource(ref AssetContext asset)
    {
        Debug.Assert(asset.FileBuffer.HasData());
        Debug.Assert(asset.ResourceContext != null);
        var handle = asset.ResourceContext->Create(asset.Descriptor, asset.FileBuffer.Slice(asset.FileSize));
        if (handle.IsInvalid())
        {
            Logger.Error<AssetLoader>($"Failed to create the asset with ID {asset.Descriptor.Id}(ManifestId: {asset.Descriptor.ManifestId}) Type {asset.Descriptor.Type}");
            asset.State = AssetState.Error;
        }
        else
        {
            asset.State = AssetState.ResourceCreationCompleted;
            asset.AssetHandle = handle;
        }
    }

    private static void AsyncReadFile(ref AssetContext asset)
    {
        var buffer = asset.FileBuffer;
        Debug.Assert(buffer.HasData());
        var reader = asset.FileReader.Value;
        var bytesRead = reader.Read(buffer.AsSpan(), asset.Descriptor);
        if (bytesRead <= 0)
        {
            Logger.Error<AssetLoader>($"Failed to read the asset {asset.Descriptor.Id} (ManifestId: {asset.Descriptor.ManifestId}). {bytesRead} returned. ");
            asset.State = AssetState.Error;
            return;
        }
        if (bytesRead != asset.FileSize)
        {
            Logger.Warning<AssetLoader>($"Mismatch in bytes read for asset {asset.Descriptor.Id} (ManifestId: {asset.Descriptor.ManifestId}). Bytes read = {bytesRead}, File size = {asset.FileSize}. This can happen when an asset is loaded from it's raw state.");
            //NOTE(Jens): If there's a difference in the size, update the filesize property on the asset.
            asset.FileSize = (uint)bytesRead;
        }
        asset.State = AssetState.ReadFileCompleted;
    }


    private static void AsyncReloadResource(ref AssetContext asset)
    {
        Debug.Assert(asset.AssetHandle.IsValid());

        var buffer = asset.FileBuffer;
        Debug.Assert(buffer.HasData());
        var reader = asset.FileReader.Value;
        var bytesRead = reader.Read(buffer.AsSpan(), asset.Descriptor);
        if (bytesRead <= 0)
        {
            Logger.Warning<AssetLoader>("Failed to read the file, resetting.");
        }
        else
        {
            var result = asset.ResourceContext->Recreate(asset.AssetHandle, asset.Descriptor, buffer.Slice((uint)bytesRead));
            if (!result)
            {
                Logger.Error<AssetLoader>($"Failed to recreate asset {asset.Descriptor.Id} (ManifestId: {asset.Descriptor.ManifestId})");
            }
        }
        asset.State = AssetState.ResourceRecreationCompleted;
    }

    public void Shutdown()
    {
        _allocator.Release();
        _fileReader.Release();
    }

    [Conditional("TRACE_ASSET_LOADING")]
    private static void Trace(string message) => Logger.Trace<AssetLoader>(message);


}
