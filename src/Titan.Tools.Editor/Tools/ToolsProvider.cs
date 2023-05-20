using Microsoft.Extensions.DependencyInjection;
using Titan.Tools.Editor.ViewModels;

namespace Titan.Tools.Editor.Tools;

public class ToolsProvider
{
    private readonly IServiceProvider _serviceProvider;

    public ToolsProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public IEnumerable<ITool> GetTools()
    {
        yield return _serviceProvider.GetRequiredService<CompileTool>();
        yield return _serviceProvider.GetRequiredService<RunGameTool>();
        yield return _serviceProvider.GetRequiredService<PublishTool>();
    }
}
