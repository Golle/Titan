using System;
using Titan.Core.Logging;
using Titan.Core;
using Titan.Core.App;
using Titan.ECS.SystemsV2;
using Titan.Graphics.Windows.Events;

namespace Titan.Graphics.Modules;

public record struct WindowCreated;
public record struct WindowClosed;

public struct WindowModule : IModule
{
    public static unsafe void Build(IApp app)
    {
        // Register the resources and events
        var eventQueue = app.GetMutableResourcePointer<WindowEventQueue>();
        // NOTE(Jens): Windows functions, replace with any other platform when we support that.
        var windowFunctions = WindowFunctions.Create<Win32WindowFunctions>();
        
        // Create the Window struct with functions and the event queue
        var window = new Window(windowFunctions, eventQueue);

        app
            .AddResource(window)
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

        var createWindowResult = app
            .GetMutableResource<Window>()
            .CreateWindow(descriptor);

        if (!createWindowResult)
        {
            Logger.Error<WindowModule>("Failed to create the Window.");
            throw new Exception($"{nameof(WindowModule)} failed to initialize the window.");
        }

        // NOTE(Jens): Add the Window translation system at the end of the frame so all Window events are available in the next frame to reduce the "lag" between an event and when it actually gets processed by the game loop.
        //app
        //    .AddSystemToStage<WindowMessageSystem>(Stage.PostUpdate);
    }
}
