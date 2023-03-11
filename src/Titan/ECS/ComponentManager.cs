using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS;

public readonly struct ComponentManager
{
    private readonly ObjectHandle<ComponentsRegistry> _registry;

    internal ComponentManager(ObjectHandle<ComponentsRegistry> registry)
    {
        _registry = registry;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AddComponent<T>(in Entity entity) where T : unmanaged, IComponent
        => ref _registry.Value.Create<T>(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T AddComponent<T>(in Entity entity, in T value) where T : unmanaged, IComponent
    {
        ref var component = ref AddComponent<T>(entity);
        component = value;
        return ref component;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveComponent<T>(in Entity entity) where T : unmanaged, IComponent =>
        _registry.Value.Remove<T>(entity);
}
