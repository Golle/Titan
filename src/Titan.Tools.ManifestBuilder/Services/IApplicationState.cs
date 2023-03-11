using Titan.Tools.ManifestBuilder.Common;

namespace Titan.Tools.ManifestBuilder.Services;

public record ProjectSettings(string? Namespace, string? OutputPath, string? GeneratedPath, int? ManifestStartId)
{
    public static ProjectSettings Default => new(default, default, default, default);
}

public interface IApplicationState
{
    string? ProjectPath { get; }
    ProjectSettings ProjectSettings { get; }
    Task InitApplicationState(string projectPath);
    void Save(ProjectSettings settings);
    Task SaveAsync(ProjectSettings settings);
}

internal class ApplicationState : IApplicationState
{
    private readonly IJsonSerializer _jsonSerializer;
    public string? ProjectPath { get; private set; }
    public ProjectSettings ProjectSettings { get; private set; } = ProjectSettings.Default;

    public ApplicationState(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }

    public async Task InitApplicationState(string projectPath)
    {
        ProjectPath = projectPath;
        ProjectSettings = ToAbsolutePaths(await LoadSettings(projectPath));
    }

    public async Task SaveAsync(ProjectSettings settings)
    {
        if (string.IsNullOrWhiteSpace(ProjectPath))
        {
            throw new InvalidOperationException($"No {nameof(ProjectPath)} has been set.");
        }
        ProjectSettings = settings;

        await using var file = File.OpenWrite(Path.Combine(ProjectPath, GlobalConfiguration.ProjectSettingsFileName));
        file.SetLength(0);

        var convertedSettings = ToRelativePaths(settings);
        await _jsonSerializer.SerializeAsync(file, convertedSettings);
    }

    private ProjectSettings ToRelativePaths(ProjectSettings settings) =>
        settings with
        {
            GeneratedPath = Path.GetRelativePath(ProjectPath!, settings.GeneratedPath ?? string.Empty),
            OutputPath = Path.GetRelativePath(ProjectPath!, settings.OutputPath ?? string.Empty)
        };

    private ProjectSettings ToAbsolutePaths(ProjectSettings settings) =>
        settings with
        {
            GeneratedPath = string.IsNullOrWhiteSpace(settings.GeneratedPath) ? string.Empty : Path.GetFullPath(Path.Combine(ProjectPath!, settings.GeneratedPath)),
            OutputPath = string.IsNullOrWhiteSpace(settings.OutputPath) ? string.Empty : Path.GetFullPath(Path.Combine(ProjectPath!, settings.OutputPath))
        };


    public void Save(ProjectSettings settings)
    {
        if (string.IsNullOrWhiteSpace(ProjectPath))
        {
            throw new InvalidOperationException($"No {nameof(ProjectPath)} has been set.");
        }
        ProjectSettings = settings;
        var json = _jsonSerializer.Serialize(ToRelativePaths(settings));
        File.WriteAllText(Path.Combine(ProjectPath, GlobalConfiguration.SettingsFile), json);
    }

    private async Task<ProjectSettings> LoadSettings(string projectPath)
    {
        var projectConfigFile = Path.Combine(projectPath, GlobalConfiguration.ProjectSettingsFileName);
        if (File.Exists(projectConfigFile))
        {
            await using var file = File.OpenRead(projectConfigFile);
            try
            {
                return await _jsonSerializer.DeserializeAsync<ProjectSettings>(file) ?? ProjectSettings.Default;
            }
            catch
            {
                return ProjectSettings.Default;
            }
        }
        return ProjectSettings.Default;
    }
}
