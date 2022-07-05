using Titan.ECS.SystemsV2;

namespace Titan.Input.Modules;

public readonly struct KeyboardInputModule : IModule
{
    public static void Build(IApp app)
    {
        app.AddSystem<KeyboardInputSystem>();
    }
}
