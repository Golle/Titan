using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Titan;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Messaging;
using Titan.Graphics.Camera;
using Titan.Graphics.Materials;
using Titan.Graphics.Meshes;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Textures;
using Titan.Input;

//var simpleMesh = @"F:\Git\GameDev\resources\models\cube.dat";
var simpleMesh = @"F:\Git\GameDev\resources\models\sphere.dat";
var simpleMesh2 = @"F:\Git\GameDev\resources\models_new\sponza.dat";
var simpleMesh1 = @"F:\Git\GameDev\resources\models\sphere1.dat";

using var engine = EngineBuilder.CreateDefaultBuilder()
    .ConfigureResourcesBasePath(() => @"F:\Git\Titan\resources\")
    .ConfigureWindow(1920, 1080, "Donkey box #2")
    .Build();

var window = engine.Window;
var container = engine.Container;
//var device = engine.Device;
var pipeline = (GraphicsPipeline)container.GetInstance<IGraphicsPipeline>();


unsafe
{
    var cameraManager = container.GetInstance<ICameraManager>();
    var textureLoader = container.GetInstance<ITextureLoader>();
    var meshLoader = container.GetInstance<IMeshLoader>();
    var input = container.GetInstance<IInputHandler>();
    var eventQueue = container.GetInstance<IEventQueue>();
    var materialsLoader = container.GetInstance<IMaterialsLoader>();
    var materialsManager = container.GetInstance<IMaterialsManager>();
    var configuration = container.GetInstance<TitanConfiguration>();
    var materialConfigurations = materialsLoader.LoadMaterials(configuration.GetPath("materials.json"));
    var stuffs = materialConfigurations.Select(m => materialsManager.CreateFromConfiguration(m)).ToArray();

    var mesh = meshLoader.LoadMesh(simpleMesh);
    var mesh1 = meshLoader.LoadMesh(simpleMesh1);
    var mesh2 = meshLoader.LoadMesh(simpleMesh2);
    
    //var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\blue.png");
    var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\link.png");
    //var texture = textureLoader.LoadTexture(@"F:\tmp\globe.png");

    var meshRenderQueue = container.GetInstance<IMeshRenderQueue>();
    //meshRenderQueue.Submit(mesh, Matrix4x4.CreateTranslation(-2f, 0, 0), texture);
    //meshRenderQueue.Submit(mesh1, Matrix4x4.CreateTranslation(1,0,0), texture);
    meshRenderQueue.Submit(mesh2, Matrix4x4.CreateTranslation(0,0,0), texture);
    

    var s = Stopwatch.StartNew();
    var frames = 0;
    var rotation = Vector2.Zero;
    const float maxRotation = (float)Math.PI / 2f - 0.01f;
    var camera = cameraManager.GetCamera();

    var firstPerson = false;
    while (engine.Window.Update())
    {
        

        eventQueue.Update();
        input.Update();


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
        //frames++;
        //if (s.Elapsed.TotalSeconds > 2.0f)
        //{
        //    s.Stop();
        //    Console.WriteLine($"FPS: {frames/s.Elapsed.TotalSeconds:##}");
            
        //    s.Restart();
        //    frames = 0;
        //}



        //GC.Collect(); // Force garbage collection to see if we have any interop pointers that needs to be pinned.
    }

    //{
    //    using ComPtr<ID3D11Debug> d3dDebug = default;
    //    fixed (Guid* debugGuidPtr = &D3D11Common.D3D11Debug)
    //    {
    //        Common.CheckAndThrow(engine.Device.Ptr->QueryInterface(debugGuidPtr, (void**)d3dDebug.GetAddressOf()), "QueryInterface");
    //    }
    //    Common.CheckAndThrow(d3dDebug.Get()->ReportLiveDeviceObjects(D3D11_RLDO_FLAGS.D3D11_RLDO_DETAIL), "ReportLiveDeviceObjects");
    //}
    //deferredShadingPixelShader.Dispose();
    //backbuffer.Dispose();
}
