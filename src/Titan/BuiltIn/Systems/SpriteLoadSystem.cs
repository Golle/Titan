using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.Core;
using Titan.Core.Maths;
using Titan.ECS.Queries;
using Titan.Graphics.D3D12;
using Titan.Graphics.Resources;
using Titan.Systems;

namespace Titan.BuiltIn.Systems;

internal struct SpriteLoadSystem : ISystem
{
    private MutableStorage<Sprite> _sprites;
    private MutableStorage<TextureComponent> _textures;
    private AssetsManager _assetsManager;
    private EntityQuery _entities;
    private ObjectHandle<IResourceManager> _resourceManager;

    public void Init(in SystemInitializer init)
    {
        _sprites = init.GetMutableStorage<Sprite>();
        _textures = init.GetMutableStorage<TextureComponent>();
        _assetsManager = init.GetAssetsManager();
        _resourceManager = init.GetManagedApi<IResourceManager>();
        _entities = init.CreateQuery(new EntityQueryArgs().With<Sprite>().Not<TextureComponent>());
    }

    public unsafe void Update()
    {
        var resourceManager = _resourceManager.Value;
        foreach (ref readonly var entity in _entities)
        {
            ref var sprite = ref _sprites[entity];
            if (_assetsManager.IsLoaded(sprite.Asset))
            {
                //NOTE(Jens): we need something that detects if a texture component has been removed and trigger an unload.
                ref var texture = ref _textures.Add(entity);
                texture.TextureHandle = _assetsManager.GetAssetHandle<Texture>(sprite.Asset);
                var d3D12Texture = (D3D12Texture*)resourceManager.AccessTexture(texture.TextureHandle);

                texture.TextureId = (uint)d3D12Texture->SRV.Index;
                texture.Height = d3D12Texture->Texture.Height;
                texture.Width = d3D12Texture->Texture.Width;

                if (sprite.SourceRect == default)
                {
                    sprite.SourceRect = new Rectangle(0, 0, (int)texture.Width, (int)texture.Height);
                }
            }
        }
    }

    public bool ShouldRun() => _entities.HasEntities();
}
