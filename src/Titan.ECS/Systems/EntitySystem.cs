using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Services;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Worlds;

namespace Titan.ECS.Systems
{
    public abstract class EntitySystem
    {
        private ComponentId _read;
        private ComponentId _mutable;
        private GameTime _gameTime;
        private World _world;
        private readonly string _name;
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
            OnUpdate(new Timestep(1/144f)); // TODO: add support for timestep
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
        internal void InitSystem(World world, IServiceCollection services)
        {
            _world = world;
            _gameTime = world.GameTime;

            EntityManager = world.Manager;
            Init(services);
            _world = null;
        }

        

        public ReadOnlyStorage<T> GetReadOnly<T>() where T : unmanaged
        {
            if (_world == null)
            {
                throw new InvalidOperationException($"{nameof(GetReadOnly)} can only be called in the {nameof(Init)} method.");
            }
            var pool = _world.Registry.GetPool<T>();
            _read += ComponentId<T>.Id;
            return new ReadOnlyStorage<T>(pool);
        }

        protected MutableStorage<T> GetMutable<T>() where T : unmanaged
        {
            if (_world == null)
            {
                throw new InvalidOperationException($"{nameof(GetMutable)} can only be called in the {nameof(Init)} method.");
            }
            var pool = _world.Registry.GetPool<T>();
            _mutable += ComponentId<T>.Id;
            return new MutableStorage<T>(pool);
        }

        protected EntityFilter CreateFilter(EntityFilterConfiguration config)
        {
            if(_world == null)
            {
                throw new InvalidOperationException($"{nameof(CreateFilter)} can only be called in the {nameof(Init)} method.");
            }
            return _world.FilterManager.Create(config);
        }
    }
}
