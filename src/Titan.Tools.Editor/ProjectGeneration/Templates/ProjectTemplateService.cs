using System.Text.Json;
using System.Text.Json.Serialization;
using Titan.Tools.Editor.Configuration;
using Titan.Tools.Editor.Core;

namespace Titan.Tools.Editor.ProjectGeneration.Templates;

internal class ProjectTemplateService : IProjectTemplateService
{
    private const string TemplateConfigFileName = "config.json";
    private const string TemplateImageFileName = "icon.png";
    private readonly IFileSystem _fileSystem;
    public ProjectTemplateService(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public async Task<IEnumerable<ProjectTemplate>> GetTemplates()
    {
        var files = _fileSystem.EnumerateFiles(GlobalConfiguration.TemplatesPath, TemplateConfigFileName, SearchOption.AllDirectories);
        List<ProjectTemplate> templates = new();
        foreach (var file in files)
        {
            var bytes = await _fileSystem.ReadBytes(file);
            var template = JsonSerializer.Deserialize(bytes, ProjectTemplateJsonContext.Default.ProjectTemplate);
            if (template != null)
            {
                var templateBasePath = Path.GetDirectoryName(file) ?? throw new InvalidOperationException($"This was not expected, failed to get the directory name of {file}");
                var templateImagePath = Path.GetFullPath(Path.Combine(templateBasePath, TemplateImageFileName));
                templates.Add(template with
                {
                    TemplateBasePath = Path.GetFullPath(templateBasePath),
                    TemplateImage = _fileSystem.Exists(templateImagePath) ? templateImagePath : null
                });
            }
        }
        return templates;
    }
}

[JsonSourceGenerationOptions]
[JsonSerializable(typeof(ProjectTemplate))]
internal partial class ProjectTemplateJsonContext : JsonSerializerContext { }

