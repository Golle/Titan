using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Titan.Core;
using Titan.Core.App;
using Titan.Core.Memory;
using Titan.ECS.Components;
using Titan.ECS.SystemsV2;
using Titan.ECS.SystemsV2.Components;
using Titan.ECS.TheNew;

namespace Titan.ECS.AnotherTry;

public struct Runner
{

}

public unsafe struct App
{
    private MemoryPool _pool;
    private World _world;
    private Scheduler _scheduler;
    private Runner _runner;

    internal void Init(in MemoryPool pool, ReadOnlySpan<SystemDescriptor> systemDescriptors, in ResourceCollection resources)
    {
        _pool = pool;
        //_runner.Init();
        //_scheduler.Init(pool);
        _world.Init(pool, resources);
    }

    public void Run()
    {

    }
}

public unsafe struct World
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
}

public unsafe struct ComponentCollection
{

}

public struct EntityManager { }
public struct Scheduler
{

}

/*
 * State (This would be the Scene management in Titan)
 *  OnLoad,
 *  OnEnter,
 *  OnLeave,
 *  OnUnload
 *
 * Sample state: Splash, MainMenu, WorldGeneration, Game, GameEnded etc.
 */


/*
 * Entities and Components are registered in the Global space
 * Systems are global by default, but can be a part of a state as well. A state has different states
 *
 * Entities - Global
 * Components - Global
 * Resources - Global (No local in first version)
 * Systems - Global  (No local in first version)
 *
 *
 */

public unsafe class AppBuilder
{
    private readonly MemoryPool _pool;
    private readonly ResourceCollection _resourceCollection;

    private readonly List<SystemDescriptor> _systems = new();
    private readonly List<ComponentDescriptor> _components = new();
    private readonly App* _app;

    private AppBuilder(AppCreationArgs args)
    {
        // allocate the memory needed for the App instance. This will ensure that only unmanaged/blittable types are used.
        _pool = MemoryPool.Create(args.UnmanagedMemory);

        // The App will be the first entry in the memory block
        _app = _pool.GetPointer<App>(initialize: true);

        // Resources must be allocated at the start because they are used when modules are built. (configs etc)
        // We might want to change this at some point to allow for better memory alignment. For example configs used for startup/construction might never be used again, but they'll be mixed with other game related resources.
        _resourceCollection = ResourceCollection.Create(args.ResourcesMemory, args.MaxResourceTypes, _pool);
    }

    public static AppBuilder Create() => new(AppCreationArgs.Default);
    public static AppBuilder Create(AppCreationArgs args) => new(args);
    public AppBuilder AddSystem<T>() where T : unmanaged, IStructSystem<T>
    {
        AddSystemToStage<T>(Stage.Update);
        return this;
    }

    public AppBuilder AddSystemToStage<T>(Stage stage) where T : unmanaged, IStructSystem<T>
    {
        _systems.Add(SystemDescriptor.Create<T>(stage));
        return this;
    }

    public AppBuilder AddResource<T>(in T value = default) where T : unmanaged
    {
        _resourceCollection.InitResource(value);
        return this;
    }

    public AppBuilder AddComponents<T>(ComponentPoolTypes type = ComponentPoolTypes.Packed, uint maxComponents = 0u) where T : unmanaged, IComponent
    {
        Debug.Assert(_components.All(c => c.ComponentId != ComponentId<T>.Id), $"Component type {typeof(T).Name} has already been registered.");
        _components.Add(ComponentDescriptor.Create<T>(type, maxComponents));
        return this;
    }

    public AppBuilder AddModule<T>() where T : IModule2
    {
        T.Build(this);
        return this;
    }

    public ref T GetResourceOrDefault<T>() where T : unmanaged, IDefault<T>
    {
        if (!_resourceCollection.HasResource<T>())
        {
            _resourceCollection.InitResource(T.Default);
        }
        return ref _resourceCollection.GetResource<T>();
    }

    public ref App Build()
    {
        if (!_resourceCollection.HasResource<EntityConfiguration>())
        {
            throw new InvalidOperationException($"Failed to find {nameof(EntityConfiguration)} resource. Make sure you've added the CoreModule before calling build.");
        }
        // Initialize the Compoenent pools (this is done at build because we want all components to be in the same place)
        ref readonly var config = ref _resourceCollection.GetResource<EntityConfiguration>();
        ref var registry = ref _resourceCollection.GetResource<ComponentRegistry>();
        registry.Init(_pool, config.MaxEntities, _components.ToArray());

        _app->Init(_pool, _systems.ToArray(), _resourceCollection);

        return ref *_app;
    }
}

public struct ResourceDescriptor
{
    private readonly ResourceId _id;
    private readonly uint _size;
    private ResourceDescriptor(ResourceId id, uint size)
    {
        _id = id;
        _size = size;
    }
    internal static unsafe ResourceDescriptor Create<T>() where T : unmanaged
        => new(ResourceId.Id<T>(), (uint)sizeof(T));
}
