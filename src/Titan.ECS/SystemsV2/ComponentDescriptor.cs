using System;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.SystemsV2.Components;
using Titan.ECS.TheNew;

namespace Titan.ECS.SystemsV2;

internal readonly unsafe struct ComponentDescriptor
{
    public readonly ResourceId ResourceId;
    public readonly ComponentId ComponentId;
    public readonly void* ComponentPoolVtbl;
    public readonly uint MaxComponents;
    public ComponentDescriptor(ResourceId resourceId, ComponentId componentId, void* componentPoolVtbl, uint maxComponents)
    {
        ResourceId = resourceId;
        ComponentId = componentId;
        ComponentPoolVtbl = componentPoolVtbl;
        MaxComponents = maxComponents;
    }

    public static ComponentDescriptor Create<T>(ComponentPoolTypes type, uint maxComponents = 0) where T : unmanaged, IComponent =>
        new(
            ResourceId.Id<T>(),
            ComponentId<T>.Id,
            type switch
            {
                ComponentPoolTypes.Packed => PackedComponentPoolNew<T>.Vtbl,
                _ => throw new NotImplementedException($"Component pool for type {type} has not been implemented.")
            },
            maxComponents
        );
}
