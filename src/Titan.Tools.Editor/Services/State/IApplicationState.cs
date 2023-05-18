using Titan.Tools.Editor.Project;

namespace Titan.Tools.Editor.Services.State;

public interface IApplicationState
{
    TitanProject Project { get; }
    string ProjectDirectory { get; }
    void Initialize(TitanProject project, string projectDirectory);
}
