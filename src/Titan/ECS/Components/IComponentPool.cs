using Titan.Core.Memory.Allocators;
using Titan.ECS.Entities;

namespace Titan.ECS.Components;

internal unsafe interface IComponentPool
{
    bool Init(ILinearAllocator allocator, in PoolConfig config);
    void* Create(in Entity entity);
    void Destroy(in Entity entity);
    void* Access(in Entity entity);
    bool Contains(in Entity entity);
}
