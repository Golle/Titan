using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Titan.Tools.ManifestBuilder.Common;

namespace Titan.Tools.ManifestBuilder.Services;


public record struct CookAssetSettings(string? Namespace, string? OutputPath, string? GeneratedPath);
public class RecentProjects
{
    public const int MaxRecentProjects = 10;
    public List<string> Paths { get; set; } = new(MaxRecentProjects);
    public bool AddProject(string path)
    {
        var alreadyExist = Paths.Any(p => p.Equals(path, StringComparison.OrdinalIgnoreCase));
        if (!alreadyExist)
        {
            Paths.Add(path);
        }

        if (Paths.Count >= MaxRecentProjects)
        {
            Paths.RemoveAt(0);
        }
        return !alreadyExist;
    }
}
public record struct PanelSize(double Size);
public record struct WindowSize(double Width, double Height, int X, int Y)
{
    [JsonIgnore]
    public bool HasValues => Width != 0 && Height != 0;
}

public record Settings(WindowSize WindowSize, PanelSize ManifestPanelSize, PanelSize PropertiesPanelSize, PanelSize ContentPanelSize, CookAssetSettings CookAssetSettings, RecentProjects RecentProjects)
{
    public static Settings Default() => new(default, default, default, default, default, new RecentProjects());
}

public interface IAppSettings
{
    Settings GetSettings();
    void Save(Settings settings);
}

internal class AppDataSettings : IAppSettings
{
    private readonly IJsonSerializer _jsonSerializer;
    private Settings? _settings;
    public AppDataSettings(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }
    public Settings GetSettings()
    {
        if (_settings != null)
        {
            return _settings;
        }

        var path = GlobalConfiguration.SettingsFile;
        if (File.Exists(path))
        {
            try
            {
                var bytes = File.ReadAllBytes(path);
                _settings = _jsonSerializer.Deserialize<Settings>(bytes);
            }
            catch
            {
                Debug.WriteLine("Failed to deserialize/read the settings. Will use default.");
            }
        }

        return _settings ??= Settings.Default();
    }

    public void Save(Settings settings)
    {
        try
        {
            var path = GlobalConfiguration.SettingsFile;
            var serializeToUtf8 = _jsonSerializer.SerializeToUtf8(settings, true);
            File.WriteAllBytes(path, serializeToUtf8);
        }
        catch
        {
            //NOTE(Jens): not sure how to handle this.
            return;
        }
        // only update settings on successful save
        _settings = settings;
    }
}
