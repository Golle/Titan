using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Systems;
using EntityFilter = Titan.ECS.Entities.EntityFilter;

namespace Titan.ECS.Scheduler;

public readonly unsafe ref struct SystemsInitializer
{
    private readonly SystemDependencyState* _state;
    private readonly World.World* _world;

    internal SystemsInitializer(ref SystemDependencyState state, ref World.World world)
    {
        _world = (World.World*)Unsafe.AsPointer(ref world);
        _state = (SystemDependencyState*)Unsafe.AsPointer(ref state);
    }

    public EntityHandler GetEntityHandler()
        => new(
            GetEventsWriter<EntityCreated>(),
            GetEventsWriter<EntityBeingDestroyed>(),
            _world->GetResourcePointer<EntityIdContainer>()
        );

    public MutableResource<T> GetMutableResource<T>() where T : unmanaged, IResource
    {
        _state->MutableResources.Add<T>();
        return new(_world->GetResourcePointer<T>());
    }

    public ReadOnlyResource<T> GetReadOnlyResource<T>() where T : unmanaged, IResource
    {
        //NOTE(Jens): should there be a destiction between local and global resources? 
        _state->ReadOnlyResources.Add<T>();
        return new(_world->GetResourcePointer<T>());
    }

    public MutableStorage<T> GetMutableStorage<T>() where T : unmanaged, IComponent
    {
        _state->MutableComponents |= ComponentId<T>.Id;
        return new(_world->GetComponents<T>(), GetEventsWriter<ComponentBeingDestroyed>(), GetEventsWriter<ComponentAdded>());
    }

    public ReadOnlyStorage<T> GetReadOnlyStorage<T>() where T : unmanaged, IComponent
    {
        _state->ReadOnlyComponents |= ComponentId<T>.Id;

        return new(_world->GetComponents<T>());
    }

    public EventsReader<T> GetEventsReader<T>() where T : unmanaged, IEvent
        => new(_world->GetEvents<T>());

    public EventsWriter<T> GetEventsWriter<T>() where T : unmanaged, IEvent
        => new(_world->GetEvents<T>());

    public ApiResource<T> GetApi<T>() where T : unmanaged, IApi
        => new(_world->GetResourcePointer<T>());

    public EntityFilter CreateFilter(in EntityFilterConfig config) => _world->CreateEntityFilter(config);
    public void GetEntities(in EntityFilterConfig filter) => _world->CreateEntityFilter(filter);

    public void RunAfter<T>() where T : unmanaged, IStructSystem<T> 
        => _state->RunAfter.Add<T>();
}
