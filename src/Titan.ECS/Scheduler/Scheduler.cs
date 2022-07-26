using System;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Threading2;
using Titan.ECS.Memory;
using Titan.ECS.Systems;
using Titan.ECS.Worlds;
using Titan.Memory;

namespace Titan.ECS.Scheduler;

/// <summary>
/// The scheduler is currently very simple. it has methods for Startup, Shutdown and Update. These will call different stages with different executors based on a configuration.
/// NOTE(Jens): in the future we might want a more dynamic scheduler with different stages, and maybe add "subsets" of stages.
/// NOTE(Jens): For example the EngineCoreStage.Update runs the scheduler for GameStages (PreUpdate, Update and PostUpdate).
/// </summary>
public unsafe struct Scheduler
{
    private int Apa;
    private NodeStage* _stages;
    private StageExecutor* _executors;
    private const int _stageCount = (int)Stage.Count;

    public void Update(ref JobApi jobApi, ref World world)
    {
        Execute((int)Stage.First, jobApi);
        Execute((int)Stage.PreUpdate, jobApi);
        Execute((int)Stage.Update, jobApi);
        Execute((int)Stage.PostUpdate, jobApi);
    }

    public void Shutdown(ref JobApi jobApi, ref World world)
    {
        Logger.Trace<Scheduler>("Shutdown");
        Execute((int)Stage.Shutdown, jobApi);
        Execute((int)Stage.PostShutdown, jobApi);
    }

    public void Startup(ref JobApi jobApi, ref World world)
    {
        Logger.Trace<Scheduler>("Startup");
        Execute((int)Stage.PreStartup, jobApi);
        Execute((int)Stage.Startup, jobApi);

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Execute(int stage, in JobApi jobApi) 
        => _executors[stage].Run(_stages[stage], jobApi);

    internal void Init(in ResourceCollection resources, ref World world)
    {
        //NOTE(Jens): Rewrite this method to something more readable
        //NOTE(Jens): it currently allocates new memory for each system when they are initialized, we should calculate the size needed and allocate everything in a single call. This is to prevent memory being allocated in different places.

        var config = resources.GetResource<SchedulerConfiguration>();

        var descriptors = resources
            .GetResource<SystemsRegistry>()
            .GetDescriptors();
        Logger.Trace<Scheduler>($"Found {descriptors.Length} {nameof(SystemDescriptor)}.");

        var count = (uint)descriptors.Length;

        ref var pool = ref resources.GetResource<PlatformAllocator>();
        ref var transient = ref resources.GetResource<TransientMemory>();

        // We sort the descriptors before we initialize them so we can easily access them in the correct Stage order
        var sortedDescriptors = new Span<SystemDescriptor>(transient.AllocateArray<SystemDescriptor>(count), (int)count);
        descriptors.CopyTo(sortedDescriptors);
        sortedDescriptors.Sort(CompareDescriptor);

        // allocate memory for the nodes
        _stages = pool.Allocate<NodeStage>(_stageCount);
        _executors = pool.Allocate<StageExecutor>(_stageCount);
        config.Get().CopyTo(new Span<StageExecutor>(_executors, _stageCount));

        var nodes = pool.Allocate<Node>(count, true);
        var states = transient.AllocateArray<SystemDependencyState>(count, true);

        var stageCounter = transient.AllocateArray<int>(_stageCount);
        
        // Create and initialize the systems (and record the dependencies)
        for (var i = 0; i < count; ++i)
        {
            var initializer = new SystemsInitializer(ref states[i], ref world);
            nodes[i] = CreateAndInit(pool, sortedDescriptors[i], initializer);
            stageCounter[(int)nodes[i].Stage]++;
        }

        // Calculate the dependencies
        var dependencies = transient.AllocateArray<int>(count);
        for (var i = 0; i < count; ++i)
        {
            var dependenciesCount = 0;
            ref readonly var systemState = ref states[i];
            for (var j = 0; j < count; ++j)
            {
                if (i == j)
                {
                    continue;
                }
                if (nodes[i].Stage != nodes[j].Stage)
                {
                    continue;
                }
                // NOTE(Jens): this has not been implemented yet.
                var dependencyType = systemState.DependsOn(states[j]);
                if (dependencyType is DependencyType.OneWay or DependencyType.TwoWay)
                {
                    //NOTE(Jens): We might need to handle these dependencies in some other way.
                    //Logger.Info<Scheduler>($"System {node[i].}");
                    dependencies[dependenciesCount++] = j;
                }
            }

            // Allocate a new array on the permanent memory pool and copy the dependencies
            if (dependenciesCount > 0)
            {
                ref var systemNode = ref nodes[i];
                systemNode.DependenciesCount = dependenciesCount;
                //NOTE(Jens): THIS WILL ALLOCATE MORE MEMORY, possible Fragmentation 
                systemNode.Dependencies = pool.Allocate<int>((uint)dependenciesCount);
                Unsafe.CopyBlockUnaligned(systemNode.Dependencies, dependencies, (uint)(sizeof(int) * dependenciesCount));
                Logger.Trace<Scheduler>($"{systemNode.Stage}: System {nodes[i].Id} has {dependenciesCount} dependencies");
                for (var a = 0; a < systemNode.DependenciesCount; ++a)
                {
                    Logger.Trace<Scheduler>($"\tDependency {nodes[systemNode.Dependencies[a]].Id}");
                }
            }
            else
            {
                Logger.Trace<Scheduler>($"{nodes[i].Stage}: System {nodes[i].Id} has no dependencies");

            }

        }

        // Group the systems in Stages so they can easily be executed
        var offset = 0;
        var node = nodes;
        for (var i = 0; i < _stageCount; ++i)
        {
            var numberOfNodes = stageCounter[i];
            _stages[i] = new NodeStage(node, numberOfNodes);
            UpdateNodeDependenciesIndex(node, numberOfNodes, offset);

            node += numberOfNodes;
            offset += numberOfNodes;

        }

        static void UpdateNodeDependenciesIndex(Node* nodes, int count, int offset)
        {
            //NOTE(Jens): We group the nodes into stages so we need to update the offsets in the dependencies array or they'll try to access things that are out of bounds.
            foreach (ref var node in new Span<Node>(nodes, count))
            {
                for (var i = 0; i < node.DependenciesCount; ++i)
                {
                    node.Dependencies[i] -= offset;
                }
            }
        }
        static Node CreateAndInit(in PlatformAllocator allocator, in SystemDescriptor descriptor, in SystemsInitializer initializer)
        {
            //NOTE(Jens): THIS WILL ALLOCATE NEW  MEMORY FOR EACH SYSTEM. We dont want that. TODO: Fix this asap, it should be taken from the same memory (FixedSizeLinearArena)
            //NOTE(Jens): add try/catch since it will be calling user code?
            var context = allocator.Allocate(descriptor.Size);
            descriptor.Init(context, initializer);
            return new Node
            {
                Id = descriptor.Id,
                Criteria = descriptor.Criteria,
                Stage = descriptor.Stage,
                Context = context,
                ShouldRunFunc = descriptor.ShouldRun,
                UpdateFunc = descriptor.Update,
                DependenciesCount = 0,
                Dependencies = null
            };
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
    }
}
