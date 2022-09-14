using System.Diagnostics;
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
    private readonly List<IConfiguration> _configs = new();

    private AppBuilder(AppCreationArgs args)
    {
        if (!MemoryManager.Create(new MemoryConfiguration(args.MaxReservedMemory, args.MaxGeneralPurposeMemory), out var memoryManager))
        {
            throw new Exception($"Failed to create the {nameof(MemoryManager)}");
        }

        // Resources must be allocated at the start because they are used when modules are built. (configs etc)
        // We might want to change this at some point to allow for better memory alignment. For example configs used for startup/construction might never be used again, but they'll be mixed with other game related resources.
        if (!ResourceCollection.Create(&memoryManager, args.MaxResourcesMemory, args.MaxResourceTypes, out _resourceCollection))
        {
            throw new Exception($"Failed to create the {nameof(ResourceCollection)}");
        }
        Thread.Sleep(2000);
        // Add the PlatformAllocator as a resource so we can use it when setting up the modules (This must be done after it's been used to create the resource collection or the _next in the pool will be wrong.
        AddResource(memoryManager);
        
        // The event system should always be added. Run it before anything else.
        AddSystemToStage<EventSystem>(Stage.First, RunCriteria.Always, int.MaxValue - 1);
    }

    public static AppBuilder Create() => new(AppCreationArgs.Default);
    public static AppBuilder Create(AppCreationArgs args) => new(args);

    /// <summary>
    /// Add manager configuration, these will only be available at startup and disposed by the end of the build.
    /// The configuration allows multiple configs of the same type, it's up the module to decide what to do with that.
    /// </summary>
    /// <param name="config">The managed configuration (usually a record containg managed references like strings)</param>
    /// <returns></returns>
    public AppBuilder AddConfiguration(IConfiguration config)
    {
        _configs.Add(config);
        return this;
    }

    public AppBuilder AddStartupSystem<T>() where T : unmanaged, IStructSystem<T>
    {
        AddSystemToStage<T>(Stage.Startup, RunCriteria.Once);
        return this;
    }
    public AppBuilder AddShutdownSystem<T>(RunCriteria criteria = RunCriteria.Once) where T : unmanaged, IStructSystem<T>
    {
        AddSystemToStage<T>(Stage.Shutdown, criteria);
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
        if (!T.Build(this))
        {
            throw new Exception($"Failed to add the {typeof(T).FullName} module");
        }
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

    public T GetConfiguration<T>() where T : IConfiguration
        => (T)_configs.FirstOrDefault(c => c.GetType() == typeof(T));

    public IEnumerable<T> GetConfigurations<T>() where T : IConfiguration
        => _configs.Where(c => c.GetType() == typeof(T)).Cast<T>();

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

        var memoryManager = _resourceCollection.GetResourcePointer<MemoryManager>();

        // Initialize the Compoenent pools (this is done at build because we want all components to be in the same place)
        ref readonly var config = ref _resourceCollection.GetResource<ECSConfiguration>();
        _resourceCollection
            .GetOrCreateResource<ComponentRegistry>()
            .Init(memoryManager, config.MaxEntities, _components.ToArray());
        //NOTE(Jens): Add a clenaup system for component registry that runs on shutdown (just for completeness, might disable it in release builds)


        _resourceCollection
            .GetOrCreateResource<EventsRegistry>()
            .Init(memoryManager, config.EventStreamSize, config.MaxEventTypes);

        _resourceCollection
            .GetOrCreateResource<SystemsRegistry>()
            .Init(memoryManager, _systems.ToArray());

        _resourceCollection
            .InitResource<Scheduler.Scheduler>();

        return App.Create(_resourceCollection);
    }
}
