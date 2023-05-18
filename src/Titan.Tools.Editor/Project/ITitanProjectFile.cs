using System.Text.Json.Serialization;
using Titan.Tools.Editor.Common;

namespace Titan.Tools.Editor.Project;

public class TitanProjectBuildSettings
{
    public required string CSharpProjectFile { get; init; }
    public bool NativeAOT { get; set; } = true;
    public bool Trimming { get; set; } = true;
    public bool ConsoleWindow { get; set; } = true;
    public bool ConsoleLogging { get; set; } = true;
    public string? OutputPath { get; set; }
    public string? Configuration { get; set; }
    public bool DebugSymbols { get; set; }
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
