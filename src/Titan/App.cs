using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Titan.Core;
using Titan.Core.App;
using Titan.Core.Events;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.Systems;
using Titan.Graphics.Modules;
using Titan.NewStuff;

namespace Titan;


public class EventSystem<T> : ResourceSystem where T : unmanaged
{
    private readonly MutableResource<Events<T>> _events;

    public EventSystem()
    {
        _events = GetMutableResource<Events<T>>();
    }
    public override void OnUpdate() => _events.Get().Swap();
}


public class TheWorld
{
    private UnmanagedResources _unmanagedResources;

    public TheWorld()
    {

    }
    //private Dictionary<>

}

public abstract class EntitySystem : ResourceSystem
{

}
public abstract class ResourceSystem : ISystem
{
    protected ReadOnlyResource<T> GetReadOnlyResource<T>() where T : unmanaged
    {
        return new ReadOnlyResource<T>();
    }
    protected MutableResource<T> GetMutableResource<T>() where T : unmanaged
    {
        return new MutableResource<T>();
    }

    protected T GetManagedResource<T>() where T : class, new()
    {
        return new T();
    }
    public abstract void OnUpdate();
}


public class SystemsDescriptorCollection
{
    private readonly List<SystemDescriptor> _descriptors = new();
    public void Add(in SystemDescriptor descriptor)
    {
        if (descriptor.Creator == null)
        {
            throw new InvalidOperationException("The System property on the descriptor has not been set.");
        }

        var type = descriptor.GetType();
        if (_descriptors.Any(d => d.Creator.GetType() == type))
        {
            // NOTE(Jens): maybe allow the same system but in different stages?
            throw new InvalidOperationException($"A system of type {type} has already been added.");
        }
        _descriptors.Add(descriptor);
    }
    public IEnumerable<SystemDescriptor> Enumerate(Stage stage) => _descriptors.Where(d => d.Stage == stage);
}

public class App : IApp
{
    private readonly UnmanagedResources _resources;
    private readonly IPersistentMemoryAllocator _persistentMemoryAllocator;
    private readonly List<IDisposable> _disposables = new();
    private readonly SystemsDescriptorCollection _systems = new();

    public static App Create()
    {
        var persistentMemoryAllocator = new NativeMemoryAllocator(1 * 1024 * 1024 * 1024); // allocate 1GB, this should be configurable, and maybe auto adjust when needed.

        return new(persistentMemoryAllocator);
    }

    private App(IPersistentMemoryAllocator persistentMemoryAllocator)
    {
        const int maxResourceTypes = 300;
        _persistentMemoryAllocator = persistentMemoryAllocator;
        _resources = new UnmanagedResources(32 * 1024 * 1024, maxResourceTypes, persistentMemoryAllocator);
    }

    public IApp AddSystem<T>() where T : ISystem, new() => AddSystemToStage<T>(Stage.Update);

    public IApp AddSystemToStage<T>(Stage stage) where T : ISystem, new()
    {
        _systems.Add(new SystemDescriptor(() => new T(), stage));
        return this;
    }

    public IApp AddEvent<T>(uint maxEvents = 10) where T : unmanaged
    {
        var events = new Events<T>(maxEvents, _persistentMemoryAllocator);
        AddResource(events);

        return this;
    }

    public IApp AddResource<T>(in T resource) where T : unmanaged
    {
        _resources.InitResource(resource);
        if (resource is IDisposable disposable)
        {
            _disposables.Add(disposable);
        }
        return this;
    }

    public ref readonly T GetResource<T>() where T : unmanaged => ref _resources.GetResource<T>();
    public ref T GetMutableResource<T>() where T : unmanaged => ref _resources.GetResource<T>();
    public unsafe T* GetMutableResourcePointer<T>() where T : unmanaged => _resources.GetResourcePointer<T>();
    public bool HasResource<T>() where T : unmanaged => _resources.HasResource<T>();
    public IApp AddDisposable(IDisposable disposable)
    {
        _disposables.Add(disposable);
        return this;
    }

    public IApp AddModule<T>() where T : IModule
    {
        try
        {
            T.Build(this);
        }
        catch (Exception e)
        {
            // NOTE(Jens): should we have a results type or just exit?
            Logger.Error<App>($"Build Module {typeof(T).Name} failed with {e.GetType().Name}: {e.Message}");
        }

        return this;
    }


    public IApp Run()
    {
        // Init systems, create execution grapt
        // call startup systems
        
        // start main loop (on a different thread)

        // start polling platform events

        // call terminate systems 

        if (HasResource<Window>())
        {
            ref var window = ref _resources.GetResource<Window>();
            Logger.Info<App>("Start window update");
            window.SetTitle("Apskit");
            while (window.Update())
            {

                // noop
            }
        }
        else
        {
            Logger.Info<App>("No window has been created.");
        }

        Logger.Info<App>("Exiting");

        return this;
    }

    public void Dispose()
    {
        // Dispose in reverse order
        for (var i = _disposables.Count - 1; i >= 0; --i)
        {
            _disposables[i].Dispose();
        }
        
        _resources.Dispose();
        (_persistentMemoryAllocator as IDisposable)?.Dispose();
    }
}
