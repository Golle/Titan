using System;
using Titan.ECS.Worlds;

namespace Titan.ECS.Scheduler;

internal unsafe struct SystemDependency
{
    //NOTE(Jens): the current max is 10 dependencies, this can be increased if needed. This is only used during setup and will only be on the stack so it wont really matter.
    private const int MaxReosurceDependencies = 10;
#pragma warning disable CS0649
    private fixed uint _dependencies[MaxReosurceDependencies];
#pragma warning restore CS0649
    private int _count;
    public SystemDependency()
    {
        _count = 0;
    }

    public void Add<T>() 
        => Add(ResourceId.Id<T>());
    public void Add(ResourceId id)
    {
        if (!Exists(id))
        {
            _dependencies[_count++] = id;
        }
    }

    private bool Exists(in ResourceId id)
    {
        for (var i = 0; i < _count; ++i)
        {
            if (_dependencies[i] == id)
            {
                return true;
            }
        }
        return false;
    }

    public readonly ReadOnlySpan<ResourceId> GetDependencies()
    {
        fixed (uint* pDeps = _dependencies)
        {
            return new(pDeps, _count);
        }
    }

    public readonly bool ContainsAny(in SystemDependency system)
    {
        foreach (var outer in GetDependencies())
        {
            foreach (var inner in system.GetDependencies())
            {
                if (outer == inner)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
