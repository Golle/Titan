using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Titan.ECS.Components;
using Titan.ECS.Entities;

namespace Titan.ECS.World
{
    internal class EntityFilterManager : IEntityFilterManager, IDisposable
    {
        private readonly IList<EntityFilter> _filters = new List<EntityFilter>(100);

        private readonly uint _maxEntities;
        
        public EntityFilterManager(WorldConfiguration configuration)
        {
            _maxEntities = configuration.MaxEntities;
        }

        public IEntityFilter Create(EntityFilterConfiguration configuration)
        {
            var filter = _filters.FirstOrDefault(f => f.ComponentMask == configuration.ComponentMask); // Re-use existing filters
            if (filter == null)
            {
                _filters.Add(filter = new EntityFilter(configuration.ComponentMask, _maxEntities));
            }
            return filter;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EntityChanged(in Entity entity, in ComponentMask components)
        {
            foreach (var filter in _filters)
            {
                filter.OnEntityChanged(entity, components);
            }
        }

        public void Dispose()
        {
            foreach (var filter in _filters)
            {
                filter.Dispose();
            }
            _filters.Clear();
        }
    }
}
