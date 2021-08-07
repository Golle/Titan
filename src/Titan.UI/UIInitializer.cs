using Titan.Assets;
using Titan.ECS;
using Titan.Graphics.Loaders.Atlas;
using Titan.UI.Components;
using Titan.UI.Rendering;
using Titan.UI.Systems;

namespace Titan.UI
{
    public record UIConfiguration(uint MaxSprites = 100, uint MaxComponents = 1000);
    public static class UIInitializer
    {

        public static WorldBuilder WithDefaultUI(this WorldBuilder builder, UIConfiguration config, UIRenderQueue renderQueue, AssetsManager assetsManager, AtlasManager atlasManager)
        {
            return builder
                    .WithComponent<AssetComponent<SpriteComponent>>(count: config.MaxSprites)
                    .WithComponent<SpriteComponent>(count: config.MaxComponents)
                    .WithComponent<RectTransform>(count: config.MaxComponents)


                    .WithSystem(new SpriteLoaderSystem(assetsManager))
                    .WithSystem(new UIRenderSystem(renderQueue, atlasManager))
                    .WithSystem(new RectTransformSystem())

                ;

        }
    }
}
