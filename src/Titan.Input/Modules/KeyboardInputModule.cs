using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.Input.Modules;

public readonly struct KeyboardInputModule : IModule
{
    public static void Build(AppBuilder builder)
    {
        builder
            .AddResource<KeyboardState>()
            .AddSystemToStage<KeyboardInputSystem>(Stage.PreUpdate, RunCriteria.Always);
    }
}
