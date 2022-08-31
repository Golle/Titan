using Titan.Core;
using Titan.Core.Threading2;
using Titan.FileSystem;

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
    public volatile int ReferenceCount;

    //File loading
    public void* FileBuffer;
    public FileSystemApi* FileSystemApi;
    public FileHandle FileHandle;

    
    public AssetDescriptor Descriptor;
}
