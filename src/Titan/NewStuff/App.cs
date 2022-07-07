using System;
using Titan.Core.App;
using Titan.Core.Events;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.ECS.SystemsV2;
using Titan.ECS.TheNew;
using Titan.Graphics.Modules;
using Titan.Modules;

namespace Titan.NewStuff;

public class App : IApp
{
    private readonly MemoryPool _pool;
    private readonly ResourceCollection _resourceCollection;
    private readonly SystemDescriptorCollection _systems;

    public static App Create(AppCreationArgs args)
    {
        var memoryPool = MemoryPool.Create(args.UnmanagedMemory);
        var resources = ResourceCollection.Create(args.GlobalResourcesMemory, args.GlobalResourcesTypes, memoryPool);
        var systems = SystemDescriptorCollection.Create(args.GlobalSystemTypes, memoryPool);

        return new(memoryPool, resources, systems);
    }

    private App(MemoryPool pool, ResourceCollection resourceCollection, SystemDescriptorCollection systems)
    {
        _pool = pool;
        _resourceCollection = resourceCollection;
        _systems = systems;
    }

    public IApp AddSystem<T>() where T : unmanaged, IStructSystem<T> => AddSystemToStage<T>(Stage.Update);
    public IApp AddSystemToStage<T>(Stage stage) where T : unmanaged, IStructSystem<T>
    {
        _systems.AddSystem<T>(stage);
        return this;
    }

    public IApp AddEvent<T>(uint maxEvents = 10) where T : unmanaged
    {
        if (!HasResource<PermanentMemory>())
        {
            throw new InvalidOperationException($"no {nameof(PermanentMemory)} resource have been registered. This is a part of the {nameof(CoreModule)}. Please add that as the first module.");
        }
        ref readonly var mem = ref GetResource<PermanentMemory>();
        return AddResource(new EventCollection<T>(maxEvents, mem))
            .AddSystemToStage<EventSystem<T>>(Stage.PreUpdate);
    }

    public IApp AddResource<T>(in T resource) where T : unmanaged
    {
        _resourceCollection.InitResource(resource);
        // NOTE(Jens): modules that add disposable components should also add a System to the "Shutdown" stage that will dispose that resource.
        return this;
    }

    public ref readonly T GetResource<T>() where T : unmanaged => ref _resourceCollection.GetResource<T>();
    public ref T GetMutableResource<T>() where T : unmanaged => ref _resourceCollection.GetResource<T>();
    public unsafe T* GetMutableResourcePointer<T>() where T : unmanaged => _resourceCollection.GetResourcePointer<T>();
    public bool HasResource<T>() where T : unmanaged => _resourceCollection.HasResource<T>();
    
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
    
            
            //var graph = SystemSchedulerFactory.Create(_worldMemory.As<PermanentMemory>(), _worldTransientMemory, initialWorld, this);

            //SystemSchedulerFactory.TestTHis(this, _resourceAllocator, initialWorld);
            // Create and init memory pool
            // Create and init world
            // Init components
            // Init the systems


            // Schedule world for running. Event?


        // Init systems, create execution grapt
        // call startup systems

        // start main loop (on a different thread)

        // start polling platform events

        // call terminate systems 


        if (HasResource<Window>() && HasResource<WindowApi>())
        {
            ref var window = ref _resourceCollection.GetResource<Window>();
            ref var windowApi = ref _resourceCollection.GetResource<WindowApi>();
            Logger.Info<App>("Start window update");
            while (windowApi.Update(window))
            {
                //Logger.Debug<App>($"Permanent: {_permanentMemory.Used}/{_permanentMemory.Size} bytes. Transient: {_transientMemory.Used}/{_transientMemory.Size} ");
                //Logger.Debug<App>($"World Permanent: {_worldMemory.Used}/{_worldMemory.Size} bytes. Transient: {_worldTransientMemory.Used}/{_worldTransientMemory.Size} ");
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
        // NOTE(Jens): execute systems in a shutdown procedure.
        _pool.Dispose();
    }
}
