using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Project;
using Titan.Tools.Editor.Services.Metadata;

namespace Titan.Tools.Editor.Services.State;

internal class ApplicationState : IApplicationState
{
    private readonly AssetsBackgroundService _assetsBackgroundService;
    private readonly ITitanProjectFile _titanProjectFile;
    private TitanProject? _project;
    private string? _projectFilePath;
    private string? _projectDirectory;
    private string? _assetsDirectory;
    private string? _intermediateDirectory;
    public TitanProject Project => _project ?? throw new InvalidOperationException($"The {nameof(ApplicationState)} has not been initialized.");
    public string ProjectDirectory => _projectDirectory ?? throw new InvalidOperationException($"The {nameof(ApplicationState)} has not been initialized.");
    public string AssetsDirectory => _assetsDirectory ?? throw new InvalidOperationException($"The {nameof(ApplicationState)} has not been initialized.");
    public string IntermediateDirectory => _intermediateDirectory ?? throw new InvalidOperationException($"The {nameof(ApplicationState)} has not been initialized.");


    public ApplicationState(AssetsBackgroundService assetsBackgroundService, ITitanProjectFile titanProjectFile)
    {
        _assetsBackgroundService = assetsBackgroundService;
        _titanProjectFile = titanProjectFile;
    }

    public void Initialize(TitanProject project, string projectFilePath)
    {
        if (_project != null)
        {
            throw new InvalidOperationException($"The {nameof(ApplicationState)} has already been initialized.");
        }
        _project = project;
        _projectFilePath = projectFilePath;
        _projectDirectory = Path.GetDirectoryName(projectFilePath) ?? throw new InvalidOperationException($"Failed to get the directory from path {projectFilePath}");
        _assetsDirectory = Path.Combine(ProjectDirectory, "assets");
        _intermediateDirectory = Path.Combine(ProjectDirectory, ".titan");

        if (!Directory.Exists(_projectDirectory))
        {
            throw new InvalidOperationException($"Failed to initialize the {nameof(ApplicationState)} because the project directory {_projectDirectory} does not exist.");
        }
        //NOTE(Jens): Just make sure these directories exist
        Directory.CreateDirectory(_assetsDirectory);
        Directory.CreateDirectory(_intermediateDirectory);

        if (!_assetsBackgroundService.Start(_assetsDirectory))
        {
            throw new InvalidOperationException($"Failed to start the {nameof(AssetsBackgroundService)}");
        }
    }

    public async Task<Result> SaveChanges()
    {
        var project = _project ?? throw new NullReferenceException("The project is null");
        var path = _projectFilePath ?? throw new NullReferenceException("The project file path is null");

        var result = await _titanProjectFile.Save(project, path);

        return result.Success ?
            Result.Ok() :
            Result.Fail($"Failed to save the application state. Error = {result.Error}");
    }

    public async Task Stop()
    {
        await _assetsBackgroundService.Stop();
    }
}
