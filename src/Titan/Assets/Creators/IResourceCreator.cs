using Titan.Core;
using Titan.Core.Memory;

namespace Titan.Assets.Creators;

public interface IResourceCreator<TResourceType> where TResourceType : unmanaged
{
    static abstract AssetDescriptorType Type { get; }
    bool Init(in ResourceCreatorInitializer initializer);
    void Release();
    Handle<TResourceType> Create(in AssetDescriptor descriptor, TitanBuffer data);
    bool Recreate(in Handle<TResourceType> handle, in AssetDescriptor descriptor, TitanBuffer data);
    void Destroy(Handle<TResourceType> handle);
}
