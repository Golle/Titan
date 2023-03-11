using System.Numerics;
using Titan;
using Titan.Assets;
using Titan.Audio;
using Titan.BuiltIn.Components;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Maths;
using Titan.Core.Memory;
using Titan.ECS;
using Titan.ECS.Entities;
using Titan.ECS.Queries;
using Titan.Graphics;
using Titan.Input;
using Titan.Runners;
using Titan.Sandbox;
using Titan.Setup.Configs;
using Titan.Systems;
using Titan.Windows;

const bool UseRawAssets = true;

var devAssetFolder = Path.GetFullPath($"{AppContext.BaseDirectory}../../../assets");
var devPakFolder = Path.Combine(devAssetFolder, "bin");
var devEngineFolder = Path.GetFullPath($"{AppContext.BaseDirectory}../../../../../");
var devConfig = new AssetsDevConfiguration(devAssetFolder, devPakFolder, devEngineFolder, UseRawAssets);
using var _ = Logger.Start(new ConsoleLogger(), 10_000);

App.Create(new AppCreationArgs())
#if DEBUG
    .AddConfig(devConfig)
#endif
    .AddAssetsManifest<AssetRegistry.SampleManifest01>()
    .AddAssetsManifest<AssetRegistry.SampleManifest02>()
    .AddConfig(ECSConfig.Default with
    {
        MaxEntities = 50_000
    })
    .AddConfig(WindowConfig.Default with
    {
        //Width = 2560,
        //Height = 1440,
        Width = 1024,
        Height = 768,
        Windowed = false,
        Title = "Titan Sandbox",
        AlwaysOnTop = false
    })
#if DEBUG
    .AddConfig(GraphicsConfig.Default with { Debug = true, TripleBuffering = true, Vsync = true, AllowTearing = true, Fullscreen = false })
#else 
    .AddConfig(GraphicsConfig.Default with { Debug = false, TripleBuffering = true })
#endif
    .AddModule<GraphicsModule>()
    .AddModule<AudioModule>()
    //.AddModule<AudioModule>()

    .AddModule<InputModule>()
    .AddSystem<LoadAndPlaySoundSystem>()
    .AddSystem<EntityTestSystem>()
    .AddSystem<EntityQueryTestSystem>()
    .AddSystem<LoadTheSprites>()
    .AddSystem<MoveSpritesSystem>()
    .AddSystem<MovingSpriteRelationshipSystem>()
    .AddSystem<SetupCameraSystem>()
    .AddSystem<ReloadSpriteSystem>()
    //.AddSystem<EntityHiearchySyste>() //Enable this for a entity hiearchy sample

    .UseRunner<WindowedRunner>()
    .Build()
    .Run()
    ;


namespace Titan.Sandbox
{

    public struct ReloadSpriteSystem : ISystem
    {
        private EntityQuery _entities;
        private ReadOnlyStorage<Sprite> _sprites;
        private AssetsManager _assetsManager;
        private InputManager _input;

        public void Init(in SystemInitializer init)
        {
            _entities = init.CreateQuery(new EntityQueryArgs().With<Sprite>());
            _sprites = init.GetReadOnlyStorage<Sprite>();
            _assetsManager = init.GetAssetsManager();
            _input = init.GetInputManager();
        }

        public void Update()
        {
            if (_input.IsKeyPressed(KeyCode.R))
            {
                Logger.Trace<ReloadSpriteSystem>($"Reload triggered! {_entities.Count}");
                foreach (ref readonly var entity in _entities)
                {
                    _assetsManager.Reload(_sprites[entity].Asset);
                }
            }
        }
    }
    public struct SetupCameraSystem : ISystem
    {
        private ComponentManager _componentManager;
        private EntityManager _entityManager;
        private AssetsManager _assestManager;
        private InputManager _input;
        private ObjectHandle<IWindow> _window;
        private MutableStorage<Camera2D> _cameras;
        private MutableStorage<Transform2D> _transform;

