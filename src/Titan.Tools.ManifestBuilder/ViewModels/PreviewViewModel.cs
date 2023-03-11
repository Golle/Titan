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
    public ICommand OpenInEditor { get; }
    public ICommand Refresh { get; }
    public PreviewViewModel(IMessenger? messenger = null, IApplicationState? appState = null, IExternalEditorService? editorService = null, IDialogService? dialogService = null)
    {
        messenger ??= Registry.GetRequiredService<IMessenger>();
        appState ??= Registry.GetRequiredService<IApplicationState>();
        editorService ??= Registry.GetRequiredService<IExternalEditorService>();
        dialogService ??= Registry.GetRequiredService<IDialogService>();

        messenger.Subscribe<ManifestNodeSelected>(this, async message =>
        {
            Node = message.Node;
            await LoadFileContents(appState.ProjectPath!);
        });

        Refresh = ReactiveCommand.CreateFromTask(() => LoadFileContents(appState.ProjectPath!));

        OpenInEditor = ReactiveCommand.CreateFromTask(async () =>
        {
            if (Node is ShaderNodeViewModel shader)
            {
                var result = await editorService.OpenEditor(shader.Item.Path);
                if (result.Failed)
                {
                    await dialogService.MessageBox("Error", $"Failed to launch the editor with message: {result.Error}");
                }
            }
        });
    }


    private async Task LoadFileContents(string projectPath)
    {
        if (Node is ShaderNodeViewModel shader)
        {
            FileContents = await File.ReadAllTextAsync(Path.Combine(projectPath!, shader.Item.Path));
        }
    }
    public PreviewViewModel()
    : this(null)
    {
    }
}
