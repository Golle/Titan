using System;
using Titan.Core.App;

namespace Titan.ECS.SystemsV2;

public interface IApp : IDisposable
{
    IApp AddModule<T>() where T : IModule;
    IApp AddEvent<T>(uint maxEventsPerFrame = 1) where T : unmanaged;
    IApp AddResource<T>(in T resource) where T : unmanaged;
    ref readonly T GetResource<T>() where T : unmanaged;
    ref T GetMutableResource<T>() where T : unmanaged;
    unsafe T* GetMutableResourcePointer<T>() where T : unmanaged;
    bool HasResource<T>() where T : unmanaged;
    IApp AddDisposable(IDisposable disposable);
    IApp Run();


    // Global systems?
    IApp AddSystem<T>() where T : unmanaged, IStructSystem<T>;
    IApp AddSystemToStage<T>(Stage stage) where T : unmanaged, IStructSystem<T>;

    IApp AddWorld(Action<WorldConfig> config);
    IApp AddWorld<T>() where T : IWorldModule;
}
