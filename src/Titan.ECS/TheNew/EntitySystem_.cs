using Titan.ECS.Systems;
using Titan.ECS.Worlds;

namespace Titan.ECS.TheNew;

public abstract class EntitySystem_ : BaseSystem
{
    public sealed override void OnUpdate()
    {
        // Execute system for the active world ?
        OnUpdate(null);
    }

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

    public abstract void OnUpdate(World world);

    //NOTE(Jens): This is a wrapper to generate a unique ID for component types.
    private struct ComponentWrapper<T> where T : unmanaged { }
}
