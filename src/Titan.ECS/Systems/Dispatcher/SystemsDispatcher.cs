using System;
using System.Threading;
using Titan.Core.Common;
using Titan.Core.Threading;

namespace Titan.ECS.Systems.Dispatcher
{
    public sealed class SystemsDispatcher
    {
        private readonly SystemNode[] _nodes;

        private readonly JobProgress _progress;
        private readonly NodeStatus[] _status;
        private readonly Handle<WorkerPool>[] _handles;

        public SystemsDispatcher(SystemNode[] nodes)
        {
            _nodes = nodes;
            _progress = new JobProgress((uint) nodes.Length);
            _status = new NodeStatus[nodes.Length];
            _handles = new Handle<WorkerPool>[nodes.Length];
        }

        public void Execute(WorkerPool pool)
        {
            _progress.Reset();
            Array.Fill(_status, NodeStatus.Waiting);

            while (!_progress.IsComplete())
            {
                for (var i = 0; i < _nodes.Length; ++i)
                {
                    if (_status[i] != NodeStatus.Waiting)
                    {
                        continue;
                    }
                    ref readonly var node = ref _nodes[i];
                    var isReady = true;
                    foreach (var dependencyIndex in node.Dependencies)
                    {
                        if (_status[dependencyIndex] != NodeStatus.Completed)
                        {
                            isReady = false;
                            break;
                        }
                    }
                    if (isReady)
                    {
                        _status[i] = NodeStatus.Running;
                        _handles[i] = pool.Enqueue(new JobDescription(node.ExecuteFunc, autoReset: false), _progress);
                    }
                }
                Thread.Yield();
                for (var i = 0; i < _handles.Length; ++i)
                {
                    ref var handle = ref _handles[i];
                    if (handle.IsValid() && pool.IsCompleted(handle))
                    {
                        pool.Reset(ref handle);
                        _status[i] = NodeStatus.Completed;
                    }
                }
            }
        }
        
        private enum NodeStatus
        {
            Waiting,
            Running,
            Completed
        }
    }
}
