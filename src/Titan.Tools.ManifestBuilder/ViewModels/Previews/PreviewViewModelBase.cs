namespace Titan.Tools.ManifestBuilder.ViewModels.Previews;

public abstract class PreviewViewModelBase : ViewModelBase
{
    public virtual Task Load() => Task.CompletedTask;
    public virtual Task Unload() => Task.CompletedTask;
}
