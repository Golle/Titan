using System.Text.Json;
using System.Text.Json.Serialization;
using Titan.Tools.Editor.Core;

namespace Titan.Tools.Editor.Configuration;


public class AppConfig
{
    public HashSet<string> RecentProjects { get; init; } = new();
}

internal interface IAppConfiguration
{
    Task<AppConfig> GetConfig();
    Task SaveConfig(AppConfig config);
}

internal class LocalAppConfiguration : IAppConfiguration
{

    private const string ConfigFileName = "config.json";
    private static readonly string ConfigFilePath = Path.Combine(GlobalConfiguration.AppDataPath, ConfigFileName);
    private readonly IFileSystem _fileSystem;

    private AppConfig? _config;
    public LocalAppConfiguration(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    public async Task<AppConfig> GetConfig()
    {
        if (_config != null)
        {
            return _config;
        }

        if (_fileSystem.Exists(ConfigFilePath))
        {
            await using var stream = _fileSystem.OpenRead(ConfigFilePath);
            _config = await JsonSerializer.DeserializeAsync(stream, AppConfigJsonContext.Default.AppConfig);
        }

        return _config ??= new AppConfig();
    }

    public async Task SaveConfig(AppConfig config)
    {
        _config = config;
        await using var stream = _fileSystem.OpenWrite(ConfigFilePath);
        stream.SetLength(0); // reset the file before writing.
        await JsonSerializer.SerializeAsync(stream, config);
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(AppConfig))]
internal partial class AppConfigJsonContext : JsonSerializerContext { }
