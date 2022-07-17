using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Entities;
using Titan.ECS.SystemsV2.Components;

namespace Titan.ECS.SystemsV2;

public readonly struct ReadOnlyStorage3<T> where T : unmanaged, IComponent
{
    private readonly Components<T> _components;
    internal ReadOnlyStorage3(Components<T> components) => _components = components;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref readonly T Get(in Entity entity) => ref _components.Get(entity);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _components.Contains( entity);
}
