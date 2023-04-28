using ReactiveUI;
using System.Windows.Input;
using Titan.Tools.Core.Manifests;
using Titan.Tools.ManifestBuilder.Services;
using Titan.Tools.ManifestBuilder.ViewModels.Manifests;

namespace Titan.Tools.ManifestBuilder.ViewModels.Previews;

public class ShaderPreviewViewModel : PreviewViewModelBase
{
    public ShaderNodeViewModel Shader { get; }
    public ICommand OpenInEditor { get; }
    public ICommand LoadContents { get; }

    private string _contents = string.Empty;

    public string FileContents
    {
        get => _contents;
        set => SetProperty(ref _contents, value);
    }

    public ShaderPreviewViewModel(ShaderNodeViewModel shader, IDialogService? dialogService = null, IExternalEditorService? editorService = null, IApplicationState? appState = null)
    {
        dialogService ??= Registry.GetRequiredService<IDialogService>();
        editorService ??= Registry.GetRequiredService<IExternalEditorService>();
        appState ??= Registry.GetRequiredService<IApplicationState>();

        Shader = shader;
        OpenInEditor = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await editorService.OpenEditor(shader.Item.Path);
            if (result.Failed)
            {
                await dialogService.MessageBox("Error", $"Failed to launch the editor with message: {result.Error}");
            }
        });

        LoadContents = ReactiveCommand.CreateFromTask(async () =>
        {
            FileContents = await File.ReadAllTextAsync(Path.Combine(appState.ProjectPath!, shader.Item.Path));
        });
    }

    public override Task Load()
    {
        LoadContents.Execute(null);
        return Task.CompletedTask;
    }

    public ShaderPreviewViewModel()
    : this(new ShaderNodeViewModel(new ShaderItem()))
    {
    }
}
