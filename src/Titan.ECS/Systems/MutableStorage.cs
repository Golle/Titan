using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.Systems;

public readonly struct MutableStorage<T> where T : unmanaged, IComponent
{
    private static readonly ComponentId ComponentId = ComponentId<T>.Id;
    private readonly Components<T> _components;
    private readonly EventsWriter<ComponentBeingDestroyed> _writer;
    private readonly EventsWriter<ComponentAdded> _added;

    public MutableStorage(Components<T> components, EventsWriter<ComponentBeingDestroyed> writer, EventsWriter<ComponentAdded> added)
    {
        _components = components;
        _writer = writer;
        _added = added;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(in Entity entity) => ref _components.Get(entity);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _components.Contains(entity);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="initialValue"></param>
    /// <returns>Returns true if the component was Created</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Create(in Entity entity, in T initialValue = default)
    {
        var created = _components.Create(entity, initialValue);
        if (created)
        {
            _added.Send(new ComponentAdded(ComponentId, entity));
        }
        return created;
    }

    /// <summary>
    /// tbd
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="value"></param>
    /// <returns>Returns true if the component was Created</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CreateOrReplace(in Entity entity, in T value = default)
    {
        var created = _components.CreateOrReplace(entity, value);
        if (created)
        {
            _added.Send(new ComponentAdded(ComponentId, entity));
        }
        return created;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy(in Entity entity) => _writer.Send(new ComponentBeingDestroyed(ComponentId, entity));

    /// <summary>
    /// This method should only be called by internal systems since it will bypass any other delete mechanic.
    /// </summary>
    /// <param name="entity"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void DestroyImmediately(in Entity entity) => _components.Destroy(entity);
}
