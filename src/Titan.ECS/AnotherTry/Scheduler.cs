using System;
using System.Runtime.CompilerServices;
using Titan.Core.App;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Core.Threading2;
using Titan.ECS.Modules;
using Titan.ECS.SystemsV2;
using Titan.ECS.TheNew;

namespace Titan.ECS.AnotherTry;

/// <summary>
/// The scheduler is currently very simple. it has methods for Startup, Shutdown and Update. These will call different stages with different executors based on a configuration.
/// NOTE(Jens): in the future we might want a more dynamic scheduler with different stages, and maybe add "subsets" of stages.
/// NOTE(Jens): For example the EngineCoreStage.Update runs the scheduler for GameStages (PreUpdate, Update and PostUpdate).
/// </summary>
public unsafe struct Scheduler
{
    private NodeStage* _stages;
    private StageExecutor* _executors;
    private const int _stageCount = (int)Stage.Count;

    public void Update(ref JobApi jobApi, ref World world)
    {
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
        var config = resources.GetResource<SchedulerConfiguration>();
        
        var descriptors = resources
            .GetResource<SystemsRegistry>()
            .GetDescriptors();
        Logger.Trace<Scheduler>($"Found {descriptors.Length} {nameof(SystemDescriptor)}.");

        var count = (uint)descriptors.Length;

        ref var pool = ref resources.GetResource<MemoryPool>();
        ref var transient = ref resources.GetResource<MemoryAllocator>();

        // We sort the descriptors before we initialize them so we can easily access them in the correct Stage order
        var sortedDescriptors = new Span<SystemDescriptor>(transient.GetPointer<SystemDescriptor>(count), (int)count);
        descriptors.CopyTo(sortedDescriptors);
        sortedDescriptors.Sort(CompareDescriptor);

        // allocate memory for the nodes
        _stages = pool.GetPointer<NodeStage>(_stageCount);
        _executors = pool.GetPointer<StageExecutor>(_stageCount);
        config.Get().CopyTo(new Span<StageExecutor>(_executors, _stageCount));

        var nodes = pool.GetPointer<Node>(count, true);
        var states = transient.GetPointer<SystemDependencyState>(count, true);
        
        var stageCounter = stackalloc int[_stageCount];
        
        // Create and initialize the systems (and record the dependencies)
        for (var i = 0; i < count; ++i)
        {
            var initializer = new SystemsInitializer(ref states[i], ref world);
            nodes[i] = CreateAndInit(pool, sortedDescriptors[i], initializer);
            stageCounter[(int)nodes[i].Stage]++;
        }

        // Calculate the dependencies
        var dependencies = transient.GetPointer<int>(count);
        var dependenciesCount = 0;
        for (var i = 0; i < count; ++i)
        {
            ref readonly var systemState = ref states[i];
            for (var j = 0; j < count; ++j)
            {
                if (i == j)
                {
                    continue;
                }
                // NOTE(Jens): this has not been implemented yet.
                if (systemState.DependsOn(states[j]))
                {
                    dependencies[dependenciesCount++] = j;
                }
            }
            
            // Allocate a new array on the permanent memory pool and copy the dependencies
            if (dependenciesCount > 0)
            {
                ref var systemNode = ref nodes[i];
                systemNode.DependenciesCount = dependenciesCount;
                systemNode.Dependencies = pool.GetPointer<int>((uint)dependenciesCount);
                Unsafe.CopyBlockUnaligned(systemNode.Dependencies, dependencies, (uint)dependenciesCount);
            }
        }

        // Group the systems in Stages so they can easily be executed
        
        var node = nodes;
        for (var i = 0; i < _stageCount; ++i)
        {
            var numberOfNodes = stageCounter[i];
            _stages[i] = new NodeStage(node, numberOfNodes);
            node += numberOfNodes;
        }

        static Node CreateAndInit(in MemoryPool pool, in SystemDescriptor descriptor, in SystemsInitializer initializer)
        {
            //NOTE(Jens): add try/catch since it will be calling user code?
            var context = pool.GetPointer(descriptor.Size);
            descriptor.Init(context, initializer);
            return new Node
            {
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
            => l.Stage - r.Stage;
    }

}


public readonly unsafe struct NodeStage
{
    public readonly Node* Nodes;
    public readonly int Count;

    public NodeStage(Node* nodes, int count)
    {
        Nodes = nodes;
        Count = count;
    }
}

public unsafe struct Node
{
    public Stage Stage;
    public delegate*<void*, void> UpdateFunc;
    public delegate*<void*, bool> ShouldRunFunc;
    public void* Context;
    public RunCriteria Criteria;

    public int* Dependencies;
    public int DependenciesCount;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool ShouldRun() => ShouldRunFunc(Context);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Update() => UpdateFunc(Context);
}
