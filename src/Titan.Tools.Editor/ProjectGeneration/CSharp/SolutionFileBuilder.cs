using System.Diagnostics;
using System.Text;

namespace Titan.Tools.Editor.ProjectGeneration.CSharp;
internal record SolutionProject
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public required Guid Type { get; init; }
    public required Guid Id { get; init; }
    public Guid? Parent { get; init; }
}
internal class SolutionFileBuilder
{
    private const string FileFormatVersion = "12.00";
    private const int VisualStudioVersionNumber = 17;
    private const string VisualStudioVersion = "17.5.33209.295";
    private const string MinimumVisualStudioVersion = "10.0.40219.1";
    private static readonly Guid ProjectGuid = new("9A19103F-16F7-4668-BE54-9A1E7A4F7556");
    private static readonly Guid SolutionFolderGuid = new("2150E333-8FDC-42A3-9474-1A3956D46DE8");

    private readonly List<SolutionProject> _projects = new();
    private readonly List<string> _configurations = new();
    private readonly Guid SolutionGuid = Guid.NewGuid();
    public void AddConfiguration(string configuration)
    {
        _configurations.Add(configuration);
    }

    public Guid AddSolutionDirectory(string name)
    {
        var project = new SolutionProject
        {
            Id = Guid.NewGuid(),
            Name = name,
            Path = name,
            Type = SolutionFolderGuid
        };
        _projects.Add(project);
        return project.Id;
    }

    public Guid AddProject(string path, Guid? parent = null)
    {
        Debug.Assert(parent == null || _projects.Any(p => p.Id == parent));

        var name = Path.GetFileNameWithoutExtension(path);
        var project = new SolutionProject
        {
            Id = Guid.NewGuid(),
            Name = name,
            Path = path,
            Type = ProjectGuid,
            Parent = parent
        };
        _projects.Add(project);
        return project.Id;
    }


    public string Build()
    {
        var builder = new StringBuilder()
                .AppendLine($"Microsoft Visual Studio Solution File, Format Version {FileFormatVersion}")
                .AppendLine($"# Visual Studio Version {VisualStudioVersionNumber}")
                .AppendLine($"VisualStudioVersion = {VisualStudioVersion}")
                .AppendLine($"MinimumVisualStudioVersion = {MinimumVisualStudioVersion}")
            ;

        foreach (var project in _projects)
        {
            var projectId = GuidUpper(project.Id);
            var type = GuidUpper(project.Type);
            builder.AppendLine($"Project(\"{type}\") = \"{project.Name}\", \"{project.Path}\", \"{projectId}\"");
            builder.AppendLine("EndProject");
        }

        builder.AppendLine("Global");
        AppendConfigurations(builder);
        AppendBuildConfigurations(builder);
        AppendNestedObjects(builder);
        AppendSolutionProperties(builder);
        AppendSolutionGuid(builder);
        builder.AppendLine("EndGlobal");
        return builder.ToString();
    }

    private void AppendSolutionGuid(StringBuilder builder)
    {
        GlobalSectionBegin(builder, "SolutionProperties", "preSolution");
        KeyValue(builder, "SolutionGuid", $"{SolutionGuid:B}");
        GlobalSectionEnd(builder);
    }

    private static void AppendSolutionProperties(StringBuilder builder)
    {
        GlobalSectionBegin(builder, "SolutionProperties", "preSolution");
        KeyValue(builder, "HideSolutionNode", "FALSE");
        GlobalSectionEnd(builder);
    }

    private void AppendNestedObjects(StringBuilder builder)
    {
        GlobalSectionBegin(builder, "NestedProjects", "preSolution");
        foreach (var project in _projects.Where(p => p.Parent != null))
        {
            var projectId = GuidUpper(project.Id);
            var parent = GuidUpper(project.Parent!.Value);
            KeyValue(builder, projectId, parent);
        }
        GlobalSectionEnd(builder);
    }

    private void AppendBuildConfigurations(StringBuilder builder)
    {
        GlobalSectionBegin(builder, "ProjectConfigurationPlatforms", "postSolution");
        foreach (var project in _projects.Where(p => p.Type == ProjectGuid))
        {
            foreach (var config in _configurations)
            {
                var projectId = GuidUpper(project.Id);
                KeyValue(builder, $"{projectId}.{config}.ActiveCfg", config);
                KeyValue(builder, $"{projectId}.{config}.Build.0", config);
            }
        }
        GlobalSectionEnd(builder);
    }

    private void AppendConfigurations(StringBuilder builder)
    {
        GlobalSectionBegin(builder, "SolutionConfigurationPlatforms", "preSolution");
        foreach (var configuration in _configurations)
        {
            KeyValue(builder, configuration, configuration);
        }
        GlobalSectionEnd(builder);
    }

    private static void GlobalSectionEnd(StringBuilder builder)
        => builder.AppendLine("\tEndGlobalSection");
    private static void GlobalSectionBegin(StringBuilder builder, string name, string type)
        => builder.AppendLine($"\tGlobalSection({name}) = {type}");
    private static void KeyValue(StringBuilder builder, string key, string value)
        => builder.AppendLine($"\t\t{key} = {value}");

    private static string GuidUpper(in Guid guid)
        => guid.ToString("B").ToUpper();
}
