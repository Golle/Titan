using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Common;
using Titan.Tools.ManifestBuilder.Services;

namespace Titan.Tools.ManifestBuilder.ViewModels;

public class ManifestViewModel : ViewModelBase
{
    private readonly IManifestService _manifestService;
    private string? _title;
    public string? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    private string? _error;
    public string? Error
    {
        get => _error;
        set
        {

            SetProperty(ref _error, value);
            HasError = !string.IsNullOrWhiteSpace(value);
            this.RaisePropertyChanged(nameof(HasError));
        }
    }
    public bool HasError { get; private set; }

    public NotifyTaskCompletion<ObservableCollection<ItemNode>> Items { get; }

    private ItemNode? _selectedItem;
    public ItemNode? SelectedItem
    {
        get => _selectedItem;
        set => SetProperty(ref _selectedItem, value);
    }
    public ManifestViewModel()
    : this(null!)
    {
    }

    internal ManifestViewModel(IManifestService manifestService)
    {
        _manifestService = manifestService;

        Items = new NotifyTaskCompletion<ObservableCollection<ItemNode>>(LoadManifests());
    }

    private async Task<ObservableCollection<ItemNode>> LoadManifests()
    {
        var result = new ObservableCollection<ItemNode>();
        if (Design.IsDesignMode)
        {
            return result;
        }

        Error = null;
        var manifests = await _manifestService.ListManifests();
        if (manifests.Failed)
        {
            Error = manifests.Error ?? "Unknown error occurred.";
            return result;
        }

        var manifest = manifests.Data!.FirstOrDefault();
        if (manifest == null)
        {
            Error = "NO manifest was found";
            return result;
        }

        Title = manifest.Name;
        result.Add(new ItemNode
        {
            Name = "Textures",
            Children = new ObservableCollection<ItemNode>(manifest.Textures.Select(t => new ItemNode { Name = System.IO.Path.GetFileNameWithoutExtension(t.Path) }))
        });

        result.Add(new ItemNode
        {
            Name = "Models",
            Children = new ObservableCollection<ItemNode>(manifest.Models.Select(t => new ItemNode { Name = System.IO.Path.GetFileNameWithoutExtension(t.Path) }))
        });
        return result;
    }

    public class ItemNode
    {
        public ObservableCollection<ItemNode> Children { get; set; } = new();
        public required string Name { get; set; }

    }
}
