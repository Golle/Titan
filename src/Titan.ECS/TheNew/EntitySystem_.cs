using Titan.ECS.Systems;

namespace Titan.ECS.TheNew;

public abstract class EntitySystem_ : BaseSystem
{
    internal sealed override void Init(World_ world)
    {
        base.Init(world);
        OnWorldInit(world);
    }

    internal sealed override void Teardown(World_ world)
    {
        base.Teardown(world);
        OnWorldTeardown(world);
    }

    protected virtual void OnWorldInit(World_ world){}
    protected virtual void OnWorldTeardown(World_ world){}

    protected ReadOnlyStorage<T> GetReadOnly<T>() where T : unmanaged
    {
        AddReadonly<ComponentWrapper<T>>();
        return new();
    }

    protected MutableStorage<T> GetMutable<T>() where T : unmanaged
    {
        AddMutable<ComponentWrapper<T>>();
        return new();
    }

    //NOTE(Jens): This is a wrapper to generate a unique ID for component types.
    private struct ComponentWrapper<T> where T : unmanaged { }
}
