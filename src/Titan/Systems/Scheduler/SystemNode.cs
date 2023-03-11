using System.Runtime.CompilerServices;
using Titan.Resources;

namespace Titan.Systems.Scheduler;

internal unsafe struct SystemNode
{
    public ResourceId Id;
    public SystemStage Stage;
    public RunCriteria Criteria;
    public void* Context;
    public delegate*<void*, void> UpdateFunc;
    public delegate*<void*, bool> ShouldRunFunc;
    public delegate*<void*, void> ShutdownFunc;
    public int* Dependencies;
    public uint DependenciesCount;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Update() => UpdateFunc(Context);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool ShouldRun() => ShouldRunFunc(Context);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Shutdown() => ShutdownFunc(Context);
}
