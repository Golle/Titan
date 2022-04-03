using System;

namespace Titan.ECS.TheNew;

public readonly struct Node
{
    public readonly BaseSystem_ System;
    public readonly int[] Dependencies;
    public readonly Action OnUpdate;

    public Node(BaseSystem_ system, int[] dependencies)
    {
        System = system;
        OnUpdate = System.OnUpdate;
        Dependencies = dependencies;
    }
}
