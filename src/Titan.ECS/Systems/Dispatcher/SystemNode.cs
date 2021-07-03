using System;
using Titan.ECS.Worlds;

namespace Titan.ECS.Systems.Dispatcher
{
    public readonly struct SystemNode
    {
        public readonly EntitySystem System;
        public readonly Action ExecuteFunc;
        public readonly Action<World> InitFunc;
        public readonly int[] Dependencies;
        public SystemNode(EntitySystem system, int[] dependencies)
        {
            System = system;
            ExecuteFunc = system.Update;
            InitFunc = system.InitSystem;
            Dependencies = dependencies;
        }
    }
}
