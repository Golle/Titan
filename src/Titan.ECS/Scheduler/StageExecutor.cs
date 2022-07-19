using System.Runtime.CompilerServices;
using Titan.Core.Threading2;

namespace Titan.ECS.Scheduler;

public unsafe struct StageExecutor
{
    public delegate*<in NodeStage, in JobApi, void> RunFunc;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Run(in NodeStage stage, in JobApi jobApi) => RunFunc(stage, jobApi);
}
