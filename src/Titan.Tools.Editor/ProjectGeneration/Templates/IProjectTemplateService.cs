namespace Titan.Tools.Editor.ProjectGeneration.Templates;

internal record ProjectTemplate(string Name, string EntryPoint, string? TemplateImage, string[] Folders, string[] Files)
{
    public string TemplateBasePath { get; init; } = string.Empty;
}
internal interface IProjectTemplateService
{
    Task<IEnumerable<ProjectTemplate>> GetTemplates();
}
