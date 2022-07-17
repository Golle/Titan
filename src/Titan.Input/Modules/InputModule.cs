using System;
using Titan.ECS.AnotherTry;
using Titan.ECS.SystemsV2;
using Titan.Graphics.Modules;

namespace Titan.Input.Modules;

public readonly struct InputModule : IModule2
{
    public static void Build(AppBuilder builder)
    {
        // Should we do this?
        //if (!builder.HasResource<Window>() || !builder.HasResource<WindowApi>())
        //{
        //    throw new InvalidOperationException($"{nameof(InputModule)} requires the {nameof(Window)} and {nameof(WindowApi)} resources to be added.");
        //}

        builder
            .AddModule<KeyboardInputModule>()
            .AddModule<MouseInputModule>()
            ;
    }
}
