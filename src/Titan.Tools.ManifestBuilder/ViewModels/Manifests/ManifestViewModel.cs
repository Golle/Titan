using System.Windows.Input;
using Avalonia.Controls;
using ReactiveUI;
using Titan.Tools.Core.Manifests;
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

    private readonly ManifestTreeNodeViewModel _textures;
    private readonly ManifestTreeNodeViewModel _models;
    private readonly ManifestTreeNodeViewModel _shaders;
    private readonly ManifestTreeNodeViewModel _materials;
    private readonly ManifestTreeNodeViewModel _audio;

    public Manifest Manifest { get; }
    public ManifestViewModel(Manifest manifest, IMessenger messenger)
    {
        Manifest = manifest;

        _textures = new ManifestTreeNodeViewModel(nameof(manifest.Textures), manifest.Textures.Select(m => CheckForDirty(new TextureNodeViewModel(m))));
        _models = new ManifestTreeNodeViewModel(nameof(manifest.Models), manifest.Models.Select(m => CheckForDirty(new ModelNodeViewModel(m))));
        _shaders = new ManifestTreeNodeViewModel(nameof(manifest.Shaders), manifest.Shaders.Select(m => CheckForDirty(new ShaderNodeViewModel(m))));
        _materials = new ManifestTreeNodeViewModel(nameof(manifest.Materials), manifest.Materials.Select(m => CheckForDirty(new MaterialNodeViewModel(m))));
        _audio = new ManifestTreeNodeViewModel(nameof(manifest.Audio), manifest.Audio.Select(m => CheckForDirty(new AudioNodeViewModel(m))));
        Nodes = new IManifestTreeNode[] { _textures, _models, _shaders, _materials, _audio };

        _selectedNodeChanged = ReactiveCommand.CreateFromTask<IManifestTreeNode>(node => messenger.SendAsync(new ManifestNodeSelected(node)));

        T CheckForDirty<T>(T viewModel) where T : ViewModelBase
        {
            viewModel.PropertyChanged += (_, _) => IsDirty = true;
            return viewModel;
        }
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
            _audio = _models = _shaders = _materials = _textures = new ManifestTreeNodeViewModel(nameof(Manifest.Textures), Enumerable.Empty<TextureNodeViewModel>());
        }
        else
        {
            throw new InvalidOperationException("Design constructor called from non design mode.");
        }
    }
    #endregion

    public void AddItemToManifest(ManifestItem item)
    {
        switch (item)
        {
            case TextureItem texture:
                Manifest.Textures.Add(texture);
                _textures.Children.Add(new TextureNodeViewModel(texture));
                break;
            case ModelItem model:
                Manifest.Models.Add(model);
                _models.Children.Add(new ModelNodeViewModel(model));
                break;
            case ShaderItem shader:
                Manifest.Shaders.Add(shader);
                _shaders.Children.Add(new ShaderNodeViewModel(shader));
                break;
            case MaterialItem material:
                Manifest.Materials.Add(material);
                _materials.Children.Add(new MaterialNodeViewModel(material));
                break;
            case AudioItem audio:
                Manifest.Audio.Add(audio);
                _audio.Children.Add(new AudioNodeViewModel(audio));
                break;
            default:
                throw new NotSupportedException($"Type {item.GetType().Name} is not supported.");
        }
        IsDirty = true;
    }
}
