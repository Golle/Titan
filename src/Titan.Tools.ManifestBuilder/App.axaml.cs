using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Titan.Tools.ManifestBuilder.ViewModels;
using Titan.Tools.ManifestBuilder.Views;

namespace Titan.Tools.ManifestBuilder
{
    public class App : Application
    {
        public static void Exit(int exitCode = 0)
        {
            if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown(exitCode);
            }
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
        public static Window MainWindow => Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop ? desktop.MainWindow : throw new NotSupportedException("This sshould not happen.");

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                };
            }
            base.OnFrameworkInitializationCompleted();
        }
    }
}
