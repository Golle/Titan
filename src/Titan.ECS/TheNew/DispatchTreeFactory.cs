using System;
using System.Collections.Generic;
using System.Linq;

namespace Titan.ECS.TheNew;

public class DispatchTreeFactory
{
    public Node[] Construct(BaseSystem_[] systems)
    {
        //DebugPrint1(systems);
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
                        Console.WriteLine("Circular dependency");
                    }
                    else
                    {
                        dependencies.Add(j);
                    }
                }
            }
            nodes[i] = new Node
            {
                System = system,
                Dependencies = dependencies.ToArray()
            };
        }
        foreach (var node in nodes)
        {
            var dependencies = string.Join(", ", node.Dependencies.Select(d => nodes[d].System.GetType().Name));
            Console.WriteLine(string.IsNullOrWhiteSpace(dependencies) ?
                $"{node.System.GetType().Name} has no dependencies" :
                $"{node.System.GetType().Name} depends on {dependencies}");
        }
        return nodes;
    }

    private static void DebugPrint1(BaseSystem_[] systems)
    {
        foreach (var system in systems)
        {
            Console.WriteLine(system.GetType().Name);
            var (readOnly, mutable, _dependsOn) = system.GetDependencies();
            Console.WriteLine($"Depends on(Read): {string.Join(", ", readOnly)}");
            Console.WriteLine($"Depends on(Mutable): {string.Join(", ", mutable)}");
            Console.WriteLine($"Depends on(System): {string.Join(", ", _dependsOn.Select(t => t.Name))}");
        }
    }
}
