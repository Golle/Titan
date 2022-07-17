using System.Runtime.CompilerServices;
using Titan.ECS.SystemsV2;

namespace Titan.ECS.Scheduler;

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
