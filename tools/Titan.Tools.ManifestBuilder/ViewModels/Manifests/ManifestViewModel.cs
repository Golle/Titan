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
    private bool _isDirty;
    public bool IsDirty
    {
        get => _isDirty;
        set => SetProperty(ref _isDirty, value);
    }
    public string Name => Manifest.Name;
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

    public Manifest Manifest { get; }
    public ManifestViewModel(Manifest manifest, IMessenger messenger)
    {
        Manifest = manifest;
        Nodes = new IManifestTreeNode[]
        {
            new ManifestTreeNodeViewModel(nameof(manifest.Textures), manifest.Textures.Select(m =>
            {
                var viewModel = new TextureNodeViewModel(m);
                viewModel.PropertyChanged += (_, _) => IsDirty = true;
                return viewModel;
            })),
            new ManifestTreeNodeViewModel(nameof(manifest.Models), manifest.Models.Select(m =>
            {
                var viewModel = new ModelNodeViewModel(m);
                viewModel.PropertyChanged += (_, _) => IsDirty = true;
                return viewModel;
            })),
            new ManifestTreeNodeViewModel(nameof(manifest.Materials), manifest.Materials.Select(m =>
            {
                var viewModel = new MaterialNodeViewModel(m);
                viewModel.PropertyChanged += (_, _) => IsDirty = true;
                return viewModel;
            })),
        };

        _selectedNodeChanged = ReactiveCommand.CreateFromTask<IManifestTreeNode>(node => messenger.SendAsync(new ManifestNodeSelected(node)));
    }

    #region DESIGN_CONSTRUCTOR
    public ManifestViewModel()
    {
        if (Design.IsDesignMode)
        {
            Manifest = new Manifest { Name = "design manifest" };
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
