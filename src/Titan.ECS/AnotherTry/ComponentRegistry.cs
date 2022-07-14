using System;
using System.Diagnostics;
using Titan.Core;
using Titan.Core.Memory;
using Titan.ECS.Components;
using Titan.ECS.SystemsV2;
using Titan.ECS.SystemsV2.Components;

namespace Titan.ECS.AnotherTry;

internal unsafe struct ComponentRegistry
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
        var components = FindComponents<T>();
        Debug.Assert(components != null, $"Component of type {typeof(T).Name} has not been registed.");
        return new Components<T>((ComponentPoolVTable<T>*)components->VTable, components->Data);
    }

    private readonly ComponentsInternal* FindComponents<T>() where T : unmanaged, IComponent
    {
        var id = ComponentId<T>.Id;
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
    }
}
