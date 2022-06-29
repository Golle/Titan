using System;
using Titan.Core.Logging;
using Titan.Core;

namespace Titan.Graphics.Modules;

public record struct WindowCreated;
public record struct WindowClosed;

public struct WindowModule : IModule
{
    public static void Build(IApp app)
    {
        app
            .AddResource(new Window(WindowFunctions.Create<Win32WindowFunctions>()))
            .AddEvent<WindowCreated>()
            .AddEvent<WindowClosed>()
            ;

        if (!app.HasResource<WindowDescriptor>())
        {
            app.AddResource(WindowDescriptor.Default());
        }

        var descriptor = app.GetResource<WindowDescriptor>();
        Logger.Info<WindowModule>($"Window descriptor: {descriptor.Title} - Size: {descriptor.Width}x{descriptor.Height}. Can resize: {descriptor.Resizable}");


        var createWindowResult = app
            .GetMutableResource<Window>()
            .CreateWindow(descriptor);

        if (!createWindowResult)
        {
            Logger.Error<WindowModule>("Failed to create the Window.");
            throw new Exception($"{nameof(WindowModule)} failed to initialize the window.");
        }


    }
}
