using Avalonia;

namespace Titan.Tools.Editor;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        //var projectTemplate = new ProjectTemplateService(new FileSystem()).GetTemplates().ConfigureAwait(false).GetAwaiter().GetResult();
        //var result = new ProjectGenerationService(new FileSystem(), new CSharpProjectFileGenerator(), new SolutionFileGenerator(), new TitanProjectFile(new FileSystem()))
        //    .CreateProjectFromTemplate("ThisIsTheGame", @"c:\tmp\poo", projectTemplate.First()).ConfigureAwait(false).GetAwaiter().GetResult();


        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();


}
