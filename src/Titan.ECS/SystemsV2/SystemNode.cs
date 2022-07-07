using System.Diagnostics;
using Titan.Core.App;
using Titan.Core.Memory;
using Titan.ECS.TheNew;

namespace Titan.ECS.SystemsV2;

internal readonly unsafe struct SystemNode
{
    public readonly int NodeId;
    public readonly void* Instance;
    public readonly delegate*<void*, void> Update;
    public readonly Stage Stage;
    public readonly ResourceId ReourceId;

    public static SystemNode CreateAndInit(int nodeId, in PermanentMemory allocator, in SystemDescriptor descriptor, in SystemsInitializer systemsInitializer)
    {
        var ptr = allocator.GetPointer(descriptor.Size, true);
        Debug.Assert(ptr != null, $"Failed to allocate memory for system with Id: {descriptor.Id}");
        descriptor.Init(ptr, systemsInitializer);
        return new SystemNode(nodeId, ptr, descriptor);
    }

    private SystemNode(int nodeId, void* instance, in SystemDescriptor descriptor)
    {
        NodeId = nodeId;
        ReourceId = descriptor.Id;
        Instance = instance;
        Update = descriptor.Update;
        Stage = descriptor.Stage;
    }
}
