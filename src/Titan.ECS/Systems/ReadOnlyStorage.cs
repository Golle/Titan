using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Systems;

public readonly struct ReadOnlyStorage<T> where T : unmanaged, IComponent
{
    private readonly Components<T> _components;
    internal ReadOnlyStorage(Components<T> components) => _components = components;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T Get(in Entity entity) => ref _components.Get(entity);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _components.Contains( entity);
}
