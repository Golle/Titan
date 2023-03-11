namespace Titan.Tools.ManifestBuilder.Common;

internal static class GlobalConfiguration
{
    public const string ManifestFileExtension = "tmanifest";
    public const string ProjectSettingsFileName = ".titanconfig";
    public static string AppDataFolder { get; }
    public static string SettingsFile { get; }
    public static string BaseFolder { get; }
    public static string? PackagerProjectPath { get; }
    public static string? PackagerExecutablePath { get; } = null;
    public static string DxcLibPath { get; }

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

        //NOTE(Jens): we only set these paths if this is compiled in release mode.
#if DEBUG
        PackagerProjectPath = Path.Combine(BaseFolder, "src", "Titan.Tools.Packager");
        DxcLibPath = Path.Combine(BaseFolder, "libs", "dxc", "bin", "x64");
#else
        PackagerExecutablePath = Path.GetFullPath(Path.Combine(BaseFolder, "..", "Titan.Tools.Packager", "Titan.Tools.Packager.exe"));
        DxcLibPath = Path.GetFullPath(Path.Combine(BaseFolder, "..", "..", "libs", "dxc", "bin", "x64"));
#endif
    }

    //NOTE(Jens): figure out how we should do this
    private static string GetBaseDirectory()
    {
        var baseDirectory = AppContext.BaseDirectory;
#if DEBUG
        const string SolutionFilename = "Titan.sln";
        return FindParentWithFile(SolutionFilename, baseDirectory, 7)
               ?? baseDirectory;
#else
        return baseDirectory;
#endif

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
