using System;
using System.Runtime.CompilerServices;
using Titan.ECS.Components;
using Titan.ECS.World;

namespace Titan.ECS.Systems
{
    public abstract class SystemBase
    {
        private readonly IWorld _world;
        internal ref readonly ComponentId Read => ref _read;
        internal ref readonly ComponentId Mutable => ref _mutable;
        internal int Priority { get; }

        private ComponentId _read;
        private ComponentId _mutable;

        private bool _locked = false;
        protected SystemBase(IWorld world, int priority = 0)
        {
            Priority = priority;
            _world = world;
        }

        protected ReadOnlyStorage<T> GetRead<T>() where T : unmanaged
        {
            if (_locked)
            {
                throw new InvalidOperationException("Failed to acquire a ReadStorage because the System has been locked.");
            }

            _read += ComponentId<T>.Id;
            return new(_world.GetComponentPool<T>());
        }

        protected MutableStorage<T> GetMutable<T>() where T : unmanaged
        {
            if (_locked)
            {
                throw new InvalidOperationException("Failed to acquire a MutableStorage because the System has been locked.");
            }
            _mutable += ComponentId<T>.Id;
            return new(_world.GetComponentPool<T>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update()
        {
            OnPreUpdate();
            OnUpdate();
            OnPostUpdate();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void OnPreUpdate() { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void OnUpdate() { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual void OnPostUpdate() { }
    }
}
