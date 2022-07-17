using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.App;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.Components;
using Titan.ECS.SystemsV2;
using Titan.ECS.SystemsV2.Components;
using Titan.ECS.TheNew;

namespace Titan.ECS.AnotherTry;

public struct HeadlessRunner : IRunner
{
    // NOTE(Jens): just a sample, might be able to use something like this for a server.
    public static void Run(ref Scheduler scheduler, ref World world) => throw new NotImplementedException();
}

public interface IRunner
{
    static abstract void Run(ref Scheduler scheduler, ref World world);
}

internal unsafe struct Runner : IApi
{
    private delegate*<ref Scheduler, ref World, void> _run;
    public static Runner Create<T>() where T : IRunner =>
        new()
        {
            _run = &T.Run
        };
    public void Run(ref Scheduler scheduler, ref World world) => _run(ref scheduler, ref world);
}

public struct App
{
    private MemoryPool _pool;
    private ResourceCollection _resources;
    private World _world;

    internal void Init(in MemoryPool pool, in ResourceCollection resources)
    {
        _pool = pool;
        _resources = resources;
        _world.Init(pool, resources);
        
        resources
            .GetResource<Scheduler>()
            .Init(_resources, ref _world);
    }

    public void Run()
    {
        ref var scheduler = ref _resources.GetResource<Scheduler>();

        _resources
            .GetResource<Runner>()
            .Run(ref scheduler, ref _world);

        Cleanup();
    }

    private void Cleanup()
    {
        _pool.Dispose();
    }
}

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
    private ResourceCollection _resourceCollection;

    private readonly List<SystemDescriptor> _systems = new();
    private readonly List<ComponentDescriptor> _components = new();
    private readonly List<EventsDescriptor> _events = new();

    private readonly App* _app;

    private AppBuilder(AppCreationArgs args)
    {
        // allocate the memory needed for the App instance. This will ensure that only unmanaged/blittable types are used.
        var pool = MemoryPool.Create(args.UnmanagedMemory);

        // The App will be the first entry in the memory block
        _app = pool.GetPointer<App>(initialize: true);

        // Resources must be allocated at the start because they are used when modules are built. (configs etc)
        // We might want to change this at some point to allow for better memory alignment. For example configs used for startup/construction might never be used again, but they'll be mixed with other game related resources.
        _resourceCollection = ResourceCollection.Create(args.ResourcesMemory, args.MaxResourceTypes, pool);

        // Add the pool as a resource so we can use it when setting up the modules (This must be done after it's been used to create the resource collection or the _next in the pool will be wrong.
        AddResource(pool);
        
        // The event system should always be added.
        AddSystemToStage<EventSystem>(Stage.PreUpdate, RunCriteria.Always);
    }

    public static AppBuilder Create() => new(AppCreationArgs.Default);
    public static AppBuilder Create(AppCreationArgs args) => new(args);

    public AppBuilder AddStartupSystem<T>() where T : unmanaged, IStructSystem<T>
    {
        AddSystemToStage<T>(Stage.Startup, RunCriteria.Once);
        return this;
    }
    public AppBuilder AddShutdownSystem<T>() where T : unmanaged, IStructSystem<T>
    {
        AddSystemToStage<T>(Stage.Shutdown, RunCriteria.Once);
        return this;
    }

    public AppBuilder AddSystem<T>(RunCriteria criteria = RunCriteria.Check) where T : unmanaged, IStructSystem<T>
    {
        AddSystemToStage<T>(Stage.Update, criteria);
        return this;
    }

    public AppBuilder AddSystemToStage<T>(Stage stage, RunCriteria criteria = RunCriteria.Check) where T : unmanaged, IStructSystem<T>
    {
        _systems.Add(SystemDescriptor.Create<T>(stage, criteria));
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

    public AppBuilder AddEvent<T>(uint maxEventsPerFrame = 1) where T : unmanaged, IEvent
    {
        _events.Add(EventsDescriptor.Create<T>(maxEventsPerFrame));
        return this;
    }

    public AppBuilder UseRunner<T>() where T : unmanaged, IRunner
    {
        if (_resourceCollection.HasResource<Runner>())
        {
            throw new InvalidOperationException("A runner has already been set.");
        }
        _resourceCollection.InitResource(Runner.Create<T>());
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

    public T* GetResourcePointer<T>() where T : unmanaged
        => _resourceCollection.GetResourcePointer<T>();

    public ref App Build()
    {
        if (!_resourceCollection.HasResource<Runner>())
        {
            throw new InvalidOperationException($"Failed to find {nameof(Runner)} resource. Make sure you've set a Runner using the {nameof(UseRunner)}<T>() method.");
        }

        if (!_resourceCollection.HasResource<EntityConfiguration>())
        {
            throw new InvalidOperationException($"Failed to find {nameof(EntityConfiguration)} resource. Make sure you've added the CoreModule before calling build.");
        }

        ref readonly var pool = ref _resourceCollection.GetResource<MemoryPool>();
        // Initialize the Compoenent pools (this is done at build because we want all components to be in the same place)
        ref readonly var config = ref _resourceCollection.GetResource<EntityConfiguration>();
        _resourceCollection
            .GetResource<ComponentRegistry>()
            .Init(pool, config.MaxEntities, _components.ToArray());

        

        _resourceCollection
            .GetResource<EventsRegistry>()
            .Init(pool, _events.ToArray());
        
        _resourceCollection
            .GetResource<SystemsRegistry>()
            .Init(pool, _systems.ToArray());

        _app->Init(pool, _resourceCollection);

        return ref *_app;
    }
}

internal unsafe struct SystemsRegistry
{
    private SystemDescriptor* _systems;
    private int _count;

    public void Init(in MemoryPool pool, ReadOnlySpan<SystemDescriptor> systems)
    {
        _count = systems.Length;
        _systems = pool.GetPointer<SystemDescriptor>((uint)_count);
        systems.CopyTo(new Span<SystemDescriptor>(_systems, _count));
    }
    public readonly ReadOnlySpan<SystemDescriptor> GetDescriptors() => new(_systems, _count);
}
