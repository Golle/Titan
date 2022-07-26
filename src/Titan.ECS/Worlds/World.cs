using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Events;

namespace Titan.ECS.Worlds;

public struct World
{
    private ResourceCollection _resources;
    internal static World Create(in ResourceCollection resources) =>
        new()
        {
            _resources = resources
        };

    public Components<T> GetComponents<T>() where T : unmanaged, IComponent =>
        _resources
            .GetResource<ComponentRegistry>()
            .Access<T>();


    public EventsReader<T> GetEventReader<T>() where T : unmanaged, IEvent =>
        _resources
            .GetResource<EventsRegistry>()
            .GetReader<T>();

    public EventsWriter<T> GetEventWriter<T>() where T : unmanaged, IEvent =>
        _resources
            .GetResource<EventsRegistry>()
            .GetWriter<T>();

    public ref T GetResource<T>() where T : unmanaged, IResource =>
        ref _resources.GetResource<T>();

    public ref T GetApi<T>() where T : unmanaged, IApi =>
        ref _resources.GetResource<T>();

    public bool HasResource<T>() where T : unmanaged
        => _resources.HasResource<T>();


    public unsafe T* GetResourcePointer<T>() where T : unmanaged =>
        _resources.GetResourcePointer<T>();


    public EntityFilter CreateEntityFilter(in EntityFilterConfig config) =>
        _resources.GetResource<EntityFilterRegistry>()
            .GetOrCreate(config);
}
