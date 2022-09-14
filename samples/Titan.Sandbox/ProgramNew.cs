using System.Numerics;
using Titan.Assets.NewAssets;
using Titan.Components;
using Titan.Core;
using Titan.Core.Logging;
using Titan.ECS.App;
using Titan.ECS.Entities;
using Titan.ECS.Modules;
using Titan.ECS.Scheduler;
using Titan.ECS.Systems;
using Titan.Graphics.Modules;
using Titan.Input;
using Titan.Input.Modules;
using Titan.Modules;
using Titan.Runner;
using Titan.Sandbox;
using EntityFilter = Titan.ECS.Entities.EntityFilter;
using Titan.Graphics;


// Folders used during development.
var baseDir = GetBaseDirectory();
var assetsFolder = Path.Combine(baseDir, "assets");
var titanPakFolder = Path.Combine(assetsFolder, "bin");


App.SetupAndRun(builder => builder
    .AddResource(new LoggingConfiguration
    {
        Enabled = true,
        Type = LoggerType.Console,
        FilePath = @"c:\tmp\titan.log"
    })
    .AddConfiguration(new AssetsDevConfiguration(assetsFolder, titanPakFolder))
    .AddConfiguration(AssetsConfiguration.CreateFromRegistry<AssetRegistry.SampleManifest01>())
    .AddConfiguration(AssetsConfiguration.CreateFromRegistry<AssetRegistry.AnotherManifest>())
    .AddResource(new ECSConfiguration { MaxEntities = 1_000_000 })
    //.AddResource(SchedulerConfiguration.SingleThreaded)
    .AddModule<CoreModule>()
    .AddModule<AssetsModule>()
    .AddModule<WindowModule>()
    .AddModule<Graphics3DModule>()
    .AddModule<InputModule>()
    
    .UseRunner<WindowRunner>()
    //.UseRunner<HeadlessRunner>()

    .AddSystem<SampleSystem1>()
    .AddSystem<SampleSystem2>()
    .AddSystem<AssetLoadingSampleSystem>()
    .AddSystemExperimental<SampleSystemInstance>()
    .AddComponents<Transform3DComponent>(maxComponents: 10_000)
    .AddComponents<TestComponent>(maxComponents: 1000)
    .AddResource<GlobalFrameCounter>()
    .AddResource<SharedAssets>()

);


static string GetBaseDirectory()
{
    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    const string BaseFileName = "Titan.Sandbox.csproj";
    return FindParentWithFile(BaseFileName, baseDirectory, 7)
           ?? baseDirectory;

    static string FindParentWithFile(string filename, string? path, int steps)
    {
        if (steps == 0 || path == null)
        {
            return null;
        }
        return File.Exists(Path.Combine(path, filename))
            ? path
            : FindParentWithFile(filename, Directory.GetParent(path)?.FullName, steps - 1);
    }
}


namespace Titan.Sandbox
{
    internal struct SampleSystemInstance : ISystem
    {
        private ReadOnlyStorage<Transform3DComponent> _transform;
        private EntityFilter _filter;
        public void Init(in SystemsInitializer init)
        {
            _transform = init.GetReadOnlyStorage<Transform3DComponent>();
            _filter = init.CreateFilter(new EntityFilterConfig().With<Transform3DComponent>());
        }

        public void Update()
        {
            foreach (ref readonly var entity in _filter.GetEntities())
            {
                ref readonly var transform = ref _transform.Get(entity);
                //Logger.Info<SampleSystemInstance>($"Entity: {entity.Id} position: {transform.Position}");
            }
        }
        public bool ShouldRun() => _filter.HasEntities();
    }

    internal struct SampleSystemStatic : IStructSystem<SampleSystemStatic>
    {
        private ReadOnlyStorage<Transform3DComponent> _transform;
        private EntityFilter _filter;
        public static void Init(ref SampleSystemStatic system, in SystemsInitializer init)
        {
            system._transform = init.GetReadOnlyStorage<Transform3DComponent>();
            system._filter = init.CreateFilter(new EntityFilterConfig().With<Transform3DComponent>());
        }
        public static void Update(ref SampleSystemStatic system)
        {
            foreach (ref readonly var entity in system._filter.GetEntities())
            {
                ref readonly var transform = ref system._transform.Get(entity);
                //Logger.Info<SampleSystemStatic>($"Entity: {entity.Id} position: {transform.Position}");
            }
        }
        public static bool ShouldRun(in SampleSystemStatic system) => system._filter.HasEntities();
    }

    internal struct TestComponent : IComponent { }
    internal struct SampleSystem1 : IStructSystem<SampleSystem1>
    {
        private MutableResource<GlobalFrameCounter> _global;
        private EntityHandler _entityHandler;
        private bool _initialized;
        private MutableStorage<Transform3DComponent> _transform;
        private EntityFilter _filter;
        private EntityFilter _notFilter;
        private MutableStorage<TestComponent> _test;

