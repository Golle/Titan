using System;
using System.IO;

namespace Titan.Tools.ManifestBuilder.Common;

internal static class GlobalConfiguration
{
    public const string ManifestFileExtension = "tmanifest";
    public static string AppDataFolder { get; }
    public static string SettingsFile { get; }

    static GlobalConfiguration()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        if (string.IsNullOrWhiteSpace(path))
        {
            //NOTE(Jens): This needs to be verified on MacOS and Linux. I think this might return null/empty.
            throw new NotSupportedException("Could not locate the appdata folder. This is probably because you're on a NIX system. Fix!");
        }
        AppDataFolder = Path.Combine(path, "Titan");
        SettingsFile = Path.Combine(AppDataFolder, "settings.json");

        if (!Directory.Exists(AppDataFolder))
        {
            Directory.CreateDirectory(AppDataFolder);
        }
    }
}
