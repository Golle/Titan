using System;
using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS;
using Titan.ECS.App;

namespace Titan.Graphics.Modules;

public record struct WindowCreated : IEvent;
public record struct WindowClosed : IEvent;

public struct WindowModule : IModule
{
    public static unsafe void Build(AppBuilder builder)
    {
        var windowFunctions = WindowFunctions.Create<Win32WindowFunctions>();
        var api = new WindowApi(windowFunctions);
        builder
            .AddResource(api)
            .AddEvent<WindowCreated>()
            .AddEvent<WindowClosed>()
            .AddEvent<KeyReleased>(10)
            .AddEvent<KeyPressed>(10)
            .AddEvent<WindowLostFocus>()
            .AddEvent<WindowGainedFocus>()
            .AddEvent<WindowResizeComplete>()
            .AddEvent<WindowSize>(20)
            ;

        // Get the window descriptor and create the window
        var descriptor = builder.GetResourceOrDefault<WindowDescriptor>();
        Logger.Trace<WindowModule>($"Window descriptor: {descriptor.Title} - Size: {descriptor.Width}x{descriptor.Height}. Can resize: {descriptor.Resizable}");

        var eventQueue = builder.GetResourcePointer<WindowEventQueue>();
        if (!api.CreateWindow(descriptor, eventQueue, out var window))
        {
            Logger.Error<WindowModule>("Failed to create the Window.");
            throw new Exception($"{nameof(WindowModule)} failed to initialize the window.");
        }

        builder.AddResource(window);

        // NOTE(Jens): Add the Window translation system at the end of the frame so all Window events are available in the next frame to reduce the "lag" between an event and when it actually gets processed by the game loop.
        builder
            .AddSystemToStage<WindowMessageSystem>(Stage.PostUpdate);
    }
}
