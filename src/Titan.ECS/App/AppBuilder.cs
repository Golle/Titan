using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Events;
using Titan.ECS.Modules;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Memory;

namespace Titan.ECS.App;

public unsafe class AppBuilder
{
    private ResourceCollection _resourceCollection;
    private readonly List<SystemDescriptor> _systems = new();
    private readonly List<ComponentDescriptor> _components = new();

    private AppBuilder(AppCreationArgs args)
    {
        var allocator = PlatformAllocator.Create(args.UnmanagedMemory);
        
        // Resources must be allocated at the start because they are used when modules are built. (configs etc)
        // We might want to change this at some point to allow for better memory alignment. For example configs used for startup/construction might never be used again, but they'll be mixed with other game related resources.
        _resourceCollection = ResourceCollection.Create(args.ResourcesMemory, args.MaxResourceTypes, allocator);

        // Add the PlatformAllocator as a resource so we can use it when setting up the modules (This must be done after it's been used to create the resource collection or the _next in the pool will be wrong.
        AddResource(allocator);

        // The event system should always be added. Run it before anything else.
        AddSystemToStage<EventSystem>(Stage.First, RunCriteria.Always, int.MaxValue - 1);
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

    public AppBuilder AddSystem<T>(RunCriteria criteria = RunCriteria.Check, uint priority = 0) where T : unmanaged, IStructSystem<T>
    {
        AddSystemToStage<T>(Stage.Update, criteria);
        return this;
    }

    public AppBuilder AddSystemToStage<T>(Stage stage, RunCriteria criteria = RunCriteria.Check, int priority = 0) where T : unmanaged, IStructSystem<T>
    {
        _systems.Add(SystemDescriptor.Create<T>(stage, criteria, priority));
        return this;
    }

    /// <summary>
    /// The ISystem interface allows for implementation that act like objects instead of taking a ref parameter. Might be easier to use. (There might be a tiny overhead in call speed)
    /// </summary>
    /// <typeparam name="T">The struct that implements the ISystem interface</typeparam>
    /// <param name="criteria"></param>
    /// <param name="priority"></param>
    /// <returns></returns>
    public AppBuilder AddSystemExperimental<T>(RunCriteria criteria = RunCriteria.Check, int priority = 0) where T : unmanaged, ISystem
    {
        _systems.Add(SystemDescriptor.CreateExperimental<T>(Stage.Update, criteria, priority));
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

    public AppBuilder UseRunner<T>() where T : unmanaged, IRunner
    {
        if (_resourceCollection.HasResource<Runner>())
        {
            throw new InvalidOperationException("A runner has already been set.");
        }
        _resourceCollection.InitResource(Runner.Create<T>());
        return this;
    }

    public AppBuilder AddModule<T>() where T : IModule
    {
        T.Build(this);
        return this;
    }

    public ref T GetResource<T>() where T : unmanaged
        => ref _resourceCollection.GetResource<T>();

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

    public bool HasResource<T>() where T : unmanaged
        => _resourceCollection.HasResource<T>();

    public App Build()
    {
        if (!_resourceCollection.HasResource<Runner>())
        {
            throw new InvalidOperationException($"Failed to find {nameof(Runner)} resource. Make sure you've set a Runner using the {nameof(UseRunner)}<T>() method.");
        }

        if (!_resourceCollection.HasResource<ECSConfiguration>())
        {
            throw new InvalidOperationException($"Failed to find {nameof(ECSConfiguration)} resource. Make sure you've added the CoreModule before calling build.");
        }

        ref readonly var allocator = ref _resourceCollection.GetResource<PlatformAllocator>();
        // Initialize the Compoenent pools (this is done at build because we want all components to be in the same place)
        ref readonly var config = ref _resourceCollection.GetResource<ECSConfiguration>();
        _resourceCollection
            .GetOrCreateResource<ComponentRegistry>()
            .Init(allocator, config.MaxEntities, _components.ToArray());
        //NOTE(Jens): Add a clenaup system for component registry that runs on shutdown (just for completeness, might disable it in release builds)


        _resourceCollection
            .GetOrCreateResource<EventsRegistry>()
            .Init(allocator, config.EventStreamSize, config.MaxEventTypes);

        _resourceCollection
            .GetOrCreateResource<SystemsRegistry>()
            .Init(allocator, _systems.ToArray());

        _resourceCollection
            .InitResource<Scheduler.Scheduler>();

        return App.Create(_resourceCollection);
    }
}
