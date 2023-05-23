using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Core;
using Titan.Tools.Editor.Services.State;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor.Tools;

internal class RunGameTool : ITool
{
    private readonly IApplicationState _applicationState;
    private readonly IProcessRunner _processRunner;
    public string Name => "Run Game";

    public RunGameTool(IApplicationState applicationState, IProcessRunner processRunner)
    {
        _applicationState = applicationState;
        _processRunner = processRunner;
    }
    public async Task<Result> Execute()
    {
        var buildSettings = _applicationState.Project.BuildSettings;
        var configuration = buildSettings.GetCurrentOrDefaultConfiguration();
        List<string> argumentList = new()
        {
            "run",
            $"--project {buildSettings.CSharpProjectFile}",
            $"-c {configuration.Configuration}"
        };

        var arguments = string.Join(" ", argumentList);
        var result = await _processRunner.Run(new ProcessArgs("dotnet", arguments)
        {
            WorkgingDiretory = _applicationState.ProjectDirectory,
            CreateWindow = true,
            Timeout = TimeSpan.MaxValue // never timeout
        });

        if (!result.Success)
        {
            //NOTE(Jens): Add some output log that we can display in another window.
            return Result.Fail($"Failed to run the game project. Error = {result.Reason}. ExitCode = {result.ExitCode}");
        }
        return Result.Ok();
    }
}
