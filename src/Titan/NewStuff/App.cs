using System.Collections.Generic;
using System.Linq;
using Titan.Core.App;
using Titan.Core.Events;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.SystemsV2;
using Titan.ECS.TheNew;
using Titan.Graphics.Modules;

namespace Titan.NewStuff;

public class App : IApp
{
    private readonly NativeMemoryPool _pool;
    private readonly UnmanagedResources _resources;
    private readonly PermanentMemory _resourceAllocator;


    private readonly List<WorldConfig> _worldConfigs = new();
    private readonly SystemDescriptorCollection _systems = new();

    private readonly List<IDisposable> _disposables = new();

    public static App Create(AppCreationArgs args)
        => new(args);

    private App(AppCreationArgs args)
    {
        _pool = new NativeMemoryPool(args.UnmanagedMemory);
        _resourceAllocator = _pool.CreateAllocator<PermanentMemory>(args.GlobalResourcesMemory);
        _resources = new UnmanagedResources(args.GlobalResourcesTypes, _resourceAllocator);
    }

    public IApp AddSystem<T>() where T : unmanaged, IStructSystem<T> => AddSystemToStage<T>(Stage.Update);
    public IApp AddSystemToStage<T>(Stage stage) where T : unmanaged, IStructSystem<T>
    {
        _systems.AddSystem<T>(stage);
        return this;
    }

    public IApp AddEvent<T>(uint maxEvents = 10) where T : unmanaged =>
        AddResource(new EventCollection<T>(maxEvents, _resourceAllocator))
            .AddSystemToStage<EventSystem<T>>(Stage.PreUpdate);

    public IApp AddWorld<T>() where T : IWorldModule => AddWorld(T.Build);
    public IApp AddWorld(Action<WorldConfig> config)
    {
        var worldConfig = new WorldConfig();
        config(worldConfig);
        _worldConfigs.Add(worldConfig);
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
        // NOTE(Jens): this is something we should change. not a nice way to dispose/shutdown things. I think the modules should have a teardown function (or be able to regsiter a teardown function)
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

        var initialWorld = _worldConfigs.FirstOrDefault();
        if (initialWorld != null)
        {
            SystemSchedulerFactory.TestTHis(this, _resourceAllocator, initialWorld);
            // Create and init memory pool
            // Create and init world
            // Init components
            // Init the systems


            // Schedule world for running. Event?
        }
        else
        {
            Logger.Warning<App>($"No worlds have been added, systems will not be executed.");
        }

       
        // Init systems, create execution grapt
        // call startup systems

        // start main loop (on a different thread)

        // start polling platform events

        // call terminate systems 


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


        if (HasResource<Window>() && HasResource<WindowApi>())
        {
            ref var window = ref _resources.GetResource<Window>();
            ref var windowApi = ref _resources.GetResource<WindowApi>();
            Logger.Info<App>("Start window update");
            while (windowApi.Update(window))
            {

                // noop
            }
        }
        else
        {
            Logger.Info<App>("No window has been created.");
        }


        Logger.Info<App>("Exiting");
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
        _pool.Dispose();
    }
}
