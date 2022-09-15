using System.Diagnostics;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Memory;

namespace Titan.Assets.NewAssets;

public unsafe struct ResourceCreatorRegistry : IResource
{
    private TitanArray<ResourceContext> _resourceContexts;
    private MemoryManager* _memoryManager;
    internal bool Init(MemoryManager* memoryManager, uint maxResourceContexts)
    {
        Debug.Assert(_memoryManager == null);
        Debug.Assert(_resourceContexts.Length == 0);
        var resourceContexts = memoryManager->AllocArray<ResourceContext>(maxResourceContexts, initialize: true);
        if (resourceContexts.Length == 0)
        {
            Logger.Error<ResourceCreatorRegistry>($"Failed to allocate {maxResourceContexts} {nameof(ResourceContext)}.");
            return false;
        }
        _memoryManager = memoryManager;
        _resourceContexts = resourceContexts;
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

    internal ResourceContext* Get(AssetDescriptorType type)
        => _resourceContexts.GetPointer((uint)type);

    internal void Release()
    {
        _memoryManager->Free(_resourceContexts);
    }
}
