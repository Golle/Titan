using System;
using Titan.ECS.Entities;

namespace Titan.ECS.Components
{
    internal class SparseComponentPool<T> : IComponentPool<T> where T : unmanaged
    {
        public SparseComponentPool(uint maxEntities)
        {
            throw new NotImplementedException();
        }

        public ref T Create(in Entity entity, in T initialValue)
        {
            throw new NotImplementedException();
        }

        public ref T Create(in Entity entity)
        {
            throw new NotImplementedException();
        }

        public ref T Get(in Entity entity)
        {
            throw new NotImplementedException();
        }

        public ref T this[in Entity entity] => throw new NotImplementedException();

        public void Destroy(in Entity entity)
        {
            throw new NotImplementedException();
        }

        public void Destroy(uint entityId)
        {
            throw new NotImplementedException();
        }

        public bool Contains(in Entity entity)
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
