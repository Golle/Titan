using Titan.Tools.ManifestBuilder.Services;
using Titan.Tools.ManifestBuilder.ViewModels.Manifests;
using Titan.Tools.ManifestBuilder.ViewModels.Previews;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class PreviewViewModel : ViewModelBase
{
    private PreviewViewModelBase? _content;
    public PreviewViewModelBase? Content
    {
        get => _content;
        private set => SetProperty(ref _content, value);
    }

    public PreviewViewModel(IMessenger messenger)
    {
        messenger.Subscribe<ManifestNodeSelected>(this, async message =>
        {
            if (Content != null)
            {
                await Content.Unload();
            }

            Content = message.Node switch
            {
                ShaderNodeViewModel shader => new ShaderPreviewViewModel(shader),
                ModelNodeViewModel model => new ModelPreviewViewModel(model),
                _ => null
            };

            if (Content != null)
            {
                await Content.Load();
            }
        });
    }
    public PreviewViewModel()
    : this(Registry.GetRequiredService<IMessenger>())
    {
    }
}
