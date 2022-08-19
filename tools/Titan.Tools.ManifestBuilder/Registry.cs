using System;
using Microsoft.Extensions.DependencyInjection;
using Titan.Tools.ManifestBuilder.Services;
using Titan.Tools.ManifestBuilder.ViewModels;
using Titan.Tools.ManifestBuilder.Views;

namespace Titan.Tools.ManifestBuilder;

internal static class Registry
{
    private static readonly IServiceProvider _serviceProvider;
    static Registry()
    {
        _serviceProvider = new ServiceCollection()
                .AddSingleton<IMessenger, MulticastDelegateMessenger>()
                .AddSingleton<IDialogService, DialogService>()
                .AddSingleton<IJsonSerializer, JsonSerializer>()
                .AddSingleton<IAppSettings, AppDataSettings>()
                .AddSingleton<IManifestService, ManifestService>()

                .AddSingleton<MainWindow>()

                .AddSingleton<EditorViewModel>()
                .AddSingleton<MainWindowViewModel>()
                .AddSingleton<ProjectSelectorViewModel>()
                .AddSingleton<ContentViewModel>()
                .AddSingleton<ManifestViewModel>()
                .AddSingleton<FileInfoViewModel>()


                .BuildServiceProvider()
            ;
    }
    public static T GetRequiredService<T>() where T : notnull => _serviceProvider.GetRequiredService<T>();
}
