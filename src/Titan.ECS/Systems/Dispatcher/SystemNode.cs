using System;
using System.Numerics;

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
            ExecuteFunc = () =>
            {
                for (var i = 0; i < 100_000; ++i)
                {
                    var a = new Matrix4x4(1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8) * new Matrix4x4(1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8);
                }
                //Console.WriteLine("Execute system: {0}", system.GetType().Name);
                //Thread.Sleep(new Random().Next(500, 4000));
                //Console.WriteLine("Finished system: {0}", system.GetType().Name);
            };
            Dependencies = dependencies;
        }
    }
}