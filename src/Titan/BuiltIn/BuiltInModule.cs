using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.BuiltIn.Systems;
using Titan.ECS.Components;
using Titan.Modules;
using Titan.Setup;
using Titan.Systems;

namespace Titan.BuiltIn;

internal struct BuiltInModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        //NOTE(Jens): maybe these should be part of other modules, so the "caller" can decide how many of each should exist.
        builder
            //NOTE(Jens): Make sure we use the correct Pool types at some point. 
            .AddComponent<Transform2D>(ComponentPoolType.Sparse)
            .AddComponent<BoxCollider2D>(ComponentPoolType.Sparse)
            .AddComponent<Sprite>(ComponentPoolType.Sparse)
            .AddComponent<Transform3D>(ComponentPoolType.Sparse)
            .AddComponent<TransformRect>(ComponentPoolType.Sparse)
            .AddComponent<TextureComponent>(ComponentPoolType.Sparse)
            .AddComponent<Camera2D>(100, ComponentPoolType.Packed) // make this configurable
            
            .AddSystemToStage<TimeStepSystem>(SystemStage.First, RunCriteria.CheckInline)
            .AddSystem<Camera2DSystem>(RunCriteria.Always)
            .AddSystem<Transform2DSystem>()
            .AddSystem<BoxCollider2DSystem>()
            .AddResource<TimeStep>()
            //NOTE(Jens): using a system like this will delay any sprites from showing for 2 frames. (on 60hz that's an additional 32ms before anything is visible)
            .AddSystem<SpriteLoadSystem>()
            ;

        return true;
    }

    public static bool Init(IApp app)
    {
        return true;
    }

    public static bool Shutdown(IApp app)
    {
        return true;
    }
}
