using System;
using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Threading2;
using Titan.Memory.Arenas;

namespace Titan.Assets.NewAssets;

internal unsafe struct AssetLoader : IResource
{
    private BestFitFixedArena _allocator;
    private AssetRegistry* _registry;
    private JobApi* _jobApi;
    private AssetFileAccessor* _fileAccessor;
    private ResourceCreatorRegistry* _creatorRegistry;

    private int _assetsInProgress;
    public bool Init(AssetRegistry* registry, JobApi* jobApi, AssetFileAccessor* fileAccessor, ResourceCreatorRegistry* creatorRegistry)
    {
        Debug.Assert(_jobApi is null);
        Debug.Assert(_fileAccessor is null);
        Debug.Assert(_registry is null);
        Debug.Assert(_creatorRegistry is null);

        if (!BestFitFixedArena.Create(4096, MemoryUtils.MegaBytes(64), out var allocator))
        {
            Logger.Error<AssetLoader>($"Failed to create the {nameof(BestFitFixedArena)}.");
            return false;
        }

        _jobApi = jobApi;
        _registry = registry;
        _allocator = allocator;
        _fileAccessor = fileAccessor;
        _creatorRegistry = creatorRegistry;
        return true;
    }

    public bool RequiresUpdate() => _assetsInProgress > 0;
    public void Load(in Handle<Asset> handle)
    {
        Debug.Assert(handle.IsValid());
        ref var asset = ref _registry->Get(handle);
        //NOTE(Jens): Need to handle more states, for example if it's in UnloadRequested state we should let the unload finish and reload the asset.
        if (asset.State == AssetState.Unloaded)
        {
            //NOTE(Jens): we could use interlocked.compareexchange to start the job when it's requested. (skip the event)
            asset.State = AssetState.LoadRequested;
            _assetsInProgress++;
        }
        //NOTE(Jens): Load and Unload will be called from AssetSystem after eachother. If this changes we can use Interlocked Increment/Decrement to do the reference counting.
        //Interlocked.Increment(ref asset.ReferenceCount);
        asset.ReferenceCount++;
    }

    public void Unload(in Handle<Asset> handle)
    {
        Debug.Assert(handle.IsValid());
        ref var asset = ref _registry->Get(handle);
        if (asset.State != AssetState.Loaded)
        {
            Logger.Warning<AssetLoader>($"Trying to unload an asset that's in state {asset.State}. (Expected: {AssetState.Loaded})");
            return;
        }
        var currentRefCount = --asset.ReferenceCount; //Interlocked.Decrement(ref asset.ReferenceCount); 
        if (currentRefCount <= 0)
        {
            asset.State = AssetState.UnloadRequested;
        }
    }

    public void Update()
    {
        //NOTE(Jens): If there's a lot of assets it might be faster to have an array/queue of "active" items. Could be pointers or indexes (indexes will be half the size)
        foreach (ref var asset in _registry->GetAssets())
        {
            //NOTE(Jens): could extract each state to functions and for them to not be inlined, might improve performance since most functions wont be called in an update. 
            switch (asset.State)
            {
                case AssetState.Loaded:
                case AssetState.Unloaded:
                case AssetState.Loading:
                case AssetState.ReadingFile:
                case AssetState.Unloading:
                    //we don't care about these states
                    break;

                case AssetState.LoadRequested:
                    // start read file
                    asset.State = AssetState.ReadingFile;
                    asset.FileBuffer = _allocator.Allocate((nuint)asset.Descriptor.GetSize());
                    asset.FileAccessor = _fileAccessor;
                    _jobApi->Enqueue(JobItem.Create(ref asset, &AsyncReadFile));
                    break;

                case AssetState.ReadFileCompleted:
                    asset.State = AssetState.Loading;
                    asset.ResourceContext = _creatorRegistry->Get(asset.Descriptor.Type);
                    if (asset.ResourceContext == null)
                    {
                        Logger.Error<AssetLoader>($"No loader found for {asset.Descriptor.Type}");
                        asset.State = AssetState.Error;
                    }
                    else
                    {
                        _jobApi->Enqueue(JobItem.Create(ref asset, &AsyncLoadAsset));
                    }
                    break;

                case AssetState.LoadingCompleted:
                    if (asset.FileBuffer != null)
                    {
                        _allocator.Free(asset.FileBuffer);
                        asset.FileBuffer = null;
                        asset.FileAccessor = null;
                    }
                    asset.State = AssetState.Loaded;
                    _assetsInProgress--;
                    break;


                case AssetState.UnloadRequested:
                    //NOTE(Jens): implement deferred releases (maybe have a counter and a different state?)
                    _jobApi->Enqueue(JobItem.Create(ref asset, &AsyncUnloadAsset));
                    asset.State = AssetState.Unloading;
                    break;

                case AssetState.UnloadCompleted:
                    asset.State = AssetState.Unloaded;
                    break;
                case AssetState.Error:
                    Logger.Error<AssetLoader>($"Asset {asset.Descriptor.Id} (Manifest: {asset.Descriptor.ManifestId}) is in an Error state. (Not sure how to handle this. retry count?)");
                    _assetsInProgress--;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private static void AsyncReadFile(ref AssetContext asset)
    {
        var fileSize = asset.Descriptor.GetSize();
        var buffer = new Span<byte>(asset.FileBuffer, (int)fileSize);
        var result = asset.FileAccessor->Read(buffer, asset.Descriptor);
        if (result != buffer.Length)
        {
            Logger.Error<AssetLoader>($"The size of the buffer and what was read does not match. Buffer: {buffer.Length} bytes. Bytes read: {result} bytes");
            asset.State = AssetState.Error;
        }
        else
        {
            asset.State = AssetState.ReadFileCompleted;
        }
    }


    private static void AsyncLoadAsset(ref AssetContext asset)
    {
        var dataSize = asset.Descriptor.GetSize();
        var buffer = new ReadOnlySpan<byte>(asset.FileBuffer, (int)dataSize);
        asset.AssetHandle = asset.ResourceContext->Create(buffer);
        if (asset.AssetHandle.IsInvalid())
        {
            Logger.Error<AssetLoader>($"An invalid handle was returned for type {asset.Descriptor.Type} with asset ID {asset.Descriptor.Id} (ManifestID: {asset.Descriptor.ManifestId})");
            asset.State = AssetState.Error;
        }
        else
        {
            asset.State = AssetState.LoadingCompleted;
        }
    }

    private static void AsyncUnloadAsset(ref AssetContext asset)
    {
        asset.ResourceContext->Destroy(asset.AssetHandle);
        asset.State = AssetState.UnloadCompleted;
    }

}
