using System.Diagnostics;

namespace Titan.Tools.Editor.Configuration;
internal static class GlobalConfiguration
{
    public static string TitanProjectFileExtension => ".titan";
    public static string BasePath => GetBasePath();
    public static string TemplatesPath => Path.Combine(BasePath, "templates");
    public static string ProjectLocationBaseDir => Path.GetFullPath($"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/titanprojects");

    private static string GetBasePath()
    {
        var baseDirectory = AppContext.BaseDirectory;
#if DEBUG
        var path = Path.Combine(baseDirectory, "..", "..", "..", "..", "..");
        Debug.Assert(File.Exists(Path.Combine(path, "Titan.sln")), $"Could not find Titan.sln in folder {path}. Something is wrong.");
        return path;
#else
        return baseDirectory;
#endif
    }
}
