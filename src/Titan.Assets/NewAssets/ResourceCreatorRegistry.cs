using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Memory;

namespace Titan.Assets.NewAssets;

public unsafe struct ResourceCreatorRegistry : IResource
{
    private TitanArray<ResourceContext> _resourceContexts;
    private PlatformAllocator* _allocator;
    public bool Init(PlatformAllocator* allocator, uint maxResourceCreators)
    {
        Debug.Assert(_allocator == null);
        Debug.Assert(_resourceContexts.Length == 0);
        var mem = allocator->Allocate<ResourceContext>(maxResourceCreators, initialize: true);
        if (mem == null)
        {
            Logger.Error<ResourceCreatorRegistry>($"Failed to allocate {maxResourceCreators} {nameof(ResourceContext)}.");
            return false;
        }
        _allocator = allocator;
        _resourceContexts = new TitanArray<ResourceContext>(mem, maxResourceCreators);
        return true;
    }

    public bool Register<TResourceType, TCreatorType>(AssetDescriptorType type, void* context) where TResourceType : unmanaged where TCreatorType : unmanaged, IResourceCreator<TResourceType>
    {
        ref var resourceContext = ref _resourceContexts[(int)type];
        if (resourceContext.IsInitialized())
        {
            Logger.Error<ResourceCreatorRegistry>($"A creator for {type} has already been registered.");
            return false;
        }
        resourceContext = ResourceContext.Create<TResourceType, TCreatorType>(context);
        return true;
    }

    public ResourceContext* Get(AssetDescriptorType type)
        => _resourceContexts.GetPointer((uint)type);
    public void Release()
    {
        _allocator->Free(_resourceContexts);
    }
}
