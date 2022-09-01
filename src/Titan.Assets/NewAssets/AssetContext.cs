using System;
using Titan.Core;
using Titan.Core.Threading2;

namespace Titan.Assets.NewAssets;

/// <summary>
/// Struct that contains all information needed for assets.
/// NOTE(Jens): this will consume a bit of memory depending on the amount of assets in the game. Might be fine, but good to keep an eye on. 
/// </summary>
internal unsafe struct AssetContext
{

    public AssetState State;
    public Handle<JobApi> JobHandle;
    public Handle AssetHandle;
    public int ReferenceCount;

    //File loading
    public void* FileBuffer;
    public AssetFileAccessor* FileAccessor;

    // Asset creation
    public ResourceContext* ResourceContext;

    public AssetDescriptor Descriptor;
}

public unsafe struct ResourceContext
{
    private void* _context;
    private delegate*<void*, ReadOnlySpan<byte>, Handle> _create;
    private delegate*<void*, in Handle, void> _destroy;
    public Handle Create(ReadOnlySpan<byte> data) => _create(_context, data);
    public void Destroy(in Handle handle) => _destroy(_context, handle);

    public bool IsInitialized() => _create != null && _destroy != null;
    public static ResourceContext Create<TResourceType, TCreatorType>(void* context)
        where TResourceType : unmanaged
        where TCreatorType : unmanaged, IResourceCreator<TResourceType> =>
        new()
        {
            _context = context,
            _create = &FunctionWrapper<TResourceType, TCreatorType>.Create,
            _destroy = &FunctionWrapper<TResourceType, TCreatorType>.Destroy
        };

    private struct FunctionWrapper<TResourceType, TCreatorType>
        where TResourceType : unmanaged
        where TCreatorType : unmanaged, IResourceCreator<TResourceType>
    {
        public static Handle Create(void* context, ReadOnlySpan<byte> buffer) => TCreatorType.Create(context, buffer).Value;
        public static void Destroy(void* context, in Handle handle) => TCreatorType.Destroy(context, handle);
    }
}
