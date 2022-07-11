using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.App;
using Titan.Core.Memory;

namespace Titan.ECS.SystemsV2.Scheduler;

public static unsafe class SystemSchedulerFactory
{
    [SkipLocalsInit]
    internal static SystemExecutionStages Create(in PermanentMemory memory, in TransientMemory transientMemory, ReadOnlySpan<SystemDescriptor> descriptors, IApp app)
    {
        // Allocate memory needed to build the graph
        var nodeCount = (uint)descriptors.Length;
        var nodes = memory.GetPointer<SystemExecutionGraphNode>(nodeCount, true);
        var states = transientMemory.GetPointer<SystemDependencyState>(nodeCount, true);
        var dependencies = transientMemory.GetPointer<TempDeps>(nodeCount, true);

        // Sort the descriptors by the Stage, so they can easily be accessed.
        var sortedDescriptors = new Span<SystemDescriptor>(transientMemory.GetPointer<SystemDescriptor>(nodeCount, false), (int)nodeCount);
        descriptors.CopyTo(sortedDescriptors);
        sortedDescriptors.Sort(CompareDescriptor);

        const int stageCount = (int)Stage.Count;

        var stageCounter = stackalloc int[stageCount];
        Unsafe.InitBlock(stageCounter, 0, sizeof(int) * stageCount);

        // Create and init the system node
        for (var i = 0; i < nodeCount; i++)
        {
            var systemNode = SystemNode.CreateAndInit(i, memory, sortedDescriptors[i], new SystemsInitializer(app, &states[i]));
            nodes[i].System = systemNode;
            // Count the number of systems in each state
            stageCounter[(int)systemNode.Stage]++;
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
            if (dependencies[i].Count > 0)
            {
                system.Dependencies = memory.GetPointer<int>((uint)dependencies->Count);
                Unsafe.CopyBlockUnaligned(system.Dependencies, dependencies->Dependencies, (uint)(sizeof(uint) * dependencies->Count));
            }
        }

        // NOTE(Jens): add circular dependency detection
        // NOTE(Jens): Add sorting to set systems with the least amount of dependencies first

        // Go through each stage and create the execution graph.
        var graphs = memory.GetPointer<SystemExecutionGraph>(stageCount);
        var node = nodes;
        for (var i = 0; i < stageCount; ++i)
        {
            var count = stageCounter[i];
            graphs[i] = new SystemExecutionGraph(node, count);
            node += count;
        }
        return new SystemExecutionStages(graphs, stageCount);

        static int CompareDescriptor(SystemDescriptor l, SystemDescriptor r)
            => l.Stage - r.Stage;
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
