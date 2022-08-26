using System;
using System.IO;

namespace Titan.Tools.ManifestBuilder.Common;

internal static class GlobalConfiguration
{
    public const string ManifestFileExtension = "tmanifest";
    public static string AppDataFolder { get; }
    public static string SettingsFile { get; }
    public static string BaseFolder { get; }

    public static string PackagerProjectPath { get; }
    
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

        BaseFolder = GetBaseDirectory();
        PackagerProjectPath = Path.Combine(BaseFolder, "tools", "Titan.Tools.Packager");
    }

    //NOTE(Jens): figure out how we should do this
    private static string GetBaseDirectory()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        const string SolutionFilename = "Titan.sln";
        return FindParentWithFile(SolutionFilename, baseDirectory, 7)
               ?? baseDirectory;

        static string? FindParentWithFile(string filename, string? path, int steps)
        {
            if (steps == 0 || path == null)
            {
                return null;
            }
            return File.Exists(Path.Combine(path, filename))
                ? path
                : FindParentWithFile(filename, Directory.GetParent(path)?.FullName, steps - 1);
        }
    }
}
