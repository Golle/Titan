using System;
using System.Collections.Generic;
using System.Linq;
using Titan.Core.Logging;
using Titan.ECS.Worlds;

namespace Titan.ECS.TheNew;

public static class DispatchTreeFactory
{
    public static Node[] Construct(EntitySystemConfiguration[] entitySystems, SystemsConfiguration config)
    {
        var systems = entitySystems
            .Select(e => e.Type)
            .Concat(config.Types)
            .Select(Activator.CreateInstance)
            .Cast<BaseSystem>()
            .ToArray();

    
        var nodes = new Node[systems.Length];

        for (var i = 0; i < systems.Length; ++i)
        {
            var system = systems[i];
            var (readOnly, mutable, _dependsOn) = system.GetDependencies();
            var dependencies = new HashSet<int>();

            for (var j = 0; j < systems.Length; ++j)
            {
                if (j == i)
                {
                    continue;
                }
                //NOTE(Jens): We only look for dependencies where one system has Mutable access or explicit System dependency.
                var hasReadOnlyReference = readOnly.Any(r => systems[j].GetDependencies().Mutable.Contains(r));
                var hasMutableReference = mutable.Any(r => systems[j].GetDependencies().Mutable.Contains(r));
                var dependsOn = _dependsOn.Any(r => systems[j].GetType() == r);

                if (hasMutableReference || hasReadOnlyReference || dependsOn)
                {
                    //NOTE(Jens): Check for a Circular Dependency (this only works for 2 systems, it wont detect System1 depends on System2 which depends on System3 which depends on System1. Those will create a Deadlock in the runner)
                    //TODO(Jens): Add more advanced check for Circular dependencies, by create a directed graph and step through it.
                    var circularDependency = nodes[j].Dependencies?.Contains(i) ?? false;
                    if (circularDependency)
                    {
                        Logger.Warning($"Circular dependency detected between {system.GetType().Name} and {nodes[j].System.GetType().Name}", typeof(DispatchTreeFactory));
                    }
                    else
                    {
                        dependencies.Add(j);
                    }
                }
            }
            nodes[i] = new Node(system, dependencies.ToArray());
        }
        foreach (var node in nodes)
        {
            var dependencies = string.Join(", ", node.Dependencies.Select(d => nodes[d].System.GetType().Name));
            Logger.Trace(string.IsNullOrWhiteSpace(dependencies) ?
                $"{node.System.GetType().Name} has no dependencies" :
                $"{node.System.GetType().Name} depends on {dependencies}", typeof(DispatchTreeFactory));
        }
        return nodes;
    }
}
