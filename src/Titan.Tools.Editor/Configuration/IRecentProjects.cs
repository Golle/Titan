namespace Titan.Tools.Editor.Configuration;

public interface IRecentProjects
{
    /// <summary>
    /// Get all the recent projects stored in the configuration file
    /// </summary>
    /// <returns>A sorted list of recent projects, first one is the last accessed one.</returns>
    Task<IEnumerable<RecentProject>> GetProjects();
    
    /// <summary>
    /// Add or update a project
    /// </summary>
    /// <param name="name">The name of the project</param>
    /// <param name="path">The path to the .titan project file</param>
    Task AddOrUpdateProject(string name, string path);
    /// <summary>
    /// Removes the projects from the list of recent projects
    /// </summary>
    /// <param name="project">The project to be removed</param>
    Task Remove(RecentProject project);
}
