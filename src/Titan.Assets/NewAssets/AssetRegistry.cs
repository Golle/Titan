using System.Diagnostics;
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
    private TitanArray<int> _manifestOffsets;
    private TitanArray<AssetContext> _assets;
    private MemoryManager* _memoryManager;

    public bool Init(MemoryManager* memoryManager, AssetsConfiguration[] configs)
    {
        Debug.Assert(_manifestOffsets.Length == 0);
        Debug.Assert(_assets.Length == 0);

        var assetCount = configs.Sum(c => c.AssetDescriptors.Length);
        var highestManifestId = configs.Max(c => c.Id);

        Logger.Trace<AssetRegistry>($"Asset count: {assetCount}");
        Logger.Trace<AssetRegistry>($"Manifest count: {configs.Length} (Highest ID: {highestManifestId})");

        //NOTE(Jens): Size of all assets + the maninfest offsets
        var manifestIndexSize = highestManifestId * sizeof(int);
        var totalSize = sizeof(AssetContext) * assetCount + manifestIndexSize;

        var mem = (byte*)memoryManager->Alloc((uint)totalSize, initialize: true);
        Debug.Assert(mem != null, $"Failed to allocate {totalSize} bytes of memory.");
        var manifestOffsets = (int*)mem;
        var assets = (AssetContext*)(mem + manifestIndexSize);
        var offset = 0;

        //NOTE(Jens): Set offsets to -1. This is to detect request with descriptors that have not been registered at startup.
        MemoryUtils.Init(manifestOffsets, manifestIndexSize, byte.MaxValue);
        foreach (var config in configs)
        {
            manifestOffsets[config.Id] = offset;
            for (var i = 0; i < config.AssetDescriptors.Length; ++i)
            {
                assets[i + offset] = new AssetContext
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

        _assets = new TitanArray<AssetContext>(assets, assetCount);
        _manifestOffsets = new TitanArray<int>(manifestOffsets, highestManifestId);
        _memoryManager = memoryManager;
        return true;
    }

    public Handle<Asset> GetHandleFromDescriptor(in AssetDescriptor descriptor)
    {
        Debug.Assert(_manifestOffsets[descriptor.ManifestId] != -1, $"Manifest with ID {descriptor.ManifestId} has not been added to the registry");
        Debug.Assert(descriptor.ManifestId <= _manifestOffsets.Length, $"Manifest with ID {descriptor.ManifestId} is out of range. Did you register the manifest?");

        return (int)(_manifestOffsets[descriptor.ManifestId] + descriptor.Id) + HandleOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLoaded(in Handle<Asset> handle)
        => Get(handle).State == AssetState.Loaded;

    public void Release()
    {
        _memoryManager->Free(_manifestOffsets);
        this = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<AssetContext> GetAssets() => _assets.AsSpan();
    public ref AssetContext Get(in Handle<Asset> handle)
    {
        var index = handle - HandleOffset;
        Debug.Assert(index >= 0 && index < _assets.Length, "Out of bounds");
        return ref _assets[index];
    }
}

