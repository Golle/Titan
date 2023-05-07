using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Configuration;
using Titan.Tools.Editor.Core;
using Titan.Tools.Editor.Project;
using Titan.Tools.Editor.ProjectGeneration.CSharp;
using Titan.Tools.Editor.ProjectGeneration.Templates;

namespace Titan.Tools.Editor.Services;

internal class ProjectGenerationService : IProjectGenerationService
{
    private readonly IFileSystem _fileSystem;
    private readonly ICSharpProjectFileGenerator _csharpProjectFileGenerator;
    private readonly ISolutionFileGenerator _solutionFileGenerator;
    private readonly ITitanProjectFile _titanProjectFile;

    private static readonly string TitanProjectFilePath = Path.Combine(GlobalConfiguration.BasePath, "src", "Titan", "Titan.csproj");
    public ProjectGenerationService(IFileSystem fileSystem, ICSharpProjectFileGenerator csharpProjectFileGenerator, ISolutionFileGenerator solutionFileGenerator, ITitanProjectFile titanProjectFile)
    {
        _fileSystem = fileSystem;
        _csharpProjectFileGenerator = csharpProjectFileGenerator;
        _solutionFileGenerator = solutionFileGenerator;
        _titanProjectFile = titanProjectFile;
    }

    public async Task<Result<string>> CreateProjectFromTemplate(string projectName, string path, ProjectTemplate template)
    {
        //NOTE(Jens): Add validation for project name.
        var projectPath = Path.Combine(path, projectName);
        if (Directory.Exists(projectPath) && Directory.GetFileSystemEntries(projectPath).Length != 0)
        {
            return Result.Fail<string>($"Folder {projectPath} is not empty.");
        }
        var projectSourceCodePath = Path.Combine(projectPath, "src", projectName);
        var titanProjectFile = Path.Combine(projectPath, $"{projectName}{GlobalConfiguration.TitanProjectFileExtension}");
        try
        {
            _fileSystem.CreateFolder(projectPath);
            _fileSystem.CreateFolder(projectSourceCodePath);
            foreach (var folder in template.Folders)
            {
                var folderPath = Path.Combine(projectPath, folder);
                _fileSystem.CreateFolder(folderPath);
                if (folder.StartsWith("."))
                {
                    _fileSystem.SetHidden(folderPath);
                }
            }
            foreach (var templateFile in template.Files)
            {
                var source = Path.GetFullPath(Path.Combine(template.TemplateBasePath, templateFile));
                var destination = Path.GetFullPath(Path.Combine(projectPath, Path.GetFileName(templateFile)));
                _fileSystem.CopyFile(source, destination);
            }

            var entryPointCode = (await _fileSystem.ReadString(Path.Combine(template.TemplateBasePath, template.EntryPoint)))
                .Replace("$TITAN_FOLDER_PATH$", GlobalConfiguration.BasePath)
                .Replace("$PROJECT_NAME$", projectName)
                ;

            await _fileSystem.WriteText(Path.Combine(projectSourceCodePath, template.EntryPoint), entryPointCode);

            var solutionFileContents = _solutionFileGenerator.GenerateSolutionFile(projectName, GlobalConfiguration.BasePath);
            var solutionFileName = $"{projectName}.sln";
            await _fileSystem.WriteText(Path.Combine(projectPath, solutionFileName), solutionFileContents);

            var projectFileContents = _csharpProjectFileGenerator.GenerateProjectFileContents(TitanProjectFilePath);
            await _fileSystem.WriteText(Path.Combine(projectSourceCodePath, $"{projectName}.csproj"), projectFileContents);

            var projectFileResult = await _titanProjectFile.Save(new TitanProject(projectName, solutionFileName), titanProjectFile);
            if (!projectFileResult.Success)
            {
                return Result.Fail<string>(projectFileResult.Error ?? "Unknown error occured when saving the project file.");
            }

        }
        catch (Exception e)
        {
            return Result.Fail<string>($"{e.GetType().Name} - {e.Message}");
        }
        return Result.Ok(projectPath);
    }
}

