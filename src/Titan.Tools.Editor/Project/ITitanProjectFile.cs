using System.Text.Json.Serialization;
using Titan.Tools.Editor.Common;

namespace Titan.Tools.Editor.Project;
internal record TitanProject(string Name, string SolutionFile);
internal interface ITitanProjectFile
{
    Task<Result> Save(TitanProject project, string path);
    Task<Result<TitanProject>> Read(string path);
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(TitanProject))]
internal partial class TitanProjectJsonContext : JsonSerializerContext
{
}
