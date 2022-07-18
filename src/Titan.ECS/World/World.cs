using System;
using Titan.Core;
using Titan.Core.Memory;
using Titan.ECS.Components;
using Titan.ECS.Events;
using Titan.ECS.TheNew;
using EntityFilter = Titan.ECS.EntitiesNew.EntityFilter;

namespace Titan.ECS.World;

public struct World
{
    private ResourceCollection _resources;
    internal void Init(in MemoryPool pool, in ResourceCollection resources)
    {
        _resources = resources;
    }

    public Components<T> GetComponents<T>() where T : unmanaged, IComponent =>
        _resources
            .GetResource<ComponentRegistry>()
            .Access<T>();

    public Events<T> GetEvents<T>() where T : unmanaged, IEvent =>
        _resources
            .GetResource<EventsRegistry>()
            .GetEvents<T>();

    public ref T GetResource<T>() where T : unmanaged, IResource =>
        ref _resources.GetResource<T>();

    public ref T GetApi<T>() where T : unmanaged, IApi =>
        ref _resources.GetResource<T>();

    public bool HasResource<T>() where T : unmanaged
        => _resources.HasResource<T>();


    public unsafe T* GetResourcePointer<T>() where T : unmanaged =>
        _resources.GetResourcePointer<T>();


    public void CreateEntityQuery(in EntityFilter filter)
    {
        throw new NotImplementedException("Need to implement entities first.");
        //GetResource<EntityFilterRegistry>()
        //    .GetOrCreate(filter);
    }
}
