using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Events;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;

namespace Titan.ECS.Scheduler;

public readonly unsafe ref struct SystemsInitializer
{
    private readonly SystemDependencyState* _state;
    private readonly World* _world;

    internal SystemsInitializer(ref SystemDependencyState state, ref World world)
    {
        _world = (World*)Unsafe.AsPointer(ref world);
        _state = (SystemDependencyState*)Unsafe.AsPointer(ref state);
    }

    public EntityHandler GetEntityHandler()
        => new(
            GetEventsWriter<EntityCreated>(),
            GetEventsWriter<EntityBeingDestroyed>(),
            _world->GetResourcePointer<EntityIdContainer>()
        );

    public MutableResource<T> GetMutableResource<T>(bool track = true) where T : unmanaged, IResource
    {
        if (track)
        {
            _state->MutableResources.Add<T>();
        }
        return new(_world->GetResourcePointer<T>());
    }

    public ReadOnlyResource<T> GetReadOnlyResource<T>(bool track = true) where T : unmanaged, IResource
    {
        if (track)
        {
            _state->ReadOnlyResources.Add<T>();
        }
        return new(_world->GetResourcePointer<T>());
    }

    public MutableStorage<T> GetMutableStorage<T>(bool track = true) where T : unmanaged, IComponent
    {
        if (track)
        {
            _state->MutableComponents |= ComponentId<T>.Id;
        }
        return new(_world->GetComponents<T>(), GetEventsWriter<ComponentBeingDestroyed>(), GetEventsWriter<ComponentAdded>());
    }

    public ReadOnlyStorage<T> GetReadOnlyStorage<T>(bool track = true) where T : unmanaged, IComponent
    {
        if (track)
        {
            _state->ReadOnlyComponents |= ComponentId<T>.Id;
        }
        return new(_world->GetComponents<T>());
    }

    public EventsReader<T> GetEventsReader<T>() where T : unmanaged, IEvent
        => _world->GetEventReader<T>();

    public EventsWriter<T> GetEventsWriter<T>() where T : unmanaged, IEvent
        => _world->GetEventWriter<T>();

    public ApiResource<T> GetApi<T>() where T : unmanaged, IApi
        => new(_world->GetResourcePointer<T>());

    public EntityFilter CreateFilter(in EntityFilterConfig config) => _world->CreateEntityFilter(config);

    public void RunAfter<T>() where T : unmanaged, IStructSystem<T>
        => _state->RunAfter.Add<T>();
}
