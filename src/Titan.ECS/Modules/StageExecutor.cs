using System.Runtime.CompilerServices;
using Titan.Core.Threading2;
using Titan.ECS.SystemsV2.Scheduler;

namespace Titan.ECS.Modules;

public unsafe struct StageExecutor
{
    public delegate*<in SystemExecutionGraph, in JobApi, void> RunFunc;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Run(in SystemExecutionGraph graph, in JobApi jobApi) => RunFunc(graph, jobApi);
}
