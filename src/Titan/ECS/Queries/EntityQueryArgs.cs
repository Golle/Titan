using Titan.ECS.Components;

namespace Titan.ECS.Queries;

public readonly ref struct EntityQueryArgs
{
    public readonly ComponentId Include;
    public readonly ComponentId Exclude;
    public readonly uint Entities;

    private EntityQueryArgs(ComponentId include, ComponentId exclude, uint maxEntities)
    {
        Include = include;
        Exclude = exclude;
        Entities = maxEntities;
    }

    public EntityQueryArgs With<T>() where T : unmanaged, IComponent
        => new(Include | ComponentId<T>.Id, Exclude, Entities);

    public EntityQueryArgs Not<T>() where T : unmanaged, IComponent
        => new(Include, Exclude | ComponentId<T>.Id, Entities);

    public EntityQueryArgs MaxEntities(uint count)
        => new(Include, Exclude, count);
}
