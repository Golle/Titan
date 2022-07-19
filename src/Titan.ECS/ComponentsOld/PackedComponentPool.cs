using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Events;

namespace Titan.ECS.ComponentsOld;

internal unsafe class PackedComponentPool<T> : IComponentPool<T> where T : unmanaged
{
    private readonly ComponentId _componentId = ComponentId<T>.Id;

    private T** _indexers;
    private readonly MemoryChunk<T> _components;
    private readonly MemoryChunk<nint> _indexersChunk;

    private int _numberOfComponents;
    private readonly uint _maxNumberOfComponents;
    private readonly uint _worldId;
    private readonly Queue<nint> _freeComponents = new(); // use type nint because T* cant be stored in the queue

    private bool _componentBeingRemoved;

    public PackedComponentPool(uint numberOfComponents, uint maxEntities, uint worldId)
    {
        _maxNumberOfComponents = numberOfComponents;
        _worldId = worldId;
        _indexersChunk = MemoryUtils.AllocateBlock<nint>(maxEntities, true);
        _components = MemoryUtils.AllocateBlock<T>(numberOfComponents, true);
        _indexers = (T**)_indexersChunk.AsPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Create(in Entity entity, in T initialValue)
    {
        ref var value = ref Create(entity);
        value = initialValue;
        return ref this[entity];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Create(in Entity entity)
    {
        if (_numberOfComponents >= _maxNumberOfComponents && _freeComponents.Count == 0)
        {
            throw new InvalidOperationException($"Maximum components of type {typeof(T)} reached({_numberOfComponents})");
        }

        Logger.Trace<PackedComponentPool<T>>($"Add {typeof(T).Name} to Entity {entity.Id}");
        ref var pComponent = ref _indexers[entity.Id];
        if (pComponent != null)
        {
            throw new InvalidOperationException($"A component of type {typeof(T)} has already been added to this entity {entity.Id}");
        }

        if (_freeComponents.TryDequeue(out var pComp))
        {
            pComponent = (T*)pComp;
        }
        else
        {
            pComponent = _components.GetPointer(Interlocked.Increment(ref _numberOfComponents));
        }

        EventManager.Push(new ComponentAddedEvent(entity, _componentId));
        return ref *pComponent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T CreateOrReplace(in Entity entity, in T value = default)
    {
        var pComponent = _indexers[entity.Id];
        if (pComponent == null)
        {
            ref var component = ref Create(entity);
            component = value;
            return ref component;
        }
        *pComponent = value;
        return ref *pComponent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T Get(in Entity entity)
    {
        ref var pComponent = ref _indexers[entity.Id];
        Debug.Assert(pComponent != null, $"Component of type {typeof(T)} has not been added to the entity.");
        return ref *pComponent;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Contains(in Entity entity) => _indexers[entity.Id] != null;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Destroy(in Entity entity)
    {
        if (_indexers[entity.Id] != null)
        {
            EventManager.Push(new ComponentBeingRemovedEvent(entity, _componentId));
            _componentBeingRemoved = true;
        }
    }

    public ref T this[in Entity entity]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => ref Get(entity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Update()
    {
        if (!_componentBeingRemoved)
        {
            return;
        }

        foreach (ref readonly var @event in EventManager.GetEvents())
        {
            if (@event.Type == ComponentBeingRemovedEvent.Id)
            {
                ref readonly var e = ref @event.As<ComponentBeingRemovedEvent>();
                if (e.Entity.WorldId != _worldId || e.Component != _componentId)
                {
                    continue;
                }

                ref var pComponent = ref _indexers[e.Entity.Id];
                Debug.Assert(pComponent != null, "Trying to destroy a component that has not been created.");
                Logger.Trace<PackedComponentPool<T>>($"Removed {typeof(T).Name} from Entity {e.Entity.Id}");
                _freeComponents.Enqueue((nint)pComponent);
                pComponent = null;
                EventManager.Push(new ComponentRemovedEvent(e.Entity, _componentId));
            }
        }
        _componentBeingRemoved = false;
    }

    public void Dispose()
    {
        _indexersChunk.Free();
        _components.Free();
        _indexers = null;
    }
}