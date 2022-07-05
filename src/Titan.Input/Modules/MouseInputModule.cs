using System;
using Titan.ECS.SystemsV2;
using Titan.Graphics.Modules;

namespace Titan.Input.Modules;

public readonly struct MouseInputModule : IModule
{
    public static void Build(IApp app)
    {
        if (!app.HasResource<Window>())
        {
            throw new InvalidOperationException($"{nameof(InputModule)} requires the {nameof(Window)} resource to be added.");
        }

        app.AddSystem<MouseInputSystem>();
    }
}
