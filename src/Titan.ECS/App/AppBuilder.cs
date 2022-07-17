using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Titan.Core;
using Titan.Core.Memory;
using Titan.ECS.Components;
using Titan.ECS.Events;
using Titan.ECS.Modules;
using Titan.ECS.Scheduler;
using Titan.ECS.SystemsV2;
using Titan.ECS.TheNew;
using ComponentRegistry = Titan.ECS.Components.ComponentRegistry;

namespace Titan.ECS.App;

public unsafe class AppBuilder
{
    private readonly ResourceCollection _resourceCollection;
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

        if (!_resourceCollection.HasResource<ECSConfiguration>())
        {
            throw new InvalidOperationException($"Failed to find {nameof(ECSConfiguration)} resource. Make sure you've added the CoreModule before calling build.");
        }

        ref readonly var pool = ref _resourceCollection.GetResource<MemoryPool>();
        // Initialize the Compoenent pools (this is done at build because we want all components to be in the same place)
        ref readonly var config = ref _resourceCollection.GetResource<ECSConfiguration>();
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
