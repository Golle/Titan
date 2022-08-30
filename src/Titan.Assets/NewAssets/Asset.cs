using Titan.Core;
using Titan.Core.Threading2;

namespace Titan.Assets.NewAssets;

public struct Asset
{
    public AssetState State;

    public Handle<JobApi> JobHandle;
    public Handle AssetHandle; 
    public int ReferenceCount;

    public AssetDescriptor Descriptor;
}
