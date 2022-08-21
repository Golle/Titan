using System.Collections.Generic;
using System.Linq;
using DynamicData;
using DynamicData.Binding;

namespace Titan.Tools.ManifestBuilder.ViewModels.Manifest;

public class ManifestTreeNodeViewModel : ViewModelBase
{
    private string _name;
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }
    public IObservableCollection<ManifestTreeNodeViewModel> Children { get; } = new ObservableCollectionExtended<ManifestTreeNodeViewModel>();

    public ManifestTreeNodeViewModel(string name)
        : this(name, Enumerable.Empty<ManifestTreeNodeViewModel>())
    {

    }
    public ManifestTreeNodeViewModel(string name, IEnumerable<ManifestTreeNodeViewModel> children)
    {
        _name = name;
        Children.AddRange(children);
    }
}
