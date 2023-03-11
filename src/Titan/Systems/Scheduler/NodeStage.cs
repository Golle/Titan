using System.Runtime.CompilerServices;
using Titan.Jobs;
using Titan.Systems.Scheduler.Executors;

namespace Titan.Systems.Scheduler;

internal readonly unsafe struct NodeStage
{
    public readonly SystemExecutor Executor;
    public readonly SystemNode* Nodes;
    public readonly int Count;

    public NodeStage(SystemNode* nodes, int count, in SystemExecutor executor)
    {
        Nodes = nodes;
        Count = count;
        Executor = executor;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Execute(IJobApi jobApi) => Executor.Func(jobApi, Nodes, Count);
}
