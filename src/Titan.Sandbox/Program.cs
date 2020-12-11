using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Titan;
using Titan.Core;
using Titan.Core.Common;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.ECS.Components;
using Titan.ECS.Registry;
using Titan.ECS.Systems;
using Titan.ECS.World;
using Titan.EntitySystem.Components;
using Titan.Graphics.Camera;
using Titan.Graphics.Materials;
using Titan.Graphics.Meshes;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Textures;
using Titan.Input;
using TimeStep = Titan.ECS.Systems.TimeStep;

//LOGGER.InitializeLogger(new ConsoleLogger(new TimeLogFormatter(new DateTimeWrapper())));

var pipelineConfiguration = new PipelineConfiguration(@"F:\Git\Titan\resources\render_pipeline.json");
var assetDirectory = new AssetsDirectory(@"F:\Git\Titan\resources");
var displayConfiguration = DisplayConfiguration.FromFile(@"F:\Git\Titan\resources\display.json");


var gameConfigurationBuilder = new GameConfigurationBuilder()
    .WithAssetsDirectory(assetDirectory)
    .WithDisplayConfiguration(displayConfiguration)
    .WithPipelineConfiguration(pipelineConfiguration)
    .WithSystem<SandboxSystem>(nameof(SandboxSystem), TitanSystems.Transform3D)
    .WithSystem<AnotherSandboxSystem>(nameof(AnotherSandboxSystem), nameof(SandboxSystem))
    
    .WithDefaultConsoleLogger();


using var application = Application.Create(gameConfigurationBuilder);
application.Run();

return;














var simpleMesh = @"F:\Git\Titan\resources\models\sphere.dat";
var simpleMesh2 = @"F:\Git\Titan\resources\models\sponza.dat";
var simpleMesh1 = @"F:\Git\Titan\resources\models\sphere1.dat";

