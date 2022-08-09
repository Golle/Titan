using System;
using System.Diagnostics;
using Titan.Core.Logging;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.Graphics.D3D12Take2.Stats;

internal struct GPUMemoryStatsSystem : IStructSystem<GPUMemoryStatsSystem>
{
    private MutableResource<RenderData> RenderData;
    private ReadOnlyResource<D3D12Core> Core;

    private int _counter;
    public static void Init(ref GPUMemoryStatsSystem system, in SystemsInitializer init)
    {
        // Dont track dependencies, will only update data that is not updated by other systems.
        system.RenderData = init.GetMutableResource<RenderData>(false);
        system.Core = init.GetReadOnlyResource<D3D12Core>(false);
    }

    public static void Update(ref GPUMemoryStatsSystem system)
    {
        //NOTE(Jens): this system will probably add some overhead, should probably be disabled by default
        if (system._counter++ < 10)
        {
            return;
        }
        system._counter = 0;
        ref readonly var adapter = ref system.Core.Get().Adapter;
        ref var renderData = ref system.RenderData.Get();
        
        if (adapter.GetNonLocalMemoryInfo(out var nonLocalMemory))
        {
            renderData.NonLocalGPUStats.Budget = nonLocalMemory.Budget;
            renderData.NonLocalGPUStats.CurrentUsage = nonLocalMemory.CurrentUsage;
            renderData.NonLocalGPUStats.AvailableForReservation = nonLocalMemory.AvailableForReservation;
            renderData.NonLocalGPUStats.CurrentReservation = nonLocalMemory.CurrentReservation;
        }

        if (adapter.GetLocalMemoryInfo(out var localMemory))
        {
            renderData.LocalGPUStats.Budget = localMemory.Budget;
            renderData.LocalGPUStats.CurrentUsage = localMemory.CurrentUsage;
            renderData.LocalGPUStats.AvailableForReservation = localMemory.AvailableForReservation;
            renderData.LocalGPUStats.CurrentReservation = localMemory.CurrentReservation;
        }
    }
    public static bool ShouldRun(in GPUMemoryStatsSystem system) => throw new NotImplementedException("Should be set as always run");
}
