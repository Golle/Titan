using Microsoft.Extensions.DependencyInjection;
using Titan.Tools.Editor.Configuration;
using Titan.Tools.Editor.Core;
using Titan.Tools.Editor.Project;
using Titan.Tools.Editor.ProjectGeneration.CSharp;
using Titan.Tools.Editor.ProjectGeneration.Templates;
using Titan.Tools.Editor.Services;
using Titan.Tools.Editor.Services.State;
using Titan.Tools.Editor.Tools;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor;

internal static class Registry
{
    public static IServiceProvider Build(IServiceCollection serviceCollection) =>
        serviceCollection
            .AddCore()
            .AddConfiguration()
            .AddViewModels()
            .AddTools()
            .AddSingleton<IDialogService, DialogService>()

            .AddSingleton<ISolutionFileGenerator, SolutionFileGenerator>()
            .AddSingleton<ICSharpProjectFileGenerator, CSharpProjectFileGenerator>()
            .AddSingleton<IProjectTemplateService, ProjectTemplateService>()
            .AddSingleton<IProjectGenerationService, ProjectGenerationService>()
            .AddSingleton<ITitanProjectFile, TitanProjectFile>()


            

            .BuildServiceProvider();

    private static IServiceCollection AddCore(this IServiceCollection collection) =>
        collection
            .AddSingleton<IFileSystem, FileSystem>()
            .AddSingleton<IProcessRunner, ProcessRunner>()

        ;

    private static IServiceCollection AddConfiguration(this IServiceCollection collection) =>
        collection
            .AddSingleton<IAppConfiguration, LocalAppConfiguration>()
            .AddSingleton<IRecentProjects, RecentProjects>()
            .AddSingleton<IApplicationState, ApplicationState>()

    ;

    private static IServiceCollection AddViewModels(this IServiceCollection collection) =>
        collection
            .AddTransient<MainWindowViewModel>()
            .AddTransient<SelectProjectViewModel>()
            .AddTransient<NewProjectViewModel>()
            .AddTransient<ToolbarViewModel>();
    
    private static IServiceCollection AddTools(this IServiceCollection collection) =>
        collection
            .AddSingleton<ToolsProvider>()
            .AddSingleton<CompileTool>()
            .AddSingleton<RunGameTool>()
            .AddSingleton<PublishTool>()

    ;
}

