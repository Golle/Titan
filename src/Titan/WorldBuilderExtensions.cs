using Titan.Assets;
using Titan.Components;
using Titan.ECS;
using Titan.ECS.Components;
using Titan.Graphics.Loaders.Models;
using Titan.Systems;
using Titan.Systems.Debugging;
using Titan.Systems.Physics;
using Titan.UI.Animation;
using Titan.UI.Components;
using Titan.UI.Debugging;

namespace Titan;

public static class WorldBuilderExtensions
{
    public static WorldBuilder WithDefault3D(this WorldBuilder builder, uint numberOfAssets = 20, uint numberOfModels = 100) =>
        builder
            .WithComponent<AssetComponent<Model>>(ComponentPoolTypes.DynamicPacked, numberOfAssets)
            .WithComponent<ModelComponent>(ComponentPoolTypes.DynamicPacked, numberOfModels)
            .WithSystem<Render3DSystem>()
            .WithSystem<ModelLoaderSystem>()
#if DEBUG
            .WithSystem<BoundingBox2DDebugSystem>()
#endif
        ;

    public static WorldBuilder WithDefault2D(this WorldBuilder builder, uint numberOfAssets = 2, uint numberOfSprites = 1000) => 
        WithDefaultUI(builder, numberOfAssets, numberOfSprites);

    public static WorldBuilder WithDefault2DCollisions(this WorldBuilder builder, uint numberOfColliders = 100) =>
        builder
            .WithComponent<BoxColliderComponent>(ComponentPoolTypes.Packed, numberOfColliders)
            .WithSystem<BoxCollision2DSystem>();


    public static WorldBuilder WithDefaultUI(this WorldBuilder builder, uint numberOfAssets = 2, uint numberOfSprites = 1000) =>
        builder
            .WithComponent<AssetComponent<SpriteComponent>>(ComponentPoolTypes.DynamicPacked, count: numberOfAssets)
            .WithComponent<SpriteComponent>(count: numberOfSprites)
            .WithSystem<SpriteLoaderSystem>()
            .WithComponent<AssetComponent<TextComponent>>(ComponentPoolTypes.DynamicPacked, count: 1)
            .WithComponent<RectTransform>(count: numberOfSprites)
            .WithComponent<InteractableComponent>(count: numberOfSprites)
            .WithComponent<TextComponent>(count: numberOfSprites)

            .WithSystem<TextLoaderSystem>()
            .WithSystem<SpriteRenderSystem>()
            .WithSystem<TextRenderSystem>()
            .WithSystem<TextUpdateSystem>()
            .WithSystem<RectTransformSystem>()
            .WithSystem<InteractableSystem>()
            .WithComponent<AnimateTranslation>(count: 100)
#if DEBUG
            .WithSystem<BoundingBox2DDebugSystem>()
#endif
    ;
}
