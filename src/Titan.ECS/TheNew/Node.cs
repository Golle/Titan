using System;

namespace Titan.ECS.TheNew;

public readonly struct Node
{
    public readonly BaseSystem System;
    public readonly int[] Dependencies;
    public readonly Action Update;

    public Node(BaseSystem system, int[] dependencies)
    {
        System = system;
        Update = System.Update;
        Dependencies = dependencies;
    }
}
