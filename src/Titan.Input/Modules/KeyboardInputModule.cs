using Titan.ECS.App;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;

namespace Titan.Input.Modules;

public readonly struct KeyboardInputModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        builder
            .AddResource<KeyboardState>()
            .AddSystemToStage<KeyboardInputSystem>(Stage.PreUpdate, RunCriteria.Always);
        return true;
    }
}
