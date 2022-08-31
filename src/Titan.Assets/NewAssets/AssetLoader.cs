using System;
using System.Diagnostics;
using System.Threading;
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

    private int _assetsInProgress;
    public bool Init(AssetRegistry* registry, JobApi* jobApi, AssetFileAccessor* fileAccessor)
    {
        Debug.Assert(_jobApi is null);
        Debug.Assert(_fileAccessor is null);
        Debug.Assert(_registry is null);

        if (!BestFitFixedArena.Create(4096, MemoryUtils.MegaBytes(64), out var allocator))
        {
            Logger.Error<AssetLoader>($"Failed to create the {nameof(BestFitFixedArena)}.");
            return false;
        }

        _jobApi = jobApi;
        _registry = registry;
        _allocator = allocator;
        _fileAccessor = fileAccessor;
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
                    _jobApi->Enqueue(JobItem.Create(ref asset, &AsyncLoadAsset));
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
        Logger.Trace<AssetLoader>("Loading asset (NYI)");
        Thread.Sleep(1000);
        Logger.Trace<AssetLoader>("Loading completed (NYI)");
        asset.AssetHandle = 122;

        asset.State = AssetState.LoadingCompleted;
    }
}
