using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Jobs;

namespace Titan.Assets;

internal unsafe class AssetsRegistry
{
    private const int HandleOffset = 780003;
    private IMemoryManager _memoryManager;
    private TitanArray<int> _manifests;
    private TitanArray<AssetContext> _assets;

    public bool Init(IMemoryManager memoryManager, AssetsConfiguration[] configs)
    {
        _memoryManager = memoryManager;
        var assetCount = configs.Sum(static c => c.AssetDescriptors.Length);
        var manifestCount = configs.Max(static c => c.Id) + 1; // add 1 so we can index the assets with the manifest ID.

        var alignedManifestSize = MemoryUtils.AlignToUpper(sizeof(int) * manifestCount);
        var alignedAssetsSize = MemoryUtils.AlignToUpper(sizeof(AssetContext) * assetCount);
        var totalSize = alignedManifestSize + alignedAssetsSize;
        Logger.Trace<AssetsRegistry>($"Allocating {totalSize} bytes of memory for the asset registry. Split between {configs.Length} manifests.");

        var mem = (byte*)memoryManager.Alloc(totalSize, true);
        if (mem == null)
        {
            Logger.Error<AssetsRegistry>($"Failed to allocate {totalSize} bytes of memory.");
            return false;
        }

        _manifests = new TitanArray<int>(mem, manifestCount);
        _assets = new TitanArray<AssetContext>(mem + alignedManifestSize, assetCount);

        //NOTE(Jens): Pack everything into the same memory block.
        MemoryUtils.Init(_manifests.GetPointer(), alignedManifestSize, byte.MaxValue);
        var offset = 0;
        foreach (var config in configs)
        {
            _manifests[config.Id] = offset;
            for (var i = 0; i < config.AssetDescriptors.Length; ++i)
            {
                _assets[i + offset] = new AssetContext
                {
                    AssetHandle = Handle.Null,
                    Descriptor = config.AssetDescriptors[i],
                    JobHandle = JobHandle.Invalid,
                    ReferenceCount = 0,
                    State = AssetState.Unloaded
                };
            }
            offset += config.AssetDescriptors.Length;
        }
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<AssetContext> GetAssets() => _assets.AsSpan();
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Handle<Asset> GetHandleFromDescriptor(in AssetDescriptor descriptor)
    {
        Debug.Assert(_manifests[descriptor.ManifestId] != -1, $"Manifest with ID {descriptor.ManifestId} has not been added to the registry");
        Debug.Assert(descriptor.ManifestId <= _manifests.Length, $"Manifest with ID {descriptor.ManifestId} is out of range. Did you register the manifest?");
        return (int)(_manifests[descriptor.ManifestId] + descriptor.Id) + HandleOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLoaded(in Handle<Asset> handle)
        => Get(handle).State == AssetState.Loaded;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref AssetContext Get(in Handle<Asset> handle)
    {
        var index = unchecked(handle - HandleOffset);
        Debug.Assert(index < _assets.Length, "Out of bounds");
        return ref _assets[index];
    }

    public void Shutdown()
    {
        if (_memoryManager != null)
        {
            _memoryManager.Free(ref _manifests);
            _assets = default;
        }
        _memoryManager = null;
    }
}