        private Entity _entity;

        public void Init(in SystemInitializer init)
        {
            _componentManager = init.GetComponentManager();
            _entityManager = init.GetEntityManager();
            _assestManager = init.GetAssetsManager();
            _window = init.GetManagedApi<IWindow>();
            _cameras = init.GetMutableStorage<Camera2D>();
            _transform = init.GetMutableStorage<Transform2D>();
            _input = init.GetInputManager();
        }

        public void Update()
        {
            if (_entity.IsInvalid)
            {
                _entity = _entityManager.Create();
                _componentManager.AddComponent(_entity, Transform2D.Default);
                _componentManager.AddComponent(_entity, new Camera2D
                {
                    Size = _window.Value.Size,
                    ClearColor = Color.Magenta
                });
                _componentManager.AddComponent(_entity, new Sprite
                {
                    Pivot = new(0.5f),
                    Color = Color.White,
                    Layer = 1,
                    Asset = _assestManager.Load(AssetRegistry.SampleManifest02.Textures.Redsheet),
                    SourceRect = new Rectangle(386, 178, 36, 36)
                });
            }
            else
            {
                //NOTE(Jens): Update the camera size to the window size
                ref var camera = ref _cameras[_entity];
                camera.Size = _window.Value.Size;

                ref var transform = ref _transform[_entity];

                const float speed = 5.01f;
                var movement = Vector2.Zero;
                var scale = Vector2.Zero;
                if (_input.IsKeyDown(KeyCode.Left))
                {
                    movement.X -= speed;
                }
                if (_input.IsKeyDown(KeyCode.Right))
                {
                    movement.X += speed;
                }
                if (_input.IsKeyDown(KeyCode.Up))
                {
                    movement.Y += speed;
                }
                if (_input.IsKeyDown(KeyCode.Down))
                {
                    movement.Y -= speed;
                }
                if (_input.IsKeyDown(KeyCode.Add))
                {
                    scale += Vector2.One * 0.03f;
                }
                if (_input.IsKeyDown(KeyCode.Subtract))
                {
                    scale -= Vector2.One * 0.03f;
                }
                transform.Position += movement;
                transform.Scale += scale;
            }
        }
    }


    public struct MovingSpriteRelationshipSystem : ISystem
    {
        private EntityManager _entityManager;
        private AssetsManager _assetsManager;
        private ComponentManager _componentManager;
        private InputManager _input;
        private MutableStorage<Transform2D> _transform;
        private Entity _entity;
        private Entity _child;
        private Entity _childOfChild;
        private ObjectHandle<IWindow> _window;

        public void Init(in SystemInitializer init)
        {
            _componentManager = init.GetComponentManager();
            _entityManager = init.GetEntityManager();
            _assetsManager = init.GetAssetsManager();
            _input = init.GetInputManager();
            _transform = init.GetMutableStorage<Transform2D>();
            _window = init.GetManagedApi<IWindow>();
        }

        public void Update()
        {
            if (_entity.IsInvalid /*&& _input.IsKeyPressed(KeyCode.Enter)*/)
            {
                var transform2D = Transform2D.Default;
                var sprite = new Sprite
                {
                    Asset = _assetsManager.Load(AssetRegistry.SampleManifest01.Textures.Bluesheet),
                    Color = Color.White,
                    Pivot = new Vector2(0.5f),
                    SourceRect = new Rectangle(0, 48, 188, 46)
                };

                _entity = _entityManager.Create();
                _componentManager.AddComponent(_entity, transform2D with { Position = Vector2.Zero });
                _componentManager.AddComponent(_entity, sprite);

                var child = _entityManager.Create();
                _componentManager.AddComponent(child, transform2D with { Position = Vector2.One * 100 });
                _componentManager.AddComponent(child, sprite with { Color = Color.Red with { B = 1f } });
                _entityManager.Attach(_entity, child);
                _child = child;

                var child2 = _entityManager.Create();
                _componentManager.AddComponent(child2, transform2D with { Position = Vector2.One * 50 });
                _componentManager.AddComponent(child2, sprite with { Color = Color.Green });
                _entityManager.Attach(child, child2);
                _childOfChild = child2;
            }
            else if (_entity.IsValid)
            {
                ref var transform = ref _transform[_entity];
                transform.Rotation += 0.01f;
                //transform.Position.X += 0.11f;
                //transform.Scale += Vector2.One * 0.001f;

                ref var t1 = ref _transform[_child];
                t1.Rotation -= 0.03f;
                //t1.Scale -= Vector2.One * 0.0003f;


                ref var t2 = ref _transform[_childOfChild];
                t2.Rotation += 0.1f;
                //t2.Scale += Vector2.One * 0.001f;
            }
        }

    }


