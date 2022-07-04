using System;
using Titan.Core.Events;
using Titan.ECS.Components;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2.Components;
using Titan.ECS.TheNew;

namespace Titan.ECS.SystemsV2;

public readonly unsafe ref struct SystemsInitializer
{
    private readonly SystemDependencyState* _state;
    private readonly IApp _app;

    internal SystemsInitializer(IApp app, SystemDependencyState * state)
    {
        _app = app;
        _state = state;
    }
    public MutableResource<T> GetMutableGlobalResource<T>() where T : unmanaged
    {
        _state->MutableGlobalResources.Add(ResourceId<T>.Id);
        return new(_app.GetMutableResourcePointer<T>());
    }

    public ReadOnlyResource<T> GetReadOnlyGlobalResource<T>() where T : unmanaged
    {
        _state->ReadOnlyGlobalResources.Add(ResourceId.Id<T>());
        return new(_app.GetMutableResourcePointer<T>());
    }

    public MutableResource<T> GetMutableResource<T>() where T : unmanaged
    {
        _state->MutableResources.Add<T>();
        return new(_app.GetMutableResourcePointer<T>());
    }

    public ReadOnlyResource<T> GetReadOnlyResource<T>() where T : unmanaged
    {
        _state->ReadOnlyResources.Add<T>();
        return new(_app.GetMutableResourcePointer<T>());
    }

    public MutableStorage2<T> GetMutableStorage<T>() where T : unmanaged
    {
        _state->MutableComponents |= ComponentId<T>.Id;
        return new(_app.GetMutableResourcePointer<Components<T>>());
    }

    public ReadOnlyStorage2<T> GetReadOnlyStorage<T>() where T : unmanaged
    {
        _state->ReadOnlyComponents |= ComponentId<T>.Id;
        return new(_app.GetMutableResourcePointer<Components<T>>());
    }

    public EventsReader<T> GetEventsReader<T>() where T : unmanaged
        => new(_app.GetMutableResourcePointer<EventCollection<T>>());

    public EventsWriter<T> GetEventsWriter<T>() where T : unmanaged
        => new(_app.GetMutableResourcePointer<EventCollection<T>>());
}
