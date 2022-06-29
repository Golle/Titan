using System;
using Titan.Core.App;

namespace Titan.Core;

public interface IModule
{
    static abstract void Build(IApp app);
}

public interface IApp : IDisposable
{
    IApp AddModule<T>() where T : IModule;
    IApp AddEvent<T>(uint maxEventsPerFrame = 1) where T : unmanaged;
    IApp AddSystem<T>() where T : ISystem, new();
    IApp AddSystemToStage<T>(Stage stage) where T : ISystem, new();
    IApp AddResource<T>(in T resource) where T : unmanaged;
    ref readonly T GetResource<T>() where T : unmanaged;
    ref T GetMutableResource<T>() where T : unmanaged;
    unsafe T* GetMutableResourcePointer<T>() where T : unmanaged;
    bool HasResource<T>() where T : unmanaged;
    IApp AddDisposable(IDisposable disposable);
    IApp Run();
}
