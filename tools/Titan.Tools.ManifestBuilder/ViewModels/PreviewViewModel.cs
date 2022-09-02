using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
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
    public ICommand OpenInCode { get; }
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

        OpenInCode = ReactiveCommand.CreateFromTask(async () =>
        {
            if (Node is ShaderNodeViewModel shader)
            {
                Process.Start(new ProcessStartInfo("code", $"\"{appState.ProjectPath}\" {shader.Item.Path}"));
            }

            await Task.Delay(1000);
        });
    }

    public PreviewViewModel()
    : this(null, null)
    {
    }
}
