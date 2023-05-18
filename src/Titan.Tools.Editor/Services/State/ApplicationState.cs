using Titan.Tools.Editor.Project;

namespace Titan.Tools.Editor.Services.State;

internal class ApplicationState : IApplicationState
{
    private TitanProject? _project;
    private string? _projectDirectory;
    public TitanProject Project => _project ?? throw new InvalidOperationException($"The {nameof(ApplicationState)} has not been initialized.");
    public string ProjectDirectory => _projectDirectory ?? throw new InvalidOperationException($"The {nameof(ApplicationState)} has not been initialized.");

    public void Initialize(TitanProject project, string projectDirectory)
    {
        if (_project != null)
        {
            throw new InvalidOperationException($"The {nameof(ApplicationState)} has already been initialized.");
        }
        _project = project;
        _projectDirectory = projectDirectory;
        
    }
}
