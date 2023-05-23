using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Core;
using Titan.Tools.Editor.Services.State;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor.Tools;

internal class PublishTool : ITool
{
    private readonly IApplicationState _applicationState;
    private readonly IProcessRunner _processRunner;
    public string Name => "Publish";

    public PublishTool(IApplicationState applicationState, IProcessRunner processRunner)
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
            "publish",
            buildSettings.CSharpProjectFile,
            "--self-contained true",
            "-r win-x64",
            $"-c {configuration.Configuration}"
        };
        var outputPath = GetOutputPath(buildSettings.OutputPath, configuration.Configuration);
        argumentList.Add($"-o {outputPath}");

        if (configuration.ConsoleWindow)
        {
            argumentList.Add("-p:ConsoleWindow=true");
        }
        if (configuration.ConsoleLogging)
        {
            argumentList.Add("-p:ConsoleLogging=true");
        }
        if (configuration.Trimming)
        {
            argumentList.Add("-p:PublishTrimmed=True");
        }
        if (configuration.NativeAOT)
        {
            argumentList.Add("-p:PublishAot=True");
        }
        if (configuration.DebugSymbols)
        {
            argumentList.Add("-p:DebugSymbols=True");
        }
        else
        {
            argumentList.Add("-p:DebugType=None");
        }

        var arguments = string.Join(" ", argumentList);
        var result = await _processRunner.Run(new ProcessArgs("dotnet", arguments)
        {
            WorkgingDiretory = _applicationState.ProjectDirectory,
            CreateWindow = true,
            Timeout = TimeSpan.FromMinutes(5)
        });

        if (!result.Success)
        {
            //NOTE(Jens): Add some output log that we can display in another window.
            return Result.Fail($"Failed to compile the project. Error = {result.Reason}. ExitCode = {result.ExitCode}");
        }
        return Result.Ok();
    }

    private string GetOutputPath(string outputPath, string configuration)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return Path.Combine(_applicationState.ProjectDirectory, "release", configuration);
        }
        if (Path.IsPathRooted(outputPath))
        {
            return outputPath;
        }
        return Path.GetFullPath(Path.Combine(_applicationState.ProjectDirectory, outputPath, configuration));
    }
}
