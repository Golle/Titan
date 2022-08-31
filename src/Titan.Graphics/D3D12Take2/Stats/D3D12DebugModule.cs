using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.Graphics.D3D12Take2.Stats;

internal struct D3D12DebugModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        builder
            .AddSystemToStage<GPUMemoryStatsSystem>(Stage.PreUpdate, RunCriteria.Always);
        return true;
    }
}
