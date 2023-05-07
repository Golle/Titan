namespace Titan.Tools.Editor.ProjectGeneration.CSharp;

internal class SolutionFileGenerator : ISolutionFileGenerator
{
    private static readonly string[] TitanProjects = { "Titan", "Titan.Core", "Titan.Platform", "Titan.Tools.Core" };
    public string GenerateSolutionFile(string projectName, string titanBasePath)
    {
        var srcPath = Path.Combine(titanBasePath, "src");

        var builder = new SolutionFileBuilder();
        builder.AddConfiguration("Debug|Any CPU");
        builder.AddConfiguration("Release|Any CPU");
        builder.AddProject(Path.Combine("src", projectName, $"{projectName}.csproj"));
        var engine = builder.AddSolutionDirectory("Engine");
        foreach (var project in TitanProjects)
        {
            builder.AddProject(Path.Combine(srcPath, project, $"{project}.csproj"), engine);
        }
        return builder.Build();
    }
}
