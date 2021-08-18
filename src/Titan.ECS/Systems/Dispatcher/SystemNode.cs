using System;

namespace Titan.ECS.Systems.Dispatcher
{
    public readonly struct SystemNode
    {
        public readonly EntitySystem System;
        public readonly Action Update;
        public readonly int[] Dependencies;
        public SystemNode(EntitySystem system, int[] dependencies)
        {
            System = system;
            Update = system.Update;
            Dependencies = dependencies;
        }
    }
}
