using System.Diagnostics;
using Titan.Assets;
using Titan.Assets.Creators;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS;
using Titan.ECS.Components;
using Titan.Modules;
using Titan.Resources;
using Titan.Setup.Configs;
using Titan.Systems;

namespace Titan.Setup;

public sealed class AppBuilder : IAppBuilder
{
    public AppCreationArgs CreationArgs { get; }

    private IRunner _runner;
    private readonly List<IConfiguration> _configurations = new();
    private readonly List<ModuleLifetime> _modules = new();
    private readonly ResourceCollection _resources = new();

    public AppBuilder(AppCreationArgs args)
    {
        CreationArgs = args;
        var memoryManager = new MemoryManager();
        if (!memoryManager.Init(new MemoryConfiguration(args.ReserveVirtualMemory, args.GeneralPurposeMemory)))
        {
            Logger.Error<AppBuilder>($"Failed to create the {nameof(MemoryManager)}.");
            throw new Exception($"Failed to init the {nameof(MemoryManager)}");
        }

        if (!_resources.Init(memoryManager, args.UnmanagedResourceTypes, args.UnmanagedResourceMemory))
        {
            Logger.Error<AppBuilder>($"Failed to create the {nameof(ResourceCollection)}.");
            memoryManager.Shutdown();
            throw new Exception($"Failed to init the {nameof(ResourceCollection)}");
        }

        // Add the memory manager and "self" to the resources.
        AddManagedResource<IMemoryManager>(memoryManager)
            .AddManagedResource(_resources)
            .AddManagedResource(new SystemsRegistry());
    }

    public IAppBuilder AddConfig(IConfiguration config)
    {
        _configurations.Add(config);

        return this;
    }

    public IAppBuilder AddModule<T>() where T : IModule
    {
        if (_modules.Any(m => m.Type == typeof(T)))
        {
            throw new AppBuilderException($"Module {typeof(T).Name} has already been added.");
        }

        if (!T.Build(this))
        {
            throw new AppBuilderException($"Failed to add module {typeof(T).Name}");
        }
        _modules.Add(new ModuleLifetime(typeof(T), T.Init, T.Shutdown));
        return this;
    }

    public IAppBuilder AddAssetsManifest<T>(bool builtInAsset) where T : struct, IManifestDescriptor
    {
        _configurations.Add(AssetsConfiguration.CreateFromRegistry<T>(builtInAsset));
        return this;
    }

    public IAppBuilder AddResourceCreator<TResource, TContext>() where TResource : unmanaged where TContext : unmanaged, IResourceCreator<TResource>
    {
        _configurations.Add(ResourceCreatorConfiguration.Create<TResource, TContext>());
        return this;
    }

    public IAppBuilder AddManagedResource<T>(T resource) where T : class
    {
        _resources.AddManaged(resource);
        return this;
    }

    public IAppBuilder AddResource<T>(in T value) where T : unmanaged
    {
        _resources.Add(value);
        return this;
    }

    public IAppBuilder AddSystem<T>(RunCriteria runCriteria, int priority) where T : unmanaged, ISystem
        => AddSystemToStage<T>(SystemStage.Update, runCriteria, priority);

    public IAppBuilder AddSystemToStage<T>(SystemStage stage, RunCriteria runCriteria, int priority) where T : unmanaged, ISystem
    {
        var registry = _resources.GetManaged<SystemsRegistry>().Value;
        Debug.Assert(registry != null);
        registry.AddSystem<T>(stage, runCriteria, priority);
        return this;
    }

    public IAppBuilder AddComponent<T>(ComponentPoolType type) where T : unmanaged, IComponent => AddComponent<T>(0, type);
    public IAppBuilder AddComponent<T>(uint count, ComponentPoolType type) where T : unmanaged, IComponent
    {
        _configurations.Add(ComponentConfig.Create<T>(count, type));
        return this;
    }

    public IAppBuilder UseRunner<T>() where T : IRunner
    {
        if (_runner != null)
        {
            throw new AppBuilderException($"A runner has already been registered({_runner.GetType().Name})");
        }

        _runner = T.Create();
        return this;
    }

    public IApp Build()
    {
        if (_runner == null)
        {
            throw new AppBuilderException($"Must specify a Runner with the {nameof(UseRunner)} method.");
        }

        return new App(
            _runner,
            _modules.ToArray(),
            _configurations.ToArray(),
            _resources
        );
    }
}

public class AppBuilderException : Exception
{
    public AppBuilderException(string message)
        : base(message)
    {
    }
}
