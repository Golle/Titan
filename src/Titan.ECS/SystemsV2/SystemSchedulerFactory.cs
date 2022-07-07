using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Memory;

namespace Titan.ECS.SystemsV2;

public static unsafe class SystemSchedulerFactory
{
    [SkipLocalsInit]
    public static SystemExecutionGraph Create(in PermanentMemory memory, in TransientMemory transientMemory, WorldConfig config, IApp app)
    {
        var descriptors = config.SystemDescriptors;
        // Allocate memory needed to build the graph
        var nodeCount = (uint)descriptors.Count;
        var nodes = memory.GetPointer<SystemExecutionGraphNode>(nodeCount, true);
        var states = transientMemory.GetPointer<SystemDependencyState>(nodeCount, true);
        var dependencies = transientMemory.GetPointer<TempDeps>(nodeCount, true);

        // Create and init the system node
        for (var i = 0; i < nodeCount; i++)
        {
            nodes[i].System = SystemNode.CreateAndInit(i, memory, descriptors[i], new SystemsInitializer(app, &states[i]));
        }

        // Calculate the dependencies
        for (var i = 0; i < nodeCount; ++i)
        {
            for (var j = 0; j < nodeCount; ++j)
            {
                if (j == i) // Ignore self
                {
                    continue;
                }

                if (states[i].DependsOn(states[j]))
                {
                    dependencies[i].Add(j);
                }
            }
        }

        // Copy the dependencies to smaller arrays allocated from the permanent memory pool
        for (var i = 0; i < nodeCount; ++i)
        {
            ref var system = ref nodes[i];
            if(dependencies[i].Count > 0)
            {
                system.Dependencies = memory.GetPointer<int>((uint)dependencies->Count);
                Unsafe.CopyBlockUnaligned(system.Dependencies, dependencies->Dependencies, (uint)(sizeof(uint)*dependencies->Count));
            }
        }

        // NOTE(Jens): add circular dependency detection
        // NOTE(Jens): Add sorting to set systems with the least amount of dependencies first
        return new SystemExecutionGraph(nodes, nodeCount);
    }

    /// <summary>
    /// Used to calculate the dependencies
    /// </summary>
    internal struct TempDeps
    {
        private const int MaxDependencies = 20;
        public int Count;
        public fixed int Dependencies[MaxDependencies];
        public void Add(int index)
        {
            Debug.Assert(Count < MaxDependencies);
            Dependencies[Count++] = index;
        }
    }
}

public readonly unsafe struct SystemExecutionGraph
{
    private readonly SystemExecutionGraphNode* _nodes;
    private readonly uint _count;
    internal SystemExecutionGraph(SystemExecutionGraphNode* nodes, uint count)
    {
        _nodes = nodes;
        _count = count;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<SystemExecutionGraphNode> GetNodes() => new(_nodes, (int)_count);
}

internal unsafe struct SystemExecutionGraphNode
{
    public SystemNode System;
    public int* Dependencies;
    public int DependenciesCount;
}

