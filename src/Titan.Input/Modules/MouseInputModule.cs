using System;
using Titan.ECS.App;
using Titan.ECS.SystemsV2;
using Titan.Graphics.Modules;

namespace Titan.Input.Modules;

public readonly struct MouseInputModule : IModule2
{
    public static void Build(AppBuilder builder)
    {
        // should we do this?
        //if (!builder.HasResource<Window>())
        //{
        //    throw new InvalidOperationException($"{nameof(InputModule)} requires the {nameof(Window)} resource to be added.");
        //}

        builder.AddSystem<MouseInputSystem>();
    }
}
