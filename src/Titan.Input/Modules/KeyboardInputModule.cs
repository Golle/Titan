using Titan.ECS.App;
using Titan.ECS.SystemsV2;

namespace Titan.Input.Modules;

public readonly struct KeyboardInputModule : IModule2
{
    public static void Build(AppBuilder builder)
    {
        builder.AddSystem<KeyboardInputSystem>();
    }
}
