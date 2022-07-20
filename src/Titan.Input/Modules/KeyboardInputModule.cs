using Titan.ECS.App;

namespace Titan.Input.Modules;

public readonly struct KeyboardInputModule : IModule
{
    public static void Build(AppBuilder builder)
    {
        builder.AddSystem<KeyboardInputSystem>();
    }
}
