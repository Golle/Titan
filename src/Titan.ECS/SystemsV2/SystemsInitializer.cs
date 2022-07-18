using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.EntitiesNew;
using Titan.ECS.Modules;
using Titan.ECS.Systems;
using Titan.ECS.TheNew;
using EntityFilter = Titan.ECS.EntitiesNew.EntityFilter;

namespace Titan.ECS.SystemsV2;

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

    public MutableResource<T> GetMutableGlobalResource<T>() where T : unmanaged, IResource
    {
        _state->MutableGlobalResources.Add(ResourceId.Id<T>());
        return new(_world->GetResourcePointer<T>());
    }

    public ReadOnlyResource<T> GetReadOnlyGlobalResource<T>() where T : unmanaged, IResource
    {
        _state->ReadOnlyGlobalResources.Add(ResourceId.Id<T>());
        return new(_world->GetResourcePointer<T>());
    }

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

    public MutableStorage3<T> GetMutableStorage<T>() where T : unmanaged, IComponent
    {
        _state->MutableComponents |= ComponentId<T>.Id;
        return new(_world->GetComponents<T>(), GetEventsWriter<ComponentBeingDestroyed>(), GetEventsWriter<ComponentAdded>());
    }

    public ReadOnlyStorage3<T> GetReadOnlyStorage<T>() where T : unmanaged, IComponent
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

    public void GetEntities(in EntityFilter filter) => _world->CreateEntityQuery(filter);

    public void RunAfter<T>() where T : unmanaged, IStructSystem<T> 
        => _state->RunAfter.Add<T>();
}