using var engine = EngineBuilder.CreateDefaultBuilder()
    .ConfigureResourcesBasePath(() => @"F:\Git\Titan\resources\")
    .ConfigureWindow(1920, 1080, "Donkey box #2")
#if DEBUG
    .ConfigureDebug(() => true)
#else
    .ConfigureDebug(() => false)
#endif
    .Build();

var window = engine.Window;
var container = engine.Container;
var pipeline = (GraphicsPipeline)container.GetInstance<IGraphicsPipeline>();
var cameraManager = container.GetInstance<ICameraManager>();
var textureLoader = container.GetInstance<ITextureLoader>();
var meshLoader = container.GetInstance<IMeshLoader>();
var input = container.GetInstance<IInputHandler>();
var eventQueue = container.GetInstance<IEventQueue>();
var materialsLoader = container.GetInstance<IMaterialsLoader>();
var materialsManager = container.GetInstance<IMaterialsManager>();
var configuration = container.GetInstance<TitanConfiguration>();
var materialConfigurations = materialsLoader.LoadMaterials(configuration.GetPath("models/sponza.json"));
var sponzaMaterials = materialConfigurations.Select(m => materialsManager.CreateFromConfiguration(m)).ToArray();

//var mesh = meshLoader.LoadMesh(simpleMesh);
var sphere = meshLoader.LoadMesh(simpleMesh1)[0];
var sponzaMeshes = meshLoader.LoadMesh(simpleMesh2);
    
var texture1 = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\blue.png");
var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\link.png");
//var texture = textureLoader.LoadTexture(@"F:\tmp\globe.png");

var lightQueue = container.GetInstance<ILigthRenderQueue>();
var meshRenderQueue = container.GetInstance<IMeshRenderQueue>();
//meshRenderQueue.Submit(mesh, Matrix4x4.CreateTranslation(-2f, 0, 0), texture);
//meshRenderQueue.Submit(mesh1, Matrix4x4.CreateTranslation(1,0,0), texture);
    
    

var s = Stopwatch.StartNew();
var frames = 0;
var rotation = Vector2.Zero;
const float maxRotation = (float)Math.PI / 2f - 0.01f;
var camera = cameraManager.GetCamera();

var firstPerson = false;


#region LIGHTS

var lightPosition = new Vector3(0, 350, 0);
var lightVelocity = 4f;
var maxDinstance = 1200f;
#endregion


while (engine.Window.Update())
{
    meshRenderQueue.Reset();
    lightQueue.Reset();

    eventQueue.Update();
    input.Update();

    #region LIGHT CALCULATION

    if (lightPosition.X > maxDinstance)
    {
        lightPosition.X = maxDinstance-0.1f;
        lightVelocity = -lightVelocity;
    }
    else if (lightPosition.X < -maxDinstance)
    {
        lightPosition.X = -maxDinstance+0.1f;
        lightVelocity = -lightVelocity;
    }

    lightPosition.X += lightVelocity;

    lightQueue.Submit(lightPosition);

    #endregion

    var lightMatrix = Matrix4x4.Transpose(Matrix4x4.CreateScale(Vector3.One * 10) *
                                          Matrix4x4.CreateFromQuaternion(Quaternion.Identity) *
                                          Matrix4x4.CreateTranslation(lightPosition));

    meshRenderQueue.Submit(sphere, lightMatrix, texture1, null);

    var modelMatrix = Matrix4x4.Transpose(Matrix4x4.CreateScale(Vector3.One) *
                                          Matrix4x4.CreateFromQuaternion(Quaternion.Identity) *
                                          Matrix4x4.CreateTranslation(Vector3.Zero));
    meshRenderQueue.Submit(sponzaMeshes[0], modelMatrix, texture, sponzaMaterials);
    meshRenderQueue.Submit(sponzaMeshes[1], modelMatrix, texture, sponzaMaterials);
    //meshRenderQueue.Submit(mesh1, Matrix4x4.CreateTranslation(new Vector3(1,1,1)), texture1);
    //meshRenderQueue.Submit(mesh1, Matrix4x4.CreateTranslation(new Vector3(2,2,1)), texture);
    //meshRenderQueue.Submit(mesh1, Matrix4x4.CreateTranslation(new Vector3(3,3,1)), texture1);
        


    #region TEMP INPUT HANLDING
    if (firstPerson)
    {
        var multiplier = input.IsKeyDown(KeyCode.Shift) ? 15f : 1f;
        var speed = 0.2f * multiplier;
        var mousePos = input.MouseLastPosition - input.MousePosition;

            
        if (mousePos.Y != 0 || mousePos.X != 0)
        {
            const float constant = 0.003f;
            rotation.X -= mousePos.X * constant;
            rotation.Y = Math.Clamp(rotation.Y + mousePos.Y * constant, -maxRotation, maxRotation);
            camera.Rotate(new Vector3(rotation, 0));
        }

        if (input.IsKeyDown(KeyCode.W))
        {
            camera.MoveForward(-speed);
        }
        if (input.IsKeyDown(KeyCode.S))
        {
            camera.MoveForward(speed);
        }
        if (input.IsKeyDown(KeyCode.A))
        {
            camera.MoveSide(speed);
        }
        if (input.IsKeyDown(KeyCode.D))
        {
            camera.MoveSide(-speed);
        }
        if (input.IsKeyDown(KeyCode.V))
        {
            camera.MoveUp(speed);
        }
        if (input.IsKeyDown(KeyCode.C))
        {
            camera.MoveUp(-speed);
        }

            
    }
    if (input.IsKeyPressed(KeyCode.Space))
    {
        firstPerson = !firstPerson;
        window.ToggleMouse();
    }

    camera.Update();
    #endregion

    window.SetTitle($"[{input.MousePosition.X}, {input.MousePosition.Y}]");

    pipeline.Execute();

    if (input.IsKeyPressed(KeyCode.Escape))
    {
        LOGGER.Debug("Escape key pressed, exiting the game.");
        engine.Stop();
    }
    frames++;
    if (s.Elapsed.TotalSeconds > 2.0f)
    {
        s.Stop();
        Console.WriteLine($"FPS: {frames / s.Elapsed.TotalSeconds:##}");

        s.Restart();
        frames = 0;
    }



    //GC.Collect(); // Force garbage collection to see if we have any interop pointers that needs to be pinned.
}


public class AnotherSandboxSystem : IEntitySystem
{
    private readonly IInputHandler _inputHandler;

    public AnotherSandboxSystem(IInputHandler inputHandler)
    {
        _inputHandler = inputHandler;
    }

    public void OnPreUpdate()
    {
        if (_inputHandler.IsKeyPressed(KeyCode.F1))
        {
            Environment.Exit(1000);
        }
    }

    public void OnUpdate(in TimeStep timeStep)
    {
        
    }

    public void Dispose() { }
}


public class SandboxSystem : IEntitySystem
{
    private readonly IComponentPool<Transform3D> _transform;
    private readonly IEntityFilter _filter;
    private readonly IEntityFilter _filter2;

    public SandboxSystem(IWorld world)
    {
        _filter = world.FilterManager.Create(new EntityFilterConfiguration().With<Transform3D>());
        _filter2 = world.FilterManager.Create(new EntityFilterConfiguration().With<Transform3D>());
        _transform = world.GetComponentPool<Transform3D>();
    }

    public void OnUpdate(in TimeStep timeStep)
    {
        foreach (ref readonly var entity in _filter.GetEntities())
        {
            ref var transform = ref _transform[entity];
            transform.Position += Vector3.UnitX * timeStep;
        }
    }

    public void Dispose() { }
}
