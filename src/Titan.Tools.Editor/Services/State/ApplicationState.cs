using Titan.Tools.Editor.Project;
using Titan.Tools.Editor.Services.Metadata;

namespace Titan.Tools.Editor.Services.State;

internal class ApplicationState : IApplicationState
{
    private readonly AssetsBackgroundService _assetsBackgroundService;
    private TitanProject? _project;
    private string? _projectDirectory;
    private string? _assetsDirectory;
    private string? _intermediateDirectory;
    public TitanProject Project => _project ?? throw new InvalidOperationException($"The {nameof(ApplicationState)} has not been initialized.");
    public string ProjectDirectory => _projectDirectory ?? throw new InvalidOperationException($"The {nameof(ApplicationState)} has not been initialized.");
    public string AssetsDirectory => _assetsDirectory ?? throw new InvalidOperationException($"The {nameof(ApplicationState)} has not been initialized.");
    public string IntermediateDirectory => _intermediateDirectory ?? throw new InvalidOperationException($"The {nameof(ApplicationState)} has not been initialized.");


    public ApplicationState(AssetsBackgroundService assetsBackgroundService)
    {
        _assetsBackgroundService = assetsBackgroundService;
    }

    public void Initialize(TitanProject project, string projectDirectory)
    {
        if (_project != null)
        {
            throw new InvalidOperationException($"The {nameof(ApplicationState)} has already been initialized.");
        }
        _project = project;
        _projectDirectory = projectDirectory;
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

    public async Task Stop()
    {
        await _assetsBackgroundService.Stop();
    }
}
