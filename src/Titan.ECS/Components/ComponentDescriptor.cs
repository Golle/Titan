using System;
using Titan.Core;

namespace Titan.ECS.Components;

internal readonly unsafe struct ComponentDescriptor
{
    public readonly ComponentId ComponentId;
    public readonly void* ComponentPoolVtbl;
    public readonly uint MaxComponents;
    private readonly delegate*<void*, uint, uint, void*> _init;
    private readonly delegate*<uint, uint, uint> _calculateSize;
    internal uint CalculateSize(uint maxEntities) => _calculateSize(maxEntities, MaxComponents);
    internal void* Init(void* mem, uint maxEntities) => _init(mem, maxEntities, MaxComponents);
    private ComponentDescriptor(ComponentId componentId, void* componentPoolVtbl, uint maxComponents, delegate*<void*, uint, uint, void*> init, delegate*<uint, uint, uint> calculateSize)
    {
        ComponentId = componentId;
        ComponentPoolVtbl = componentPoolVtbl;
        MaxComponents = maxComponents;
        _init = init;
        _calculateSize = calculateSize;
    }

    public static ComponentDescriptor Create<T>(ComponentPoolTypes type, uint maxComponents = 0) where T : unmanaged, IComponent
    {
        var vtbl = type switch
        {
            ComponentPoolTypes.Packed => Components.PackedComponentPool<T>.Vtbl,
            _ => throw new NotImplementedException($"Component pool for type {type} has not been implemented.")
        };

        return new(
            ComponentId<T>.Id,
            vtbl,
            maxComponents,
            vtbl->Init,
            vtbl->CalculateSize
        );
    }
}
