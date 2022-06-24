using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Services;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Systems;
using Titan.ECS.Systems.Dispatcher;


namespace Titan.ECS.Worlds;

public record WorldConfigurationOld
{
    public string Name { get; init; }
    public uint Id { get; init; }
    public float FixedTimeStep { get; init; } = 1f / 60f;
    public uint MaxEntities { get; init; }
    public ComponentConfiguration[] Components { get; init; }
    public EntitySystemConfiguration[] Systems { get; init; }
    public Action<World> Setup { get; init; }
    public Action<World> Teardown { get; init; }
}

public record ComponentConfiguration(Type Type, ComponentPoolTypes PoolType, uint Count = 0);
public record EntitySystemConfiguration(Type Type);

internal class SystemsManager
{
    private readonly EntitySystem[] _systems;
    private SystemsDispatcher _dispatcher;

    public SystemsManager(EntitySystemConfiguration[] systems)
    {
        _systems = systems
            .Select(config => (EntitySystem)Activator.CreateInstance(config.Type))
            .ToArray();
        Logger.Trace<SystemsManager>($"Created {systems.Length} systems");
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RunDispatcher() => _dispatcher.Execute();

    public void Init(World world, IServiceCollection services)
    {
        Logger.Trace<SystemsManager>("Init all systems");
        foreach (var system in _systems)
        {
            system.InitSystem(world, services);
        }
        Logger.Trace<SystemsManager>("Create system nodes and the SystemsDispatcher");
        var nodes = SystemNodeFactory.Create(_systems);
        _dispatcher = new SystemsDispatcher(nodes);
    }
}

public class World : IDisposable
{
    internal uint Id => Config.Id;
    internal GameTime GameTime { get; }
    internal EntityManager Manager { get; }
    internal EntityInfoManager InfoManager { get; }
    internal ComponentRegistry Registry { get; }
    internal WorldConfigurationOld Config { get; }
    internal EntityFilterManager FilterManager { get; }
        

    private static readonly IdContainer WorldIds = new(100);
    private static readonly World[] Worlds = new World[100];
    private static uint ActiveWorldId;

    private readonly SystemsManager _systems;
    private World(WorldConfigurationOld config, IServiceCollection services)
    {
        Config = config with {Id = WorldIds.Next() };
        Logger.Info<World>($"Creating world {Id}");
        Manager = new(Config);
        Registry = new (Config);
        InfoManager = new(Config);
        FilterManager = new(Config, InfoManager);
        GameTime = new(Config);
        _systems = new(config.Systems);
        Worlds[Id] = this;


        _systems.Init(this, services);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AttachEntity(in Entity parent, in Entity entity) => Manager.Attach(parent, entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DetachEntity(in Entity entity) => Manager.Detach(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DestroyEntity(in Entity entity) => Manager.Destroy(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Entity CreateEntity() => Manager.Create();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddComponent<T>(in Entity entity) where T : unmanaged => Registry.GetPool<T>().Create(entity);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddComponent<T>(in Entity entity, in T value) where T : unmanaged => Registry.GetPool<T>().Create(entity, value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveComponent<T>(in Entity entity) where T : unmanaged
    {
        // TODO: should this be done in 2 frames? flag for deletion, and delete in next frame?
        Registry.GetPool<T>().Destroy(entity);
    }

    private void Update(bool isCurrentlyActive)
    {
        // The order of execution here is important
        InfoManager.Update();
        GameTime.Update();
        Manager.Update();
        Registry.Update();

        // Filter should be executed last (before dispatcher)
        FilterManager.Update();
        if (isCurrentlyActive) // Don't run the Systems when it's not active (This is something i think we want to change)
        {
            _systems.RunDispatcher();
        }
    }

    internal static World GetWorldById(uint worldId)
    {
        var world = Worlds[worldId];
#if DEBUG
        if (world == null)
        {
            throw new InvalidOperationException($"World with id {worldId} does not exist.");
        }
#endif
        return world;
    }

    public void Dispose()
    {
        Logger.Trace<World>($"Disposing world {Id}");
        Manager.Dispose();
        InfoManager.Dispose();
        FilterManager.Dispose();
        Worlds[Id] = null;
        WorldIds.Return(Id);
    }


    public static World CreateWorld(WorldBuilderOld builder, IServiceCollection services, bool setActive) => CreateWorld(builder.Build("THIS IS NOT WORKDING"), services, setActive);

    public static World CreateWorld(WorldConfigurationOld config, IServiceCollection services, bool setActive)
    {
        var world = new World(config, services);

        if (setActive)
        {
            ActiveWorldId = world.Id;
        }
        return world;
    }

    public static void UpdateWorlds()
    {
        foreach (var world in Worlds)
        {
            world?.Update(world.Id == ActiveWorldId);
        }
    }

    public static void DisposeWorlds()
    {
        for (var i = 0; i < Worlds.Length; ++i)
        {
            ref var world = ref Worlds[i];
            if (world != null)
            {
                world.Dispose();
                world = null;
            }       
        }
    }

    public static void SetActive(World world)
    {
        // maybe do this some other way?
        ActiveWorldId = world.Id;
    }
}
