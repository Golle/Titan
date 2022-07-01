using System;
using System.Collections.Generic;
using System.Linq;
using Titan.Core;
using Titan.Core.App;
using Titan.Core.Events;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.Components;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;
using Titan.ECS.SystemsV2.Components;
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

        return new App(persistentMemoryAllocator);
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

    public IApp AddEvent<T>(uint maxEvents = 10) where T : unmanaged =>
        AddResource(new Events<T>(maxEvents, _persistentMemoryAllocator))
            .AddSystemToStage<EventSystem<T>>(Stage.PreUpdate);

    public IApp AddComponent<T>(uint maxComponents = 100/*, ComponentPoolTypes type = ComponentPoolTypes.Packed*/) where T : unmanaged
    {
        //NOTE(Jens): Components and Systems belong in the "World". Should we support multiple worlds in the same "App" or a single World per app?
        // Register a factory for this as a "module" ?
        var type = ComponentPoolTypes.Packed;
        var components = type switch
        {
            ComponentPoolTypes.Packed => PackedComponentPool<T>.CreatePool(_persistentMemoryAllocator, 10_000, maxComponents),
            _ => throw new NotSupportedException()
        };
        _resources.InitResource(components);
        // NOTE(Jens): implement this when we support it
        //AddSystemToStage<ComponentSystem<T>>(Stage.PreUpdate);
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
        var preUpdate = _systems.Enumerate(Stage.PreUpdate).Select(s => s.Creator()).ToArray();
        var update = _systems.Enumerate(Stage.Update).Select(s => s.Creator()).ToArray();
        var postUpdate = _systems.Enumerate(Stage.PostUpdate).Select(s => s.Creator()).ToArray();
        // Init systems, create execution grapt
        // call startup systems

        // start main loop (on a different thread)

        // start polling platform events

        // call terminate systems 

        var active = true;

        //var t = new Thread(() =>
        //{
        //    while (active)
        //    {
        //        foreach (var system in preUpdate)
        //        {
        //            system.OnUpdate();
        //        }

        //        foreach (var system in update)
        //        {
        //            system.OnUpdate();
        //        }

        //        foreach (var system in postUpdate)
        //        {
        //            system.OnUpdate();
        //        }

        //        Thread.Sleep(100);
        //    }
        //});

        //t.Start();

        if (HasResource<Window>())
        {
            ref var window = ref _resources.GetResource<Window>();
            Logger.Info<App>("Start window update");
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
        active = false;
        //t.Join();
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
