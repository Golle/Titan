using System;
using Titan.Core.Logging;
using Titan.Core.App;
using Titan.ECS.SystemsV2;

namespace Titan.Graphics.Modules;

public record struct WindowCreated;
public record struct WindowClosed;

public struct WindowModule : IModule
{
    public static unsafe void Build(IApp app)
    {
        // NOTE(Jens): Windows functions, replace with any other platform when we support that.
        var windowFunctions = WindowFunctions.Create<Win32WindowFunctions>();
        var api = new WindowApi(windowFunctions);
        app
            .AddResource(api)
            .AddEvent<WindowCreated>()
            .AddEvent<WindowClosed>()
            .AddEvent<KeyReleased>()
            .AddEvent<KeyPressed>()
            .AddEvent<WindowLostFocus>()
            .AddEvent<WindowGainedFocus>()
            ;

        // Get the window descriptor and create the window
        if (!app.HasResource<WindowDescriptor>())
        {
            app.AddResource(WindowDescriptor.Default());
        }

        var descriptor = app.GetResource<WindowDescriptor>();
        Logger.Trace<WindowModule>($"Window descriptor: {descriptor.Title} - Size: {descriptor.Width}x{descriptor.Height}. Can resize: {descriptor.Resizable}");

        var eventQueue = app.GetMutableResourcePointer<WindowEventQueue>();
        if (!api.CreateWindow(descriptor, eventQueue, out var window))
        {
            Logger.Error<WindowModule>("Failed to create the Window.");
            throw new Exception($"{nameof(WindowModule)} failed to initialize the window.");
        }

        app.AddResource(window);
        
        // NOTE(Jens): Add the Window translation system at the end of the frame so all Window events are available in the next frame to reduce the "lag" between an event and when it actually gets processed by the game loop.
        app
            .AddSystemToStage<WindowMessageSystem>(Stage.PostUpdate);
    }
}