    public struct EntityHiearchySyste : ISystem
    {
        private EntityManager _entityManager;

        private bool _complete;
        public void Init(in SystemInitializer init)
        {
            _entityManager = init.GetEntityManager();
        }

        public void Update()
        {
            var middle = _entityManager.Create();

            var root = _entityManager.Create();

            var ent = root;
            for (var i = 0; i < 10; ++i)
            {
                var newEntity = i == 5 ? middle : _entityManager.Create();
                _entityManager.Attach(ent, newEntity);
                ent = newEntity;
            }
            _entityManager.DebugPrint(root);
            _complete = true;
        }

        public bool ShouldRun() => !_complete;
    }

    public struct LoadTheSprites : ISystem
    {
        private EntityManager _entityManager;
        private AssetsManager _assetsManager;
        private ComponentManager _componentManager;
        private InputManager _input;

        public void Init(in SystemInitializer init)
        {
            _componentManager = init.GetComponentManager();
            _entityManager = init.GetEntityManager();
            _assetsManager = init.GetAssetsManager();
            _input = init.GetInputManager();
        }

        public void Update()
        {
            for (var i = 0; i < 1000; ++i)
            {
                var entity = _entityManager.Create();

                _componentManager.AddComponent(entity, Transform2D.Default with
                {
                    Position = new Vector2(100 + 500 * (i % 2), i > 1 ? 0 : 500),
                    Scale = Vector2.One * (Random.Shared.NextSingle() + 0.4f)
                });

                _componentManager.AddComponent(entity, new Sprite
                {
                    Asset = (i % 4) switch
                    {
                        0 => _assetsManager.Load(AssetRegistry.SampleManifest01.Textures.Bluesheet),
                        1 => _assetsManager.Load(AssetRegistry.SampleManifest01.Textures.Greensheet),
                        2 => _assetsManager.Load(AssetRegistry.SampleManifest02.Textures.Greysheet),
                        3 or _ => _assetsManager.Load(AssetRegistry.SampleManifest02.Textures.Redsheet)
                    },
                    Color = new Color(1f, 0f, 1f),
                    Pivot = Vector2.Zero

                });

            }
        }

        public bool ShouldRun()
        {
            return _input.IsKeyPressed(KeyCode.Space);
        }
    }

    public struct MoveSpritesSystem : ISystem
    {
        private EntityQuery _query;
        private MutableStorage<Transform2D> _transform;
        private MutableStorage<Sprite> _sprite;
        private InputManager _input;
        private uint _currentIndex;

        public void Init(in SystemInitializer init)
        {
            _query = init.CreateQuery(new EntityQueryArgs().With<Sprite>().With<Transform2D>());
            _transform = init.GetMutableStorage<Transform2D>();
            _sprite = init.GetMutableStorage<Sprite>();
            _input = init.GetInputManager();
        }

