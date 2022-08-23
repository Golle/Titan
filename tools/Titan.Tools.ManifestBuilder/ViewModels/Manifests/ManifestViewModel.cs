using System;
using System.Linq;
using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Titan.Tools.ManifestBuilder.Models;
using Titan.Tools.ManifestBuilder.Services;

namespace Titan.Tools.ManifestBuilder.ViewModels.Manifests;
public record struct ManifestNodeSelected(IManifestTreeNode Node);

public class ManifestViewModel : ViewModelBase
{
    public string Name => _manifest.Name;
    private readonly Manifest _manifest;
    public IManifestTreeNode[] Nodes { get; }

    private IManifestTreeNode? _selectedNode;
    public IManifestTreeNode? SelectedNode
    {
        get => _selectedNode;
        set
        {
            SetProperty(ref _selectedNode, value);
            _selectedNodeChanged.Execute(_selectedNode);
        }
    }
    private readonly ICommand _selectedNodeChanged;
    public ManifestViewModel(Manifest manifest, IMessenger messenger)
    {
        _manifest = manifest;
        Nodes = new IManifestTreeNode[]
        {
            new ManifestTreeNodeViewModel(nameof(manifest.Textures), manifest.Textures.Select(m => new TextureNodeViewModel(m))),
            new ManifestTreeNodeViewModel(nameof(manifest.Models), manifest.Models.Select(m => new ModelNodeViewModel(m))),
        };

        _selectedNodeChanged = ReactiveCommand.CreateFromTask<IManifestTreeNode>(node => messenger.SendAsync(new ManifestNodeSelected(node)));
    }

    #region DESIGN_CONSTRUCTOR
    public ManifestViewModel()
    {
        if (Design.IsDesignMode)
        {
            _manifest = new Manifest { Name = "design manifest" };
            Nodes = new IManifestTreeNode[]
            {
                new ManifestTreeNodeViewModel("Test_01", new[] { new ManifestTreeNodeViewModel("Child 01"), new ManifestTreeNodeViewModel("Child 02") }),
                new ManifestTreeNodeViewModel("Test_02", new[] { new ManifestTreeNodeViewModel("Child 01"), new ManifestTreeNodeViewModel("Child 02", new[] { new ManifestTreeNodeViewModel("Sub child 01") }) })
            };
            _selectedNodeChanged = ReactiveCommand.Create(() => { });
        }
        else
        {
            throw new InvalidOperationException("Design constructor called from non design mode.");
        }
    }
    #endregion
}
