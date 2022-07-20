using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Memory;

namespace Titan.ECS.Entities;

internal readonly unsafe struct EntityInfoRegistry : IResource
{
    private readonly EntityInfo* _info;
    private readonly int _count;

    public EntityInfoRegistry(EntityInfo* info, uint count)
    {
        _info = info;
        _count = (int)count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref EntityInfo Get(in Entity entity)
    {
        Debug.Assert(entity < _count, "Entity ID is out of range");
        return ref _info[entity];
    }

    public static EntityInfoRegistry Create(in MemoryPool pool, uint maxEntities)
    {
        var info = pool.GetPointer<EntityInfo>(maxEntities, initialize: true);
        return new EntityInfoRegistry(info, maxEntities);
    }
}
