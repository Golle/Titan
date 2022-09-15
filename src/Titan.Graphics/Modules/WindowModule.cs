using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Scheduler;

namespace Titan.Graphics.Modules;

public record struct WindowCreated : IEvent;
public record struct WindowClosed : IEvent;

public struct WindowModule : IModule
{
    public static unsafe bool Build(AppBuilder builder)
    {
        var windowFunctions = WindowFunctions.Create<Win32WindowFunctions>();
        var api = new WindowApi(windowFunctions);
        builder
            .AddResource<WindowEventQueue>()
            .AddResource(api)
            ;

        // Get the window descriptor and create the window
        var descriptor = builder.GetResourceOrDefault<WindowDescriptor>();
        Logger.Trace<WindowModule>($"Window descriptor: {descriptor.Title} - Size: {descriptor.Width}x{descriptor.Height}. Can resize: {descriptor.Resizable}");

        var eventQueue = builder
            .GetResourcePointer<WindowEventQueue>();

        if (!api.CreateWindow(descriptor, eventQueue, out var window))
        {
            Logger.Error<WindowModule>("Failed to create the Window.");
            return false;
        }

        builder.AddResource(window);

        // NOTE(Jens): Add the Window translation system at the end of the frame so all Window events are available in the next frame to reduce the "lag" between an event and when it actually gets processed by the game loop.
        builder
            .AddSystemToStage<WindowMessageSystem>(Stage.PostUpdate);

        return true;
    }
}
