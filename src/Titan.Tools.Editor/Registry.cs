using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Configuration;
using Titan.Tools.Editor.Core;
using Titan.Tools.Editor.Project;
using Titan.Tools.Editor.ProjectGeneration;
using Titan.Tools.Editor.ProjectGeneration.CSharp;
using Titan.Tools.Editor.ProjectGeneration.Templates;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.Services.Assets;
using Titan.Tools.Editor.Services.Metadata;
using Titan.Tools.Editor.Services.State;
using Titan.Tools.Editor.Tools;
using Titan.Tools.Editor.ViewModels;
using Titan.Tools.Editor.ViewModels.ProjectSettings;

namespace Titan.Tools.Editor;

internal static class Registry
{
    public static IServiceProvider Build(IServiceCollection serviceCollection) =>
        serviceCollection
            .AddCore()
            .AddConfiguration()
            .AddViewModels()
            .AddTools()
            .AddAssetMetadataTools()
            .AddProjectSettings()
            .AddSingleton<IDialogService, DialogService>()

            .AddSingleton<ISolutionFileGenerator, SolutionFileGenerator>()
            .AddSingleton<ICSharpProjectFileGenerator, CSharpProjectFileGenerator>()
            .AddSingleton<IProjectTemplateService, ProjectTemplateService>()
            .AddSingleton<IProjectGenerationService, ProjectGenerationService>()
            .AddSingleton<ITitanProjectFile, TitanProjectFile>()


            .AddSingleton<IContentBrowser, ContentBrowser>()



            .BuildServiceProvider();

    private static IServiceCollection AddCore(this IServiceCollection collection) =>
        collection
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<IProcessRunner, ProcessRunner>()

        ;

    private static IServiceCollection AddConfiguration(this IServiceCollection collection)
    {
        collection
            .AddSingleton<IAppConfiguration, LocalAppConfiguration>()
            .AddSingleton<IRecentProjects, RecentProjects>()
            ;
        if (Design.IsDesignMode)
        {
            collection.AddSingleton<IApplicationState, DesignApplicationState>();
        }
        else
        {
            collection.AddSingleton<IApplicationState, ApplicationState>();
        }
        return collection;
    }

    private static IServiceCollection AddAssetMetadataTools(this IServiceCollection collection) =>
        collection
            .AddSingleton<AssetsBackgroundService>()
            .AddSingleton<AssetsFileWatcher>()
            .AddTransient<IAssetFileProcessor, AssetFileMetadataProcessor>()

    ;

    private static IServiceCollection AddProjectSettings(this IServiceCollection collection) =>
        collection
            .AddTransient<ProjectSettingsViewModel>()
            .AddTransient<IProjectSettings, GeneralSettingsViewModel>()
            .AddTransient<IProjectSettings, BuildSettingsViewModel>()

    ;

    private static IServiceCollection AddViewModels(this IServiceCollection collection) =>
        collection
            .AddTransient<MainWindowViewModel>()
            .AddTransient<SelectProjectViewModel>()
            .AddTransient<NewProjectViewModel>()
            .AddTransient<ToolbarViewModel>()
            .AddTransient<TerminalViewModel>()
            .AddTransient<AssetBrowserViewModel>();

    private static IServiceCollection AddTools(this IServiceCollection collection) =>
        collection
            .AddSingleton<ITool, CompileTool>()
            .AddSingleton<ITool, RunGameTool>()
            .AddSingleton<ITool, PublishTool>()

    ;
}


internal class DesignApplicationState : IApplicationState
{
    public TitanProject Project => new()
    {
        BuildSettings = new TitanProjectBuildSettings
        {
            CSharpProjectFile = "designer.csproj",
            CurrentConfiguration = "DesignConf",
            OutputPath = "release",
            Configurations = new List<GameBuildConfiguration>
            {
                new()
                {
                    Name = "DesignConf",
                    Configuration = "design_debug",
                    NativeAOT = true,
                    Trimming = false,
                }
            }
        },
        Name = "designer project",
        SolutionFile = "designer.sln"
    };
    public string ProjectDirectory => "c:/DESIGNER/";
    public string AssetsDirectory => "c:/DESIGNER/assets";
    public void Initialize(TitanProject project, string projectFilePath)
    {
    }

    public Task Stop() => Task.CompletedTask;
    public Task<Result> SaveChanges() => Task.FromResult(Result.Ok());
}
