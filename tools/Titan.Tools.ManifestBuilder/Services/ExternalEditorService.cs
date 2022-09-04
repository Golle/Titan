using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Titan.Tools.Core.Common;

namespace Titan.Tools.ManifestBuilder.Services;

public interface IExternalEditorService
{
    Task<Result> OpenEditor(string relativeFilePath);
}

internal class ExternalEditorService : IExternalEditorService
{
    private readonly IAppSettings _appSettings;
    private readonly IApplicationState _applicationState;

    public ExternalEditorService(IAppSettings appSettings, IApplicationState applicationState)
    {
        _appSettings = appSettings;
        _applicationState = applicationState;
    }
    public async Task<Result> OpenEditor(string relativeFilePath)
    {
        var settings = _appSettings.GetSettings();
        var projectPath = _applicationState.ProjectPath;

        if (string.IsNullOrWhiteSpace(settings.EditorPath))
        {
            return Result.Fail("No editor has been specified.");
        }
        var arguments = settings.EditorArgumentsFormat?.Replace("%d", projectPath).Replace("%f", relativeFilePath) ?? string.Empty;

        var process = new Process
        {
            StartInfo = new ProcessStartInfo(settings.EditorPath, arguments)
            {
                WorkingDirectory = projectPath
            }
        };
        try
        {
            process.Start();
            await Task.Delay(200);
            if (process.HasExited)
            {
                return Result.Fail($"Process exited immediately with code {process.ExitCode}");
            }
        }
        catch (Exception e)
        {
            return Result.Fail($"A {e.GetType().Name} was thrown with messag {e.Message}");
        }
        return Result.Success();
    }
}
