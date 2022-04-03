using System;

namespace Titan.ECS.TheNew;

public readonly struct Node
{
    public readonly BaseSystem System;
    public readonly int[] Dependencies;
    public readonly Action OnUpdate;

    public Node(BaseSystem system, int[] dependencies)
    {
        System = system;
        OnUpdate = System.OnUpdate;
        Dependencies = dependencies;
    }
}
