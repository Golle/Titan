using System;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Threading;

namespace Titan.ECS.Systems.Dispatcher;

// TODO: can this be optimized? can we use spans for nodes? build a better structure for the execution tree
public sealed class SystemsDispatcher
{
    private readonly SystemNode[] _nodes;
    private readonly JobProgress _progress;
    private readonly NodeStatus[] _status;
    private readonly Handle<WorkerPool>[] _handles;

    public SystemsDispatcher(SystemNode[] nodes)
    {
        _nodes = nodes;
        _progress = new JobProgress((uint)nodes.Length);
        _status = new NodeStatus[nodes.Length];
        _handles = new Handle<WorkerPool>[nodes.Length];
    }
    public void Execute()
    {
        _progress.Reset();
        Array.Fill(_status, NodeStatus.Waiting);

        //fixed (NodeStatus* s = _status)
        //{
        //    Unsafe.InitBlock(s, 0, (uint)(sizeof(NodeStatus) * _status.Length));
        //}
            
            
        // Put it in a local variable to avoid bounds checking
        var status = _status; 
        var nodes = _nodes;
        var handles = _handles;
        while (!_progress.IsComplete())
        {
            for (var i = 0; i < nodes.Length; ++i)
            {
                if (status[i] != NodeStatus.Waiting)
                {
                    continue;
                }
                    
                ref readonly var node = ref nodes[i];
                var isReady = true;
                foreach (var dependencyIndex in node.Dependencies)
                {
                    if (status[dependencyIndex] != NodeStatus.Completed)
                    {
                        isReady = false;
                        break;
                    }
                }
                if (isReady)
                {
                    status[i] = NodeStatus.Running;
                    handles[i] = WorkerPool.Enqueue(new JobDescription(node.Update, autoReset: false), _progress);
                }
            }
            ResetHandles();
        }
        // Make sure all handles have been reset
        ResetHandles();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ResetHandles()
    {
        var handles = _handles;
        var status = _status;
        for (var i = 0; i < handles.Length; ++i)
        {
            ref var handle = ref handles[i];
            if (handle.IsValid() && WorkerPool.IsCompleted(handle))
            {
                WorkerPool.Reset(ref handle);
                status[i] = NodeStatus.Completed;
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
