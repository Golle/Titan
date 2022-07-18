using Titan.Core;
using Titan.ECS.Components;

namespace Titan.ECS.EntitiesNew;

public readonly ref struct EntityFilter
{
    internal readonly ComponentId Include;
    internal readonly ComponentId Exclude;
    private EntityFilter(in ComponentId include, in ComponentId exclude)
    {
        Include = include;
        Exclude = exclude;
    }
    public EntityFilter With<T>() where T : unmanaged, IComponent
        => new(Include | ComponentId<T>.Id, Exclude);

    public EntityFilter Not<T>() where T : unmanaged, IComponent
        => new(Include, Exclude | ComponentId<T>.Id);
}
