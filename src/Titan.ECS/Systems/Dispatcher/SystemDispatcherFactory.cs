using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Titan.Core.Logging;

namespace Titan.ECS.Systems.Dispatcher
{
    internal static class SystemDispatcherFactory
    {
        public static SystemsDispatcher Create(in EntitySystem[] systems)
        {
            var count = systems.Length;
            var sortedSystems = systems.OrderByDescending(s => s.Priority).ToArray();
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

                    var hasReadOnlyDependency = system.Read.MatchesAny(sortedSystems[j].Mutable);
                    var hasMutableDependency = j < i && system.Mutable.MatchesAny(sortedSystems[j].Mutable); // Only look at systems before the current one

                    if (hasReadOnlyDependency || hasMutableDependency)
                    {
                        dependencies.Add(j);
                    }
                }
                nodes[i] = new SystemNode(system, dependencies.ToArray());
            }

            LogDependencies(nodes);
            return new SystemsDispatcher(nodes);
        }

        [Conditional("TRACE")]
        private static void LogDependencies(SystemNode[] nodes)
        {
            foreach (var node in nodes)
            {
                var dependencies = string.Join(", ", node.Dependencies.Select(d => nodes[d].System.GetType().Name));
                Logger.Trace(string.IsNullOrWhiteSpace(dependencies) ? 
                    $"{node.System.GetType().Name} has no dependencies" : 
                    $"{node.System.GetType().Name} depends on {dependencies}", typeof(SystemDispatcherFactory));
            }
        }
    }
}
