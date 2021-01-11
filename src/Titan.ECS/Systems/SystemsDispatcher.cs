using System;
using System.Collections;
using System.Threading;
using Titan.Core.Common;
using Titan.Core.Threading;

namespace Titan.ECS.Systems
{

    
    public sealed class SystemsDispatcher
    {
        private readonly SystemNode[] _nodes;
        private readonly JobProgress _progress;

        private readonly Handle<WorkerPool>[] _handles;

        public SystemsDispatcher(SystemNode[] nodes)
        {
            _nodes = nodes;
            _progress = new JobProgress((uint) nodes.Length);
            _handles = new Handle<WorkerPool>[nodes.Length];
        }

        
        public void Execute(WorkerPool pool)
        {
            _progress.Reset();
            Array.Fill(_handles, default);
            foreach (var node in _nodes)
            {
                pool.Enqueue(new JobDescription(node.ExecuteFunc), _progress);
            }
            _progress.Wait();
            //while (!_progress.IsComplete())
            //{
            //    for (var i = 0; i < _nodes.Length; ++i)
            //    {
            //        //QueueIfReady(i, pool);
            //    }

                
            //    Thread.Yield();
            //}

        }

        private void QueueIfReady(int index, WorkerPool pool)
        {
            // This doesn't work :<
            ref readonly var node = ref _nodes[index];
            foreach (var dependencyIndex in node.Dependencies)
            {
                if (!pool.IsCompleted(_handles[dependencyIndex]))
                {
                    return;
                }
            }

            _handles[index] = pool.Enqueue(new JobDescription(node.ExecuteFunc), _progress);

        }
    }
    

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
                //Console.WriteLine("Execute system: {0}", system.GetType().Name);
                //Thread.Sleep(new Random().Next(500, 4000));
                //Console.WriteLine("Finished system: {0}", system.GetType().Name);
            };
            Dependencies = dependencies;
        }
    }
}
