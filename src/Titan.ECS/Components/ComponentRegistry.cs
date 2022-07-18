using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Memory;
using Titan.ECS.Entities;
using Titan.ECS.SystemsV2;

namespace Titan.ECS.Components;

internal unsafe struct ComponentRegistry : IApi
{
    private ComponentsInternal* _components;
    private uint _count;
    public void Init(in MemoryPool pool, uint maxEntities, ReadOnlySpan<ComponentDescriptor> descriptors)
    {
        _count = (uint)descriptors.Length;
        _components = pool.GetPointer<ComponentsInternal>(_count);

        for (var i = 0; i < _count; ++i)
        {
            ref readonly var desc = ref descriptors[i];
            var size = desc.CalculateSize(maxEntities);
            // Allocate memory for the pool based on the calculated size and initialize it.
            var mem = pool.GetPointer(size);
            desc.Init(mem, maxEntities);
            _components[i] = new ComponentsInternal
            {
                ComponentId = desc.ComponentId,
                Data = mem,
                VTable = desc.ComponentPoolVtbl
            };
        }

    }

    public readonly Components<T> Access<T>() where T : unmanaged, IComponent
    {
        var components = FindComponents(ComponentId<T>.Id);
        Debug.Assert(components != null, $"Component of type {typeof(T).Name} has not been registed.");
        return new Components<T>((ComponentPoolVTable<T>*)components->VTable, components->Data);
    }

    public void Destroy(in Entity entity, in ComponentId componentId)
    {
        for (var i = 0; i < _count; ++i)
        {
            ref var components = ref _components[i];
            if (components.ComponentId == componentId)
            {
                components.Destroy(entity);
            }
        }
    }

    public void Destroy(in Entity entity)
    {
        for (var i = 0; i < _count; ++i)
        {
            _components[i].Destroy(entity);
        }
    }

    private readonly ComponentsInternal* FindComponents(in ComponentId id)
    {
        for (var i = 0; i < _count; ++i)
        {
            var comp = _components + i;
            if (comp->ComponentId == id)
            {
                return comp;
            }
        }
        return null;
    }

    private struct ComponentsInternal
    {
        public ComponentId ComponentId;
        public void* VTable;
        public void* Data;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Destroy(in Entity entity) => ((ComponentPoolCommonVTable*)VTable)->Destroy(Data, entity);
    }
}
