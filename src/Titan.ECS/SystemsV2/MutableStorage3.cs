using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2.Components;

namespace Titan.ECS.SystemsV2;

public readonly struct MutableStorage3<T> where T : unmanaged, IComponent
{
    private static readonly ComponentId ComponentId = ComponentId<T>.Id;
    private readonly Components<T> _components;
    private readonly EventsWriter<ComponentDestroyed> _writer;
    public MutableStorage3(Components<T> components, EventsWriter<ComponentDestroyed> writer)
    {
        _components = components;
        _writer = writer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(in Entity entity) => ref _components.Get(entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _components.Contains(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Create(in Entity entity, in T initialValue = default) => ref _components.Create(entity, initialValue);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T CreateOrReplace(in Entity entity, in T value = default) => ref _components.CreateOrReplace(entity, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy(in Entity entity) => _writer.Send(new ComponentDestroyed(ComponentId, entity));

    /// <summary>
    /// This method should only be called by internal systems since it will bypass any other delete mechanic.
    /// </summary>
    /// <param name="entity"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void DestroyImmediately(in Entity entity) => _components.Destroy(entity);
}
