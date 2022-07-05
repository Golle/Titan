using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.Core.Logging;

namespace Titan.ECS.SystemsV2.Scheduler;



public static unsafe class SchedulerTest
{

    // get all systems with their dependencies


    public static void RunSystems()
    {
        const int systemCount = 3;
        Span<ASystemNode> systems = stackalloc ASystemNode[systemCount];
        InitSystems(systems);

        while (true)
        {
            Logger.Info("Starting systems loop", typeof(SchedulerTest));
            Func(systems);
            Logger.Info("Completed systems loop", typeof(SchedulerTest));
            break;
        }
    }

    public enum SystemState
    {
        Waiting,
        Running,
        Completed
    }
    
    [SkipLocalsInit]
    public static void Func(Span<ASystemNode> systems)
    {
        var systemCount = systems.Length;
        var systemsLeft = systemCount;
        var state = stackalloc SystemState[systems.Length];

        //// Check for any system that should not be scheduled
        for (var i = 0; i < systems.Length; ++i)
        {
            ref var system = ref systems[i];
            if (!system.ShouldRun())
            {
                Logger.Error($"Set system {system.Id} to completed.", typeof(SchedulerTest));
                state[system.Id] = SystemState.Completed;
                systemsLeft--;
            }
            else
            {
                Logger.Error($"Set system {system.Id} to waiting.", typeof(SchedulerTest));
                state[system.Id] = SystemState.Waiting;
                system.IterationsLeft = system.IterationsToComplete;
            }
        }

        var debugIterations = 0;

        while (systemsLeft > 0)
        {
            Logger.Error($"Iteration: {debugIterations}", typeof(SchedulerTest));
            for (var i = 0; i < systems.Length; ++i)
            {
                ref var system = ref systems[i];
                if (IsReadyToRun(system))
                {
                    Logger.Error($"Starting system {system.Id} on iteration {debugIterations}.");
                    state[system.Id] = SystemState.Running;
                }
            }

            foreach (ref var system in systems)
            {
                if (state[system.Id] == SystemState.Running && system.IterationsLeft > 0)
                {
                    system.IterationsLeft--;
                    if (system.IterationsLeft == 0)
                    {
                        
                        state[system.Id] = SystemState.Completed;
                        systemsLeft--;
                        Logger.Error($"Completed system {system.Id} on iteration {debugIterations}. {systemsLeft} systems left.");
                    }
                }
            }

            Thread.Sleep(400);
            debugIterations++;
        }


        bool IsReadyToRun(in ASystemNode node)
        {
            if (state[node.Id] != SystemState.Waiting)
                return false;

            for (var i = 0; i < node.DependenciesCount; ++i)
            {
                if (state[node.Dependencies[i]] != SystemState.Completed)
                {
                    return false;
                }
            }
            return true;
        }
    }




    private static void InitSystems(Span<ASystemNode> systems)
    {
        var index = 0;
        systems[index] = Create<SystemA>();
        systems[index].IterationsToComplete = 8;


        index++;
        systems[index] = Create<SystemB>();
        systems[index].Dependencies[0] = systems[0].Id;
        systems[index].DependenciesCount = 1;
        systems[index].ShouldRun = &TestClass.ShouldNotRun;
        systems[index].IterationsToComplete = 2;

        index++;
        systems[index] = Create<SystemC>();
        systems[index].Dependencies[0] = systems[0].Id;
        systems[index].Dependencies[1] = systems[1].Id;
        systems[index].DependenciesCount = 2;
        systems[index].IterationsToComplete = 10;

    }

    private static ASystemNode Create<T>() =>
        new()
        {
            Id = Interlocked.Increment(ref ASystemNode._next) - 1,
            Update = &TestClass.Update,
            ShouldRun = &TestClass.ShouldRun,
            Dependencies = (uint*)NativeMemory.Alloc((nuint)sizeof(uint), 10),
            DependenciesCount = 0
        };
}






struct SystemA : IStructSystem<SystemA>
{
    public static void Init(ref SystemA system, in SystemsInitializer init) => throw new System.NotImplementedException();
    public static void Update(ref SystemA system) => throw new System.NotImplementedException();
}

struct SystemB : IStructSystem<SystemB>
{
    public static void Init(ref SystemB system, in SystemsInitializer init) => throw new System.NotImplementedException();
    public static void Update(ref SystemB system) => throw new System.NotImplementedException();
}

struct SystemC : IStructSystem<SystemC>
{
    public static void Init(ref SystemC system, in SystemsInitializer init) => throw new System.NotImplementedException();
    public static void Update(ref SystemC system) => throw new System.NotImplementedException();
}






public unsafe struct ASystemNode
{
    public uint Id;
    public uint* Dependencies;
    public uint DependenciesCount;
    public delegate*<uint, void> Update;
    public delegate*<bool> ShouldRun;
    
    // for testing
    public uint IterationsToComplete;
    public uint IterationsLeft;
    public static uint _next = 0;
}


public static class TestClass
{
    public static void Update(uint id) => Logger.Info($"Update system: {id}");
    public static bool ShouldRun() => true;
    public static bool ShouldNotRun() => false;
}