        public void Update()
        {
            for (var i = 0; i < 9; ++i)
            {
                var keyCode = (int)KeyCode.One + i;
                if (_input.IsKeyPressed((KeyCode)keyCode))
                {
                    _currentIndex = (uint)i;
                    break;
                }
            }

            var speed = _input.IsKeyDown(KeyCode.Shift) ? 10f : 1f;
            var count = 0;
            foreach (ref readonly var entity in _query)
            {

                ref var sprite = ref _sprite[entity];
                if (count++ == _currentIndex)
                {
                    var distance = 0.1f * speed;
                    ref var transform = ref _transform[entity];

                    if (_input.IsKeyDown(KeyCode.Up))
                    {
                        transform.Position.Y += distance;
                    }
                    if (_input.IsKeyDown(KeyCode.Down))
                    {
                        transform.Position.Y -= distance;
                    }
                    if (_input.IsKeyDown(KeyCode.Left))
                    {
                        transform.Position.X -= distance;
                    }
                    if (_input.IsKeyDown(KeyCode.Right))
                    {
                        transform.Position.X += distance;
                    }

                    //sprite.Color = new Color(1f, 1f, 1f);
                }
                else
                {
                    //sprite.Color = new Color(1f, 0f, 1f);
                }
            }
        }
    }
    public struct LoadAndPlaySoundSystem : ISystem
    {
        private AssetsManager _assetsManager;
        private InputManager _inputManager;
        private AudioManager _audioManager;
        private Handle<Asset> _soundAsset;
        private Handle<Audio.Audio> _persistentAudio;

        public void Init(in SystemInitializer init)
        {
            _assetsManager = init.GetAssetsManager();
            _audioManager = init.GetAudioManager();
            _inputManager = init.GetInputManager();
        }

        public void Update()
        {
            if (_soundAsset.IsInvalid)
            {
                Logger.Trace<LoadAndPlaySoundSystem>($"load asset {nameof(AssetRegistry.SampleManifest01.Textures.Boom)}");
                _soundAsset = _assetsManager.Load(AssetRegistry.SampleManifest01.Textures.Boom);
            }

            if (_inputManager.IsKeyPressed(KeyCode.Q))
            {
                _audioManager.PlayOnce(_soundAsset);
            }

            if (_inputManager.IsKeyPressed(KeyCode.W))
            {
                if (_persistentAudio.IsInvalid)
                {
                    Logger.Info<LoadAndPlaySoundSystem>("Create and Play");
                    _persistentAudio = _audioManager.CreateAndPlay(_soundAsset);
                }
                else
                {
                    Logger.Info<LoadAndPlaySoundSystem>("Play");
                    _audioManager.Play(_persistentAudio);
                }
            }

            if (_inputManager.IsKeyPressed(KeyCode.E) && _persistentAudio.IsValid)
            {
                Logger.Info<LoadAndPlaySoundSystem>("Destroy");
                _audioManager.Destroy(ref _persistentAudio);
            }

            if (_inputManager.IsKeyPressed(KeyCode.Space))
            {
                _assetsManager.Unload(_soundAsset);
            }

            if (_inputManager.IsKeyPressed(KeyCode.One))
            {
                SetVolume(_persistentAudio, 0.1f);
            }
            else if (_inputManager.IsKeyPressed(KeyCode.Two))
            {
                SetVolume(_persistentAudio, 0.2f);
            }
            else if (_inputManager.IsKeyPressed(KeyCode.Three))
            {
                SetVolume(_persistentAudio, 0.3f);
            }
            else if (_inputManager.IsKeyPressed(KeyCode.Four))
            {
                SetVolume(_persistentAudio, 0.4f);
            }
            else if (_inputManager.IsKeyPressed(KeyCode.Five))
            {
                SetVolume(_persistentAudio, 0.5f);
            }   
            else if (_inputManager.IsKeyPressed(KeyCode.Six))
            {
                SetVolume(_persistentAudio, 0.6f);
            }
            else if (_inputManager.IsKeyPressed(KeyCode.Seven))
            {
                SetVolume(_persistentAudio, 0.7f);
            }
            else if (_inputManager.IsKeyPressed(KeyCode.Eight))
            {
                SetVolume(_persistentAudio, 0.8f);
            }
            else if (_inputManager.IsKeyPressed(KeyCode.Nine))
            {
                SetVolume(_persistentAudio, 0.9f);
            }
            else if (_inputManager.IsKeyPressed(KeyCode.Zero))
            {
                SetVolume(_persistentAudio, 1f);
            }
        }

