using Titan.ECS.App;

namespace Titan.Input.Modules;

public readonly struct MouseInputModule : IModule
{
    public static bool Build(AppBuilder builder)
    {
        // should we do this?
        //if (!builder.HasResource<Window>())
        //{
        //    throw new InvalidOperationException($"{nameof(InputModule)} requires the {nameof(Window)} resource to be added.");
        //}

        builder.AddSystem<MouseInputSystem>();
        return true;
    }
}
