using Titan.Tools.Editor.Common;
using Titan.Tools.Editor.Project;

namespace Titan.Tools.Editor.Services.State;

public interface IApplicationState
{
    TitanProject Project { get; }
    string ProjectDirectory { get; }
    string AssetsDirectory{ get; }
    void Initialize(TitanProject project, string projectFilePath);
    Task Stop();
    Task<Result> SaveChanges();
}
