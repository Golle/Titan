using System;

namespace Titan.ECS.Systems.Dispatcher
{
    public readonly struct SystemNode
    {
        public readonly SystemBase System;
        public readonly Action ExecuteFunc;
        public readonly int[] Dependencies;
        public SystemNode(SystemBase system, int[] dependencies)
        {
            System = system;
            ExecuteFunc = system.Update;
            Dependencies = dependencies;
        }
    }
}
