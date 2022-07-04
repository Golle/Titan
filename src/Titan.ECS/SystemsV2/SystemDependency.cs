using System;
using Titan.ECS.TheNew;

namespace Titan.ECS.SystemsV2;

internal unsafe struct SystemDependency
{
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
}
