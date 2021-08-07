using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.ECS.Worlds;

namespace Titan.ECS.Systems
{
    public abstract class EntitySystem
    {
        private ComponentId _read;
        private ComponentId _mutable;
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

        protected abstract void Init();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void OnPreUpdate(){}

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
            OnUpdate(new Timestep(1f));
            OnPostUpdate();
            s.Stop();
            EngineStats.SetStats(_name, s.Elapsed.TotalMilliseconds);
        }

#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Update()
        {
            OnPreUpdate();
            OnUpdate(new Timestep(1f));
            OnPostUpdate();
        }
#endif
        internal void InitSystem(World world)
        {
            _world = world;
            EntityManager = world.Manager;
            Init();
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
    


        


    public readonly struct Timestep
    {
        public readonly float ElapsedTime;
        public Timestep(float elapsedTime)
        {
            ElapsedTime = elapsedTime;
        }
    }
}
