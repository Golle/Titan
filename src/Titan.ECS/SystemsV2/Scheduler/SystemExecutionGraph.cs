using System;
using System.Runtime.CompilerServices;

namespace Titan.ECS.SystemsV2.Scheduler;

public unsafe struct SystemExecutionGraph
{
    private readonly SystemExecutionGraphNode* _nodes;
    private readonly int _count;
    internal SystemExecutionGraph(SystemExecutionGraphNode* nodes, int count)
    {
        _nodes = nodes;
        _count = count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal readonly ReadOnlySpan<SystemExecutionGraphNode> GetNodes() => new(_nodes, _count);
}
