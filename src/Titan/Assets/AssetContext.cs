using Titan.Assets.Creators;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Jobs;

namespace Titan.Assets;

/// <summary>
/// Struct that contains all information needed for assets.
/// NOTE(Jens): this will consume a bit of memory depending on the amount of assets in the game. Might be fine, but good to keep an eye on. 
/// </summary>
internal unsafe struct AssetContext
{

    public AssetState State;
    public JobHandle JobHandle;
    public Handle AssetHandle;
    public int ReferenceCount;

    //File loading
    public TitanBuffer FileBuffer;
    public uint FileSize;
    public ObjectHandle<IAssetFileReader> FileReader;
    //public AssetFileAccessor* FileAccessor;

    // Asset creation
    public ResourceContext* ResourceContext;
    public AssetDescriptor Descriptor;

    
}
