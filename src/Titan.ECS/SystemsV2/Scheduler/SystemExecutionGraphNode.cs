using System.Runtime.CompilerServices;

namespace Titan.ECS.SystemsV2.Scheduler;

internal unsafe struct SystemExecutionGraphNode
{
    public SystemNode System;
    public int* Dependencies;
    public int DependenciesCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool ShouldRun() => System.ShouldRun(System.Instance);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Update() => System.Update(System.Instance);
}

