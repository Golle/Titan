using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Threading;
using Titan.ECS.Worlds;

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

        public void Init(World world)
        {
            foreach (var systemNode in _nodes)
            {
                systemNode.InitFunc(world);
            }
        }

        public void Execute()
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
                        _handles[i] = WorkerPool.Enqueue(new JobDescription(node.ExecuteFunc, autoReset: false), _progress);
                    }
                }
                Thread.Yield();

                ResetHandles();
            }
            // Make sure all handles have been reset
            ResetHandles();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResetHandles()
        {
            for (var i = 0; i < _handles.Length; ++i)
            {
                ref var handle = ref _handles[i];
                if (handle.IsValid() && WorkerPool.IsCompleted(handle))
                {
                    WorkerPool.Reset(ref handle);
                    _status[i] = NodeStatus.Completed;
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
