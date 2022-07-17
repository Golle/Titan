using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Services;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Systems.Resources;
using Titan.ECS.WorldsOld;

namespace Titan.ECS.Systems;


public abstract class EntitySystem
{
    private ComponentId _read;
    private ComponentId _mutable;
    private GameTime _gameTime;
    private WorldsOld.World _world;
    private bool _initialized;
    private readonly string _name;
    private ISharedResources _resources;

    internal ref readonly ComponentId Read => ref _read;
    internal ref readonly ComponentId Mutable => ref _mutable;
    protected EntityManager EntityManager { get; private set; }
        
    internal int Priority { get; }

    protected EntitySystem(int priority = 0)
    {
        Priority = priority;
            
        _name = GetType().Name;
    }

    protected abstract void Init(IServiceCollection services);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void OnPreUpdate(){}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void OnFixedUpdate(in Timestep timestep){}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected abstract void OnUpdate(in Timestep timestep);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void OnPostUpdate() { }

        
#if STATS
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Update()
    {
        var s = Stopwatch.StartNew();
        OnPreUpdate();
        EngineStats.SetSystemStats(_name, SystemStats.PreUpdate, s.Elapsed.TotalMilliseconds);
        s.Restart();
        var gametime = _gameTime.Current;
        if (gametime.FixedUpdateCalls > 0)
        {
            for (var i = 0; i < gametime.FixedUpdateCalls; ++i)
            {
                OnFixedUpdate(gametime.FixedUpdateDeltaTime);
            }
        }
        EngineStats.SetSystemStats(_name, SystemStats.FixedUpdate, s.Elapsed.TotalMilliseconds);
        s.Restart();
        OnUpdate(gametime.DeltaTime); // TODO: add support for timestep
        EngineStats.SetSystemStats(_name, SystemStats.Update, s.Elapsed.TotalMilliseconds);
        s.Restart();
        OnPostUpdate();
        EngineStats.SetSystemStats(_name, SystemStats.PostUpdate, s.Elapsed.TotalMilliseconds);
    }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Update()
        {
            OnPreUpdate();
            var gametime = _gameTime.Current;
            if (gametime.FixedUpdateCalls > 0)
            {
                for (var i = 0; i < gametime.FixedUpdateCalls; ++i)
                {
                    OnFixedUpdate(gametime.FixedUpdateDeltaTime);
                }
            }
            OnUpdate(gametime.DeltaTime);
            OnPostUpdate();
        }
#endif
    internal void InitSystem(WorldsOld.World world, IServiceCollection services)
    {
        _world = world;
        _gameTime = world.GameTime;

        EntityManager = world.Manager;
        _resources = services.Get<ISharedResources>();
        Init(services);
        _initialized = true;
    }


    protected ReadOnlyStorage<T> GetReadOnly<T>() where T : unmanaged
    {
        if (_initialized)
        {
            throw new InvalidOperationException($"{nameof(GetReadOnly)} can only be called in the {nameof(Init)} method.");
        }
        var pool = _world.Registry.GetPool<T>();
        _read += ComponentId<T>.Id;
        return new ReadOnlyStorage<T>(pool);
    }

    protected MutableStorage<T> GetMutable<T>() where T : unmanaged
    {
        if (_initialized)
        {
            throw new InvalidOperationException($"{nameof(GetMutable)} can only be called in the {nameof(Init)} method.");
        }
        var pool = _world.Registry.GetPool<T>();
        _mutable += ComponentId<T>.Id;
        return new MutableStorage<T>(pool);
    }

    protected EntityFilter CreateFilter(EntityFilterConfiguration config)
    {
        if(_initialized)
        {
            throw new InvalidOperationException($"{nameof(CreateFilter)} can only be called in the {nameof(Init)} method.");
        }
        return _world.FilterManager.Create(config);
    }


    protected ReadOnlyResource<T> GetReadOnlyResource<T>() where T : unmanaged
    {
        if (_initialized)
        {
            throw new InvalidOperationException($"{nameof(GetReadOnlyResource)} can only be called in the {nameof(Init)} method.");
        }

        unsafe
        {
            return new(_resources.GetMemoryForType<T>());
        }
    }

    protected MutableResource<T> GetMutableResource<T>() where T : unmanaged
    {
        if (_initialized)
        {
            throw new InvalidOperationException($"{nameof(GetMutableResource)} can only be called in the {nameof(Init)} method.");
        }

        unsafe
        {
            return new(_resources.GetMemoryForType<T>());
        }
    }

    protected Entity CreateEntity() => _world.CreateEntity();
}
