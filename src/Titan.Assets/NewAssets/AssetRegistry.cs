using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Threading2;
using Titan.Memory;

namespace Titan.Assets.NewAssets;

internal unsafe struct AssetRegistry : IResource
{
    //NOTE(Jens): the handle offset is to allow 0 indexed arrays. Handle<T>.IsInvalid is checking for 0. We could also leave index 0 and have a 1 indexed array. That would remove the addition/subtraction needed to access the assets.
    private const int HandleOffset = 70000;
    private int* _manifestOffsets;
    private Asset* _assets;
    private int _size;
    private uint _highestManifestId;

    public static bool Create(PlatformAllocator* allocator, AssetsConfiguration[] configs, out AssetRegistry registry)
    {
        var assetCount = configs.Sum(c => c.AssetDescriptors.Length);
        var highestManifestId = configs.Max(c => c.Id);

        Logger.Trace<AssetRegistry>($"Asset count: {assetCount}");
        Logger.Trace<AssetRegistry>($"Manifest count: {configs.Length} (Highest ID: {highestManifestId})");

        //NOTE(Jens): Size of all assets + the maninfest offsets
        var manifestIndexSize = highestManifestId * sizeof(int);
        var totalSize = sizeof(Asset) * assetCount + manifestIndexSize;

        var mem = (byte*)allocator->Allocate((nuint)totalSize, initialize: true);
        Debug.Assert(mem != null, $"Failed to allocate {totalSize} bytes of memory.");
        var manifestOffsets = (int*)mem;
        var assets = (Asset*)(mem + manifestIndexSize);
        var offset = 0;
         
        //NOTE(Jens): Set offsets to -1. This is to detect request with descriptors that have not been registered at startup.
        MemoryUtils.Init(manifestOffsets, manifestIndexSize, byte.MaxValue);
        foreach (var config in configs)
        {
            manifestOffsets[config.Id] = offset;
            for (var i = 0; i < config.AssetDescriptors.Length; ++i)
            {
                assets[i + offset] = new Asset
                {
                    AssetHandle = Handle.Null,
                    Descriptor = config.AssetDescriptors[i],
                    JobHandle = Handle<JobApi>.Null,
                    ReferenceCount = 0,
                    State = AssetState.Unloaded
                };
            }
            offset += config.AssetDescriptors.Length;
        }

        registry = new AssetRegistry
        {
            _size = assetCount,
            _manifestOffsets = manifestOffsets,
            _assets = assets,
            _highestManifestId = highestManifestId
        };
        return true;
    }

    public Handle<Asset> GetHandleFromDescriptor(in AssetDescriptor descriptor)
    {
        Debug.Assert(_manifestOffsets[descriptor.ManifestId] != -1, $"Manifest with ID {descriptor.ManifestId} has not been added to the registry");
        Debug.Assert(descriptor.ManifestId <= _highestManifestId, $"Manifest with ID {descriptor.ManifestId} is out of range. Did you register the manifest?");

        return (int)(_manifestOffsets[descriptor.ManifestId] + descriptor.Id) + HandleOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLoaded(in Handle<Asset> handle)
        => Get(handle).State == AssetState.Loaded;

    public void Release()
    {
        //NOTE(Jens): not sure if we want to free the memory or not.
    }

    public ref Asset Get(in Handle<Asset> handle)
    {
        var index = handle - HandleOffset;
        Debug.Assert(index >= 0 && index < _size, "Out of bounds");
        return ref _assets[index];
    }
}

