using Titan.Assets;
using Titan.Assets.Creators;
using Titan.ECS.Components;
using Titan.Modules;
using Titan.Setup.Configs;
using Titan.Systems;

namespace Titan.Setup;

public interface IAppBuilder
{
    AppCreationArgs CreationArgs { get; }
    IAppBuilder AddConfig(IConfiguration config);
    IAppBuilder AddModule<T>() where T : IModule;
    //NOTE(Jens): the new() constraint is not compatible with NativeAOT/Trimming. Waiting for an answer from the C# community.
    //IAppBuilder AddManagedResource<T>() where T : class, new();
    IAppBuilder AddAssetsManifest<T>(bool builtinAsset = false) where T : struct, IManifestDescriptor;
    IAppBuilder AddResourceCreator<TResource, TContext>() where TResource : unmanaged where TContext : unmanaged, IResourceCreator<TResource>;
    IAppBuilder AddManagedResource<T>(T resource) where T : class;
    IAppBuilder AddResource<T>(in T value = default) where T : unmanaged;
    IAppBuilder AddSystem<T>(RunCriteria runCriteria = RunCriteria.Check, int priority = 0) where T : unmanaged, ISystem;
    IAppBuilder AddSystemToStage<T>(SystemStage stage, RunCriteria runCriteria = RunCriteria.Check, int priority = 0) where T : unmanaged, ISystem;
    IAppBuilder AddComponent<T>(ComponentPoolType type) where T : unmanaged, IComponent;
    IAppBuilder AddComponent<T>(uint count, ComponentPoolType type) where T : unmanaged, IComponent;
    IAppBuilder UseRunner<T>() where T : IRunner;
    IApp Build();
}
