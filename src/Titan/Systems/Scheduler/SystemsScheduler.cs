using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Memory.Allocators;
using Titan.Jobs;
using Titan.Resources;
using Titan.Systems.Scheduler.Executors;

namespace Titan.Systems.Scheduler;

internal enum DependencyType
{
    None,
    OneWay,
    TwoWay
}

internal unsafe class SystemsScheduler : IScheduler
{
    private TitanArray<NodeStage> _stages;
    private TitanArray<SystemNode> _systems;
    private ILinearAllocator _allocator;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnUpdate(IJobApi jobApi)
    {
        _stages[(int)SystemStage.First].Execute(jobApi);
        _stages[(int)SystemStage.PreUpdate].Execute(jobApi);
        _stages[(int)SystemStage.Update].Execute(jobApi);
        _stages[(int)SystemStage.PostUpdate].Execute(jobApi);
        _stages[(int)SystemStage.Last].Execute(jobApi);
    }

    public bool Init(IMemoryManager memoryManager, SystemDescriptor[] descriptors, SystemExecutors executors, ResourceCollection resources)
    {
        const uint stageCount = (uint)SystemStage.Count;
        var systemCount = (uint)descriptors.Length;

        // sort the descriptors, so we add them to the array on the correct order and no reodering is required. (based on stage and priority)
        descriptors.AsSpan().Sort(CompareDescriptor);

        var totalSize = MemoryUtils.MegaBytes(16);
        Logger.Trace<SystemsScheduler>($"Reserving {totalSize} bytes of memory for the Systems and it's dependencies (HARDCODED, make it configurable)");
        if (!memoryManager.TryCreateLinearAllocator(AllocatorStrategy.Permanent, totalSize, out var allocator))
        {
            Logger.Error<SystemsScheduler>($"Failed to create a linear allocator with {totalSize} bytes.");
            return false;
        }

        // Allocate memory for all systems/stages, call init on them
        var stages = allocator.AllocArray<NodeStage>(stageCount);
        var systems = allocator.AllocArray<SystemNode>(systemCount);

        // Initialize all systems and store the state(Dependencies)
        var states = new SystemDependencyState[systemCount];
        var stageCounter = new int[stageCount];
        for (var i = 0; i < systemCount; ++i)
        {
            ref readonly var desc = ref descriptors[i];
            var context = allocator.Alloc(desc.Size, true);
            var initializer = new SystemInitializer(ref states[i], resources, allocator);
            desc.Init(context, initializer);
            systems[i] = new SystemNode
            {
                Context = context,
                Criteria = desc.Criteria,
                ShouldRunFunc = desc.ShouldRun,
                ShutdownFunc = desc.Shutdown,
                UpdateFunc = desc.Update,
                Id = desc.Id,
                Stage = desc.Stage,
                Dependencies = null,
                DependenciesCount = 0
            };
            stageCounter[(int)desc.Stage]++;
        }

        // Calculate dependencies
        {
            var dependencies = stackalloc int[(int)systemCount]; // temp array for storing dependencies
            for (var outer = 0; outer < systemCount; ++outer)
            {
                var dependencyCount = 0u;
                ref readonly var systemState = ref states[outer];
                for (var inner = 0; inner < systemCount; ++inner)
                {
                    // Don' compare with self
                    if (outer == inner)
                    {
                        continue;
                    }
                    // don't check for dependencies between stages. that could potentially cause a deadlock and doesn't really make any sense anyway.
                    if (systems[outer].Stage != systems[inner].Stage)
                    {
                        continue;
                    }
                    var dependencyType = systemState.DependsOn(states[inner]);
                    if (dependencyType is DependencyType.OneWay or DependencyType.TwoWay)
                    {
                        //NOTE(Jens): We might need to handle these dependencies in some other way.
                        if (IsCircular(systems, inner, outer))
                        {
                            Logger.Warning<SystemsScheduler>($"System {systems[outer].Id} has a circular dependency to {systems[inner].Id}. The system will not be added to the dependency list and will be executed before the other system");
                        }
                        else
                        {
                            dependencies[dependencyCount++] = inner;
                        }
                    }
                }

                if (dependencyCount > 0)
                {
                    ref var system = ref systems[outer];
                    system.DependenciesCount = dependencyCount;
                    system.Dependencies = allocator.Alloc<int>(dependencyCount);
                    MemoryUtils.Copy(system.Dependencies, dependencies, sizeof(int) * dependencyCount);
#if DEBUG
                    Logger.Trace<SystemsScheduler>($"{system.Stage}: System {system.Id} has {dependencyCount} dependencies");
                    for (var a = 0; a < system.DependenciesCount; ++a)
                    {
                        Logger.Trace<SystemsScheduler>($"\tDependency {systems[system.Dependencies[a]].Id}");
                    }
#endif
                }
            }
        }

        {
            // Store the pointer and the count
            var offset = 0;
            var node = systems.GetPointer();
            for (var i = 0; i < stageCount; ++i)
            {
                var numberOfNodes = stageCounter[i];
                Logger.Trace<SystemsScheduler>($"Stage {(SystemStage)i} ({numberOfNodes} systems)");
                var executor = executors.GetExecutor((SystemStage)i);
                stages[i] = new NodeStage(node, numberOfNodes, executor);
                // Recalculate the offsets since the index of the dependencies wont be the same
                UpdateNodeDependenciesIndex(node, numberOfNodes, offset);
                node += numberOfNodes;
                offset += numberOfNodes;
            }
        }

        _allocator = allocator;
        _systems = systems;
        _stages = stages;
        return true;

        static void UpdateNodeDependenciesIndex(SystemNode* nodes, int count, int offset)
        {
            //NOTE(Jens): We group the nodes into stages so we need to update the offsets in the dependencies array or they'll try to access things that are out of bounds.
            foreach (ref var node in new Span<SystemNode>(nodes, count))
            {
                for (var i = 0; i < node.DependenciesCount; ++i)
                {
                    node.Dependencies[i] -= offset;
                }
            }
        }

        static int CompareDescriptor(SystemDescriptor l, SystemDescriptor r)
        {
            var stageDiff = l.Stage - r.Stage;
            if (stageDiff != 0)
            {
                return stageDiff;
            }

            // The priority will determine which order the system will be executed. This will only affect systems that are executed with the Sequential/ReversedSequential executor. 
            if (l.Priority == r.Priority)
            {
                return 0;
            }

            if (l.Priority > r.Priority)
            {
                return -1;
            }
            return 1;
        }

        static bool IsCircular(in TitanArray<SystemNode> nodes, int current, int id)
        {
            ref readonly var node = ref nodes[current];
            for (var i = 0; i < node.DependenciesCount; ++i)
            {
                if (node.Dependencies[i] == id)
                {
                    return true;
                }

                if (IsCircular(nodes, node.Dependencies[i], id))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public void Shutdown()
    {
        if (_allocator != null)
        {
            foreach (ref var system in _systems.AsSpan())
            {
                system.Shutdown();
            }
            _allocator.Destroy();
            _systems = default;
            _allocator = null;
        }
    }
}
