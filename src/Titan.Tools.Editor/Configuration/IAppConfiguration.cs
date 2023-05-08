using System.Text.Json.Serialization;

namespace Titan.Tools.Editor.Configuration;

public class AppConfig
{
    public List<RecentProject> RecentProjects { get; init; } = new();
}

public class RecentProject
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public required DateTime LastAccessed { get; set; }
};

internal interface IAppConfiguration
{
    Task<AppConfig> GetConfig();
    Task SaveConfig(AppConfig config);
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppConfig))]
internal partial class AppConfigJsonContext : JsonSerializerContext { }
