using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.ProjectGeneration.Templates;

namespace Titan.Tools.Editor.ProjectGeneration;

internal interface IProjectGenerationService
{
    Task<Result<string>> CreateProjectFromTemplate(string projectName, string path, ProjectTemplate template);
}
