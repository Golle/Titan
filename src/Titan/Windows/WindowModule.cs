using Titan.Core.Logging;
using Titan.Modules;
using Titan.Setup;
using Titan.Setup.Configs;
using Titan.Systems;
using Titan.Windows.Linux;
using Titan.Windows.Win32;

namespace Titan.Windows;

internal struct WindowModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        IWindow window = GlobalConfiguration.OperatingSystem switch
        {
            OperatingSystem.Windows => new Win32Window(),
            OperatingSystem.Linux => new WaylandWindow(),
            _ => null,
        };
        if (window == null)
        {
            Logger.Error<WindowModule>($"Platform {GlobalConfiguration.OperatingSystem} is not supported.");
            return false;
        }

        builder
            .AddManagedResource(window)
            .AddResource<WindowEventQueue>()
            .AddSystemToStage<WindowEventSystem>(SystemStage.PostUpdate, RunCriteria.Check);
        return true;
    }

    public static unsafe bool Init(IApp app)
    {
        var config = app.GetConfigOrDefault<WindowConfig>();
        var window = app.GetManagedResource<IWindow>();
        var eventQueue = app.GetResourcePointer<WindowEventQueue>();
        var args = new WindowCreationArgs(config.Width, config.Height, config.Title, config.Windowed, config.Resizable, config.AlwaysOnTop);

        if (window is Win32Window win32 && !win32.Init(args, eventQueue))
        {
            Logger.Error<WindowModule>($"Failed to Init the {nameof(Win32Window)}.");
            return false;
        }

        if (window is WaylandWindow wayland && !wayland.Init(args))
        {
            Logger.Error<WindowModule>($"Failed to Init the {nameof(WaylandWindow)}.");
            return false;
        }

        return true;
    }

    public static bool Shutdown(IApp app)
    {
        var window = app.GetManagedResource<IWindow>();
        (window as Win32Window)?.Shutdown();
        (window as WaylandWindow)?.Shutdown();

        return true;
    }
}
