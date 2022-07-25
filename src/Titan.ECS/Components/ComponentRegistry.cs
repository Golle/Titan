using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS.Entities;
using Titan.Memory;

namespace Titan.ECS.Components;

internal unsafe struct ComponentRegistry : IApi
{
    private ComponentsInternal* _components;
    private uint _count;
    public void Init(in PlatformAllocator allocator, uint maxEntities, ReadOnlySpan<ComponentDescriptor> descriptors)
    {
        Logger.Warning<ComponentRegistry>("The allocations for the components are not memory aligned, this needs to be fixed.");
        _count = (uint)descriptors.Length;

        var internalsSize = (nuint)(sizeof(ComponentsInternal) * _count);
        var componentSize = CalculateComponentSize(descriptors, maxEntities); // TODD: Not aligned
        var mem = allocator.Allocate(internalsSize + componentSize);
        _components = (ComponentsInternal*)mem;
        var components = (byte*)(_components + _count);

        for (var i = 0; i < _count; ++i)
        {
            ref readonly var desc = ref descriptors[i];
            desc.Init(components, maxEntities);
            _components[i] = new ComponentsInternal
            {
                ComponentId = desc.ComponentId,
                Data = components,
                VTable = desc.ComponentPoolVtbl
            };
            // move the pointer to the next block
            components += desc.CalculateSize(maxEntities); // Align this move.
        }

        static nuint CalculateComponentSize(ReadOnlySpan<ComponentDescriptor> descriptors, uint maxEntities)
        {
            nuint size = 0u;
            foreach (var descriptor in descriptors)
            {
                size += descriptor.CalculateSize(maxEntities);
            }
            return size;
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


    public void Release(in PlatformAllocator allocator)
    {
        allocator.Free(_components);
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