        public static void Init(ref SampleSystem1 system, in SystemsInitializer init)
        {
            system._global = init.GetMutableResource<GlobalFrameCounter>();
            system._filter = init.CreateFilter(new EntityFilterConfig().With<Transform3DComponent>());
            system._notFilter = init.CreateFilter(new EntityFilterConfig().With<TestComponent>().Not<Transform3DComponent>());
            system._entityHandler = init.GetEntityHandler();
            system._transform = init.GetMutableStorage<Transform3DComponent>();
            system._test = init.GetMutableStorage<TestComponent>();

            system._initialized = false;
        }

        // call an instance method if that is preferred. THe code gen "should" be the same after JIT. 
        public static void Update(ref SampleSystem1 system) => system.Update();
        private void Update()
        {
            _global.Get().FrameCounter++;

            if (!_initialized)
            {
                for (var i = 0; i < 3; ++i)
                {
                    var entity = _entityHandler.Create();
                    _transform.Create(entity, new Transform3DComponent
                    {
                        Position = Vector3.One * i,
                        Rotation = Quaternion.Identity,
                        Scale = Vector3.One
                    });
                    _test.Create(entity);
                    //Logger.Trace<FrameCounter>($"Creating entity with ID: {entity.Id}");
                }
                _initialized = true;
                return;
            }
            //if (_counter++ == 3)
            //{
            //    _transform.Destroy(new Entity(1, 0));
            //}

            foreach (ref readonly var entity in _filter.GetEntities())
            {
                // always true
                //if (_transform.Contains(entity))
                {
                    ref readonly var t = ref _transform.Get(entity);
                    //_transform.Destroy(entity);
                    //Logger.Error<FrameCounter>($"{entity.Id} Position: {t.Position}");
                }
            }

            foreach (ref readonly var entity in _notFilter.GetEntities())
            {
                //Logger.Trace<FrameCounter>($"Entity with ID: {entity.Id} is missing a transform component. adding");
                //_transform.Create(entity);
            }
        }

        public static bool ShouldRun(in SampleSystem1 system) => true;
    }

    internal struct SampleSystem2 : IStructSystem<SampleSystem2>
    {
        private ReadOnlyResource<GlobalFrameCounter> _global;
        private ReadOnlyResource<KeyboardState> _keyState;
        private int _counter;

        public static void Init(ref SampleSystem2 system, in SystemsInitializer init)
        {
            system._global = init.GetReadOnlyResource<GlobalFrameCounter>();
            system._keyState = init.GetReadOnlyResource<KeyboardState>();
            system._counter = 0;
        }

        public static void Update(ref SampleSystem2 system)
        {
            ref readonly var keyboardState = ref system._keyState.Get();
            if (keyboardState.IsKeyReleased(KeyCode.A))
            {
                Logger.Warning<SampleSystem2>("Key released");
            }
            if (keyboardState.IsKeyPressed(KeyCode.A))
            {
                Logger.Warning<SampleSystem2>("Key pressed");
            }

            if (keyboardState.IsKeyDown(KeyCode.S))
            {
                system._counter++;
            }
            else if (keyboardState.IsKeyReleased(KeyCode.S))
            {
                Logger.Trace<SampleSystem2>($"Keycount: {system._counter}");
            }
        }

        public static bool ShouldRun(in SampleSystem2 system) => true;
    }


    struct GlobalFrameCounter : IResource
    {
        public long FrameCounter;
    }

    internal struct TextureSample { } // Fake texture just to test AssetManagement
    internal struct SharedAssets : IResource
    {
        public Handle<Asset> Texture1;
        public Handle<Asset> Texture2;
        public Handle<Asset> Texture3;
    }
    internal struct AssetLoadingSampleSystem : IStructSystem<AssetLoadingSampleSystem>
    {
        private AssetManager AssetManager;
        private MutableResource<SharedAssets> SharedAssets;
        private bool IsDone;
        public static void Init(ref AssetLoadingSampleSystem system, in SystemsInitializer init)
        {
            system.AssetManager = init.GetAssetManager();
            system.SharedAssets = init.GetMutableResource<SharedAssets>();
        }

        public static void Update(ref AssetLoadingSampleSystem system)
        {
            ref var assetManager = ref system.AssetManager;
            ref var assets = ref system.SharedAssets.Get();

            if (assets.Texture1.IsInvalid())
            {
                assets.Texture1 = assetManager.Load(AssetRegistry.AnotherManifest.Textures.Redsheet);
                Logger.Trace<AssetLoadingSampleSystem>($"Load Texture1");
            }
            else
            {
                if (assetManager.IsLoaded(assets.Texture1))
                {
                    var handle = assetManager.GetAssetHandle<TextureSample>(assets.Texture1);
                    Logger.Trace<AssetLoadingSampleSystem>($"Texture 1 handle: {handle}");
                    Logger.Trace<AssetLoadingSampleSystem>($"Unload Texture1 and load Texture2");
                    assetManager.Unload(ref assets.Texture1);
                    assets.Texture2 = assetManager.Load(AssetRegistry.AnotherManifest.Textures.SeqoeUiLight);
                    system.IsDone = true;
                }
            }
        }

        public static bool ShouldRun(in AssetLoadingSampleSystem system) => !system.IsDone;
        //=> system.SharedAssets.Get().Texture1.IsInvalid();
    }
}


