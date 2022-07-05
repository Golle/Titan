using System;
using Titan.ECS.SystemsV2;
using Titan.Graphics.Modules;

namespace Titan.Input.Modules;

public readonly struct InputModule : IModule
{
    public static void Build(IApp app)
    {
        if (!app.HasResource<Window>() || !app.HasResource<WindowApi>())
        {
            throw new InvalidOperationException($"{nameof(InputModule)} requires the {nameof(Window)} and {nameof(WindowApi)} resources to be added.");
        }

        app
            .AddModule<KeyboardInputModule>()
            .AddModule<MouseInputModule>()
            ;
    }
}
