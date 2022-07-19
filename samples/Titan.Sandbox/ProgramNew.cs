using System.Numerics;
using Titan.Components;
using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Entities;
using Titan.ECS.EntitiesNew;
using Titan.ECS.Modules;
using Titan.ECS.Systems;
using Titan.ECS.SystemsV2;
using Titan.EventNewer;
using Titan.Graphics.Modules;
using Titan.Input;
using Titan.Input.Modules;
using Titan.Modules;
using Titan.Sandbox;


var f = new Titan.ECS.EntitiesNew.EntityFilter()
    .With<Transform3DComponent>()
    .Not<TestComponent>();


AppBuilder
    .Create()
    .AddResource(new ECSConfiguration { MaxEntities = 10_000_000 })
    //.AddResource(SchedulerConfiguration.SingleThreaded)
    .AddModule<CoreModule>()
    .AddModule<WindowModule>()
    .AddModule<InputModule>()
    .UseRunner<WindowRunner>()

    .AddSystem<FrameCounter>()
    .AddSystem<PrintFrameCounter>()
    .AddComponents<Transform3DComponent>(maxComponents: 100_000)
    .Build()
    .Run();

internal struct TestComponent : IComponent { }

namespace Titan.Sandbox
{
    //using var app = App
    //    .Create(AppCreationArgs.Default)
    //    .AddModule<CoreModule>()
    //    .AddResource(new WindowDescriptor { Height = 600, Width = 800, Resizable = true, Title = "Sandbox" })
    //    .AddModule<WindowModule>()
    //    .AddModule<InputModule>()
    //    .AddModule<RenderModule>()
    //    .AddSystemToStage<FrameCounter>(Stage.PreUpdate)
    //    .AddSystem<PrintFrameCounter>()
    //    .AddSystemToStage<FrameCounter>(Stage.PostUpdate)
    //    .AddResource(new GlobalFrameCounter())
    //    .Run()
    //    ;

    internal struct FrameCounter : IStructSystem<FrameCounter>
    {
        private MutableResource<GlobalFrameCounter> _global;
        private EntityHandler _entityHandler;
        private bool _initialized;
        private uint _counter;
        private MutableStorage3<Transform3DComponent> _transform;

        public static void Init(ref FrameCounter system, in SystemsInitializer init)
        {
            system._global = init.GetMutableResource<GlobalFrameCounter>();
            //init.GetEntities(new Titan.ECS.EntitiesNew.EntityFilter().With<Transform3DComponent>());
            system._entityHandler = init.GetEntityHandler();
            system._transform = init.GetMutableStorage<Transform3DComponent>();
            system._initialized = false;
        }

        // call an instance method if that is preferred. THe code gen "should" be the same after JIT. 
        public static void Update(ref FrameCounter system) => system.Update();
        private void Update()
        {
            _global.Get().FrameCounter++;

            if (!_initialized)
            {
                for (var i = 0; i < 10; ++i)
                {
                    var entity = _entityHandler.Create();
                    _transform.Create(entity, new Transform3DComponent
                    {
                        Position = Vector3.One * i,
                        Rotation = Quaternion.Identity,
                        Scale = Vector3.One
                    });
                    Logger.Trace<FrameCounter>($"Creating entity with ID: {entity.Id}");
                }
                _initialized = true;
                return;
            }
            if(_counter++ == 10)
            {
                _transform.Destroy(new Entity(1, 0));
            }
            for (var i = 0u; i < 10; ++i)
            {
                var entity = new Entity(i + 1, 0);
                if (_transform.Contains(entity))
                {
                    ref readonly var t = ref _transform.Get(entity);
                    //Logger.Error<FrameCounter>($"Creating entity with ID: {entity.Id} Position: {t.Position}");
                }
                else
                {
                    Logger.Trace<FrameCounter>($"Entity with ID: {entity.Id} is missing a transform component. adding");
                    _transform.Create(entity);
                }


            }
        }

        public static bool ShouldRun(in FrameCounter system) => true;
    }

    internal struct PrintFrameCounter : IStructSystem<PrintFrameCounter>
    {
        private ReadOnlyResource<GlobalFrameCounter> _global;
        private ReadOnlyResource<KeyboardState> _keyState;
        private int _counter;

        public static void Init(ref PrintFrameCounter system, in SystemsInitializer init)
        {
            system._global = init.GetReadOnlyResource<GlobalFrameCounter>();
            system._keyState = init.GetReadOnlyResource<KeyboardState>();
            system._counter = 0;
        }

        public static void Update(ref PrintFrameCounter system)
        {
            ref readonly var keyboardState = ref system._keyState.Get();
            if (keyboardState.IsKeyReleased(KeyCode.A))
            {
                Logger.Warning<PrintFrameCounter>("Key released");
            }
            if (keyboardState.IsKeyPressed(KeyCode.A))
            {
                Logger.Warning<PrintFrameCounter>("Key pressed");
            }

            if (keyboardState.IsKeyDown(KeyCode.S))
            {
                system._counter++;
            }
            else if (keyboardState.IsKeyReleased(KeyCode.S))
            {
                Logger.Trace<PrintFrameCounter>($"Keycount: {system._counter}");
            }
        }

        public static bool ShouldRun(in PrintFrameCounter system) => true;
    }


    struct GlobalFrameCounter : IResource
    {
        public long FrameCounter;
    }

    //internal class SandboxGame : Game
    //{
    //    public override EngineConfiguration ConfigureEngine(EngineConfiguration config) => config with
    //    {
    //        AssetsPath = "assets",
    //        BasePathSearchPattern = "Titan.Sandbox.csproj"
    //    };

    //    public override WindowConfiguration ConfigureWindow(WindowConfiguration config) =>
    //        config with
    //        {
    //            Height = 300,
    //            Width = 400,
    //            RawInput = false,
    //            Resizable = false,
    //            Windowed = true
    //        };

    //    public override SystemsConfiguration ConfigureSystems(SystemsBuilder builder) =>
    //        builder
    //            .Build();

    //    public override IEnumerable<WorldConfiguration> ConfigureWorlds()
    //    {
    //        yield return new WorldConfigurationBuilder(10_000)
    //            .WithComponent<Transform3DComponent>()
    //            .WithSystem<Transform3DSystem>()
    //            .Build("Game");

    //    }
    //}
}
