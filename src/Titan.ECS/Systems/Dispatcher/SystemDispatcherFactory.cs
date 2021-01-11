using System.Collections.Generic;
using System.Linq;

namespace Titan.ECS.Systems.Dispatcher
{
    internal class SystemDispatcherFactory
    {
        public SystemsDispatcher Create(in SystemBase[] systems)
        {
            var count = systems.Length;
            var sortedSystems = systems.OrderBy(s => s.Priority).ToArray();
            var nodes = new SystemNode[count];
            for (var i = 0; i < count; ++i)
            {
                var system = sortedSystems[i];
                List<int> dependencies = new();
                for (var j = 0; j < count; ++j)
                {
                    // Can't depend on self
                    if (i == j) 
                    {
                        continue;
                    }

                    var hasReadOnlyDependency = system.Read.Contains(sortedSystems[j].Mutable);
                    var hasMutableDependency = j < i && system.Mutable.Contains(sortedSystems[j].Mutable); // Only look at systems before the current one

                    if (hasReadOnlyDependency || hasMutableDependency)
                    {
                        dependencies.Add(j);
                    }
                }
                nodes[i] = new SystemNode(system, dependencies.ToArray());
            }

            return new SystemsDispatcher(nodes);
        }
    }
}
