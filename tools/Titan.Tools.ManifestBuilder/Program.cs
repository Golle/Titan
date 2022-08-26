using System;
using Avalonia;
using Avalonia.ReactiveUI;

namespace Titan.Tools.ManifestBuilder;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var app = BuildAvaloniaApp();

        // add any init logic needed here
        app.StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure(() => new App())
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();
    }
}
