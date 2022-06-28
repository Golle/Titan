using System.Runtime.CompilerServices;
using Titan.ECS.TheNew;

namespace Titan.NewStuff;

internal class ManagedResources
{
    private readonly object[] _resources;
    public ManagedResources(uint maxResourceTypes)
    {
        _resources = new object[maxResourceTypes];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void InitResource<T>(in T value = null) where T : class, new()
    {
        var id = ResourceId.Id<T>();
        _resources[id] = value ?? new T();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasResource<T>() where T : class => _resources[ResourceId.Id<T>()] != null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T GetResource<T>() where T : class, new() => (T)(_resources[ResourceId.Id<T>()] ??= new T());
}
