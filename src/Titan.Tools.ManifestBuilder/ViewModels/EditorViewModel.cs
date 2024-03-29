using Microsoft.Extensions.DependencyInjection;
using Titan.Tools.ManifestBuilder.ViewModels.Manifests;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public record LoadProjectResult(bool Success, string? Error = null);
public class EditorViewModel : ViewModelBase
{
    public ViewModelBase ManifestView { get; }
    public ViewModelBase ContentView { get; }
    public ViewModelBase FileInfoView { get; }

    public EditorViewModel(IServiceProvider provider)
    {
        ContentView = provider.GetRequiredService<ContentViewModel>();
        FileInfoView = provider.GetRequiredService<FileInfoViewModel>();
        ManifestView = provider.GetRequiredService<ManifestViewModel>();
    }
}
