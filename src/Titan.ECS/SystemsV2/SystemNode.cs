using System.Diagnostics;
using Titan.Core.App;
using Titan.Core.Memory;
using Titan.ECS.TheNew;

namespace Titan.ECS.SystemsV2;

internal readonly unsafe struct SystemNode
{
    private readonly void* _instance;
    private readonly delegate*<void*, void> _update;
    public readonly ResourceId Id;
    public readonly Stage Stage;

    public static SystemNode CreateAndInit(in PermanentMemory allocator, in SystemDescriptor descriptor, in SystemsInitializer systemsInitializer)
    {
        var ptr = allocator.GetPointer(descriptor.Size, true);
        Debug.Assert(ptr != null, $"Failed to allocate memory for system with Id: {descriptor.Id}");
        descriptor.Init(ptr, systemsInitializer);
        return new SystemNode(ptr, descriptor);
    }

    private SystemNode(void* instance, in SystemDescriptor descriptor)
    {
        Id = descriptor.Id;
        _instance = instance;
        _update = descriptor.Update;
        Stage = descriptor.Stage;
    }
}
