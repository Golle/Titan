using System.IO;
using Titan.Tools.ManifestBuilder.Services;
using Titan.Tools.ManifestBuilder.ViewModels.Manifests;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class PreviewViewModel : ViewModelBase
{
    private IManifestTreeNode? _node;


    public IManifestTreeNode? Node
    {
        get => _node;
        set => SetProperty(ref _node, value);
    }

    private string _fileContents = string.Empty;
    public string FileContents { get => _fileContents; set => SetProperty(ref _fileContents, value); }

    public PreviewViewModel(IMessenger? messenger, IApplicationState? appState)
    {
        messenger ??= Registry.GetRequiredService<IMessenger>();
        appState ??= Registry.GetRequiredService<IApplicationState>();

        messenger.Subscribe<ManifestNodeSelected>(this, async message =>
        {
            Node = message.Node;
            if (message.Node is ShaderNodeViewModel shader)
            {
                FileContents = await File.ReadAllTextAsync(Path.Combine(appState.ProjectPath!, shader.Item.Path));
            }
        });
    }

    public PreviewViewModel()
    : this(null, null)
    {

    }
}
