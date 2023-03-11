#nullable disable
using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;
using Titan.Resources;

namespace Titan.Assets.Creators;

internal unsafe class ResourceCreatorRegistry
{
    private TitanArray<ResourceContext> _contexts;
    private ILinearAllocator _allocator;

    public bool Init(IMemoryManager memoryManager, ResourceCreatorConfiguration[] configs, ResourceCollection resourceCollection)
    {
        const int count = (int)AssetDescriptorType.Count;

        var totalSize = count * MemoryUtils.AlignToUpper(sizeof(ResourceContext))
                         + configs.Sum(static c => MemoryUtils.AlignToUpper(c.Descriptor.ContextSize));
        Logger.Trace<ResourceCreatorRegistry>($"Allocating {totalSize} bytes of memory for the resource creators.");

        if (!memoryManager.TryCreateLinearAllocator(AllocatorStrategy.Permanent, (uint)totalSize, out _allocator))
        {
            Logger.Error<ResourceCreatorRegistry>($"Failed to create an allocator with {totalSize} bytes.");
            return false;
        }
        _contexts = _allocator.AllocArray<ResourceContext>(count, true);

        if (!_contexts.IsValid)
        {
            Logger.Error<ResourceCreatorRegistry>($"Failed to allocate memory for {count} {nameof(ResourceContext)}s");
            return false;
        }

        foreach (var config in configs)
        {
            var index = (int)config.Descriptor.Type;
            Debug.Assert(index is >= 0 and < count);
            var descriptor = config.Descriptor;
            Debug.Assert(descriptor.ContextSize > 0);
            var context = _allocator.Alloc(descriptor.ContextSize, true);
            if (context == null)
            {
                Logger.Error<ResourceCreatorRegistry>($"Failed to allocate {descriptor.ContextSize} bytes for the Resource context");
                goto Error;
            }

            if (!descriptor.Init(context, new ResourceCreatorInitializer(resourceCollection)))
            {
                Logger.Error<ResourceCreatorRegistry>($"Failed to init the resource creator for type  {descriptor.Type}");
                goto Error;
            }
            _contexts[index] = new ResourceContext(context, descriptor);
        }
        return true;

Error:
        Shutdown();
        return false;
    }

    public void Shutdown()
    {
        if (_allocator != null)
        {
            foreach (ref var context in _contexts.AsSpan())
            {
                if (context.IsValid())
                {
                    context.Release();
                }
            }
            _allocator.Destroy();
            _allocator = null;
            _contexts = default;
        }
    }

    public ResourceContext* GetPointer(AssetDescriptorType type)
    {
        var context = _contexts.GetPointer((uint)type);
        Debug.Assert(context != null && context->IsValid(), $"No Creator has been registered for type {type}");
        return context;
    }
}
