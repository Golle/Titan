using System;
using System.Collections.Generic;
using System.Linq;
using Titan.ECS.Systems;

namespace Titan
{
    public class SystemsCollection
    {
        internal List<EntitySystem> Systems { get; } = new();
        public SystemsCollection Add(EntitySystem system)
        {
            if(Systems.Any(s => s.GetType() == system.GetType()))
            {
                throw new InvalidOperationException($"Multiple systems of the same type added. {system.GetType().Name}");
            }
            Systems.Add(system);
            return this;
        }
    }
}
