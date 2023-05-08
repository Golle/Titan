namespace Titan.Tools.Editor.Configuration;

/// <summary>
/// Helper function for managing the recent projects
/// </summary>
internal class RecentProjects : IRecentProjects
{
    private readonly IAppConfiguration _appConfiguration;

    public RecentProjects(IAppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
    }

    public async Task<IEnumerable<RecentProject>> GetProjects()
    {
        var config = await _appConfiguration.GetConfig();
        return config.RecentProjects.OrderByDescending(p => p.LastAccessed);
    }

    public async Task AddOrUpdateProject(string name, string path)
    {
        var config = await _appConfiguration.GetConfig();
        var project = config.RecentProjects.FirstOrDefault(p => p.Name == name && p.Path == path);
        if (project != null)
        {
            project.LastAccessed = DateTime.Now;
        }
        else
        {
            config.RecentProjects.Add(new RecentProject
            {
                Name = name,
                Path = path,
                LastAccessed = DateTime.Now
            });
        }

        await _appConfiguration.SaveConfig(config);
    }
}
