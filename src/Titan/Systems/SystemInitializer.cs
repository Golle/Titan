using Titan.Assets;
using Titan.Audio;
using Titan.Audio.Playback;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;
using Titan.ECS;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Queries;
using Titan.Events;
using Titan.Input;
using Titan.Memory;
using Titan.Resources;
using Titan.Systems.Scheduler;

namespace Titan.Systems;

public readonly unsafe ref struct SystemInitializer
{
    private readonly ResourceCollection _resources;
    private readonly ILinearAllocator _allocator;
    private readonly ref SystemDependencyState _state;

    internal SystemInitializer(ref SystemDependencyState state, ResourceCollection resources, ILinearAllocator allocator)
    {
        _resources = resources;
        _allocator = allocator;
        _state = ref state;
    }

    public AssetsManager GetAssetsManager()
        => new(_resources.GetManaged<AssetsRegistry>(), GetEventsWriter<AssetLoadRequested>(), GetEventsWriter<AssetUnloadRequested>(), GetEventsWriter<AssetReloadRequested>());

    public InputManager GetInputManager()
        => new(_resources.GetPointer<InputState>());

    public EntityManager GetEntityManager()
        => new(_resources.GetManaged<EntityRegistry>());

    public ComponentManager GetComponentManager()
        => new(_resources.GetManaged<ComponentsRegistry>());
    public AudioManager GetAudioManager()
    => new(_resources.GetManaged<AudioCommandQueue>());

    public TempArena GetTempArena()
        => new(_resources.GetPointer<PerFrameArena>());

    public MutableResource<T> GetMutableResource<T>(bool trackDependency = true) where T : unmanaged, IResource
    {
        if (trackDependency)
        {
            _state.MutableResources.Add<T>();
        }
        return new(_resources.GetPointer<T>());
    }

    public ReadOnlyResource<T> GetReadOnlyResource<T>(bool trackDependency = true) where T : unmanaged, IResource
    {
        if (trackDependency)
        {
            _state.ReadOnlyResources.Add<T>();
        }
        return new(_resources.GetPointer<T>());
    }

    public MutableStorage<T> GetMutableStorage<T>(bool trackDependency = true) where T : unmanaged, IComponent
    {
        if (trackDependency)
        {
            _state.MutableComponents += ComponentId<T>.Id;
        }
        var pool = _resources
            .GetManaged<ComponentsRegistry>()
            .Value
            .GetPool<T>();
        var registry = _resources
            .GetManaged<ComponentsRegistry>();

        return new(pool, registry);
    }

    public ReadOnlyStorage<T> GetReadOnlyStorage<T>(bool trackDependency = true) where T : unmanaged, IComponent
    {
        if (trackDependency)
        {
            _state.ReadOnlyComponents += ComponentId<T>.Id;
        }
        var pool = _resources
            .GetManaged<ComponentsRegistry>()
            .Value
            .GetPool<T>();

        return new(pool);
    }

    public Ref<T> GetApi<T>() where T : unmanaged
        => throw new NotImplementedException();
    /*=> _resources.Get<T>()*/

    public ObjectHandle<T> GetManagedApi<T>() where T : class
        => _resources.Get<ObjectHandle<T>>();

    public EventsReader<T> GetEventsReader<T>() where T : unmanaged, IEvent
        => _resources.GetManaged<IEventsManager>().Value.CreateReader<T>();

    public EventsWriter<T> GetEventsWriter<T>() where T : unmanaged, IEvent
        => _resources.GetManaged<IEventsManager>().Value.CreateWriter<T>();
    public EntityQuery CreateQuery(in EntityQueryArgs args)
        => _resources.GetManaged<EntityQueryRegistry>().Value.CreateQuery(args);

    public TitanArray<T> AllocArray<T>(uint count, bool initialize = true) where T : unmanaged
        => _allocator.AllocArray<T>(count, initialize);
    public TitanBuffer AllocBuffer<T>(uint size, bool initialize = true) where T : unmanaged
        => new(_allocator.Alloc(size, initialize), size);


}