        void SetVolume(Handle<Audio.Audio> handle, float volume)
        {
            //_audioManager.SetMasterVolume(volume);
            if (_persistentAudio.IsInvalid)
            {
                Logger.Warning<LoadAndPlaySoundSystem>("Press W to load the audio into the persistent audio.");
            }
            else
            {
                _audioManager.SetVolume(handle, volume);
            }
        }
        public bool ShouldRun() => true;
    }
    public struct EntityQueryTestSystem : ISystem
    {
        private EntityQuery _query;
        private EntityManager _entityManager;
        private MutableStorage<Transform3D> _transform3D;

        private uint _state;
        public void Init(in SystemInitializer init)
        {
            _query = init.CreateQuery(new EntityQueryArgs().With<Transform3D>());
            _entityManager = init.GetEntityManager();
            _transform3D = init.GetMutableStorage<Transform3D>();
        }

        public void Update()
        {
            _state++;
            if (_state == 1)
            {
                var entity = _entityManager.Create();
                _transform3D.Add(entity) = Transform3D.Default;
                Logger.Trace<EntityQueryTestSystem>($"Created entity {entity.Id}");
            }
            foreach (ref readonly var entity in _query)
            {
                Logger.Trace<EntityQueryTestSystem>($"Entity {entity.Id} is in the query! Transform: {_transform3D.Get(entity).Scale}");
                if (_state >= 10)
                {
                    _transform3D.Remove(entity);
                }
            }
        }
    }

    public struct EntityTestSystem : ISystem
    {
        private const uint TotalEntities = 2;
        private EntityManager _entityManager;
        private int _state;
        private ObjectHandle<IMemoryManager> _memoryManager;
        private TitanArray<Entity> _entities;
        private MutableStorage<Transform2D> _transform2D;

        public void Init(in SystemInitializer init)
        {
            _entityManager = init.GetEntityManager();
            _memoryManager = init.GetManagedApi<IMemoryManager>();
            _entities = _memoryManager.Value.AllocArray<Entity>(TotalEntities);
            _transform2D = init.GetMutableStorage<Transform2D>();
        }

        public void Update()
        {
            switch (_state)
            {
                case 0:
                    for (var i = 0; i < TotalEntities; i++)
                    {
                        _entities[i] = _entityManager.Create();
                    }
                    Logger.Trace<EntityTestSystem>($"Created {TotalEntities} entities! Last ID: {_entities[TotalEntities - 1]}");
                    break;
                case 1:
                    for (var i = 0; i < TotalEntities; i++)
                    {
                        ref var component = ref _transform2D.Add(_entities[i]);
                        component = new Transform2D
                        {
                            Scale = Vector2.One * 2 * i,

                            Position = Vector2.One * i
                        };
                    }

                    break;
                case 2:

                    foreach (ref readonly var entity in _entities.AsReadOnlySpan())
                    {
                        ref readonly var transform = ref _transform2D[entity];
                        Logger.Trace($"Transform for Entity {entity.Id}: {transform.Position} {transform.Rotation} {transform.Scale}");
                    }
                    break;
                case 3:
                    foreach (ref readonly var entity in _entities.AsReadOnlySpan())
                    {
                        _transform2D.Remove(entity);
                    }
                    break;
                case 6:

                    for (var i = 0; i < TotalEntities; i++)
                    {
                        _entityManager.Destroy(_entities[i]);
                    }
                    Logger.Trace<EntityTestSystem>($"Destroyed {TotalEntities} entities! Last ID {_entities[TotalEntities - 1]}");
                    break;
            }
            _state++;
        }
    }
}
