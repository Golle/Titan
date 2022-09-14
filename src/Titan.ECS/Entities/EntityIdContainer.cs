using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Memory.Allocators2;

namespace Titan.ECS.Entities;

/// <summary>
/// Next and Return are not synchronized which means that they can never be called at the same time. The results of doing that are undefined.
/// Make sure Next is called during the Update stage and Return is called either in PostUpdate or PreUpdate.
/// </summary>
internal unsafe struct EntityIdContainer
{
    private readonly uint* _ids;
    //private readonly uint _max; // max is not really used since we'll check for 0 when reading the next ID.
    private uint _head;
    private EntityIdContainer(uint* ids, uint max)
    {
        _ids = ids;
        _head = max;
        for (var i = 1u; i < max; ++i)
        {
            _ids[i] = max - i;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Next()
    {
        var index = Interlocked.Decrement(ref _head);
        Debug.Assert(index != 0, "Max number of entities reached.");
        return _ids[index];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Return(uint id) => _ids[Interlocked.Increment(ref _head) - 1] = id;

    public static EntityIdContainer Create(LinearAllocator* allocator, uint maxEntities)
    {
        var ids = allocator->Alloc<uint>(maxEntities, initialize: false);
        return new EntityIdContainer(ids, maxEntities);
    }
}
