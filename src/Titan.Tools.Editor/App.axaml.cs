using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Titan.Tools.Editor.Views;

namespace Titan.Tools.Editor;
public partial class App : Application
{
    private IServiceProvider? _services;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _services = Registry.Build(new ServiceCollection());
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            //ExpressionObserver.DataValidators.RemoveAll(x => x is DataAnnotationsValidationPlugin);
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static T GetRequiredService<T>() where T : notnull
    {
        if (Current is App { _services: not null } app)
        {
            return app._services.GetRequiredService<T>();
        }
        throw new InvalidOperationException($"Services has not been initialized or the type {typeof(T).Name} has not been registered.");
    }

    public static Window GetMainWindow()
        => (Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow ?? throw new InvalidOperationException($"Failed to get the {nameof(IClassicDesktopStyleApplicationLifetime.MainWindow)}");
    public static void Exit(int exitCode = 0)
    {
        if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown(exitCode);
        }
    }
}
