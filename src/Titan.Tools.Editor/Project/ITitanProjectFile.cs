using System.Text.Json.Serialization;
using Titan.Tools.Editor.Common;

namespace Titan.Tools.Editor.Project;

public class GameBuildConfiguration
{
    public required string Name { get; set; }
    public required string Configuration { get; set; }
    public bool NativeAOT { get; set; } = true;
    public bool Trimming { get; set; } = true;
    public bool ConsoleWindow { get; set; } = true;
    public bool ConsoleLogging { get; set; } = true;
    public bool DebugSymbols { get; set; }
}

public class TitanProjectBuildSettings
{
    public const string DefaultOutputPath = "release";
    public required string CSharpProjectFile { get; init; }
    public required string OutputPath { get; set; }
    public required string CurrentConfiguration { get; init; }
    public List<GameBuildConfiguration> Configurations { get; set; } = new();

    public GameBuildConfiguration GetCurrentOrDefaultConfiguration()
    {
        if (Configurations.Count == 0)
        {
            throw new InvalidOperationException("No configurations found.");
        }

        //NOTE(Jens): not sure if we should crash or just return the first one in the list.
        var match = Configurations.FirstOrDefault(c => c.Name == CurrentConfiguration);
        return match ?? Configurations.First();
    }
}

public class TitanProject
{
    public required string Name { get; init; }
    public required string SolutionFile { get; init; }
    public required TitanProjectBuildSettings BuildSettings { get; init; }
}

public interface ITitanProjectFile
{
    Task<Result> Save(TitanProject project, string path);
    Task<Result<TitanProject>> Read(string path);
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(TitanProject))]
internal partial class TitanProjectJsonContext : JsonSerializerContext
{
}
