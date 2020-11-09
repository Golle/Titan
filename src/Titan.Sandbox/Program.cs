using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Titan;
using Titan.Core;
using Titan.Core.Messaging;
using Titan.Graphics.Materials;
using Titan.Graphics.Meshes;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Textures;
using Titan.Input;

//var simpleMesh = @"F:\Git\GameDev\resources\models\cube.dat";
var simpleMesh = @"F:\Git\GameDev\resources\models\sphere.dat";
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
    //using var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\blue.png");
    //using var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\tree01.png");
    var texture = textureLoader.LoadTexture(@"F:\tmp\globe.png");

    var meshRenderQueue = container.GetInstance<IMeshRenderQueue>();
    meshRenderQueue.Submit(mesh, Matrix4x4.CreateTranslation(-2f, 0, 0), texture);
    meshRenderQueue.Submit(mesh1, Matrix4x4.CreateTranslation(1,0,0), texture);
    

    var s = Stopwatch.StartNew();
    var frames = 0;
    while (engine.Window.Update())
    {
        eventQueue.Update();
        input.Update();
        window.SetTitle($"[{input.MousePosition.X}, {input.MousePosition.Y}]");

        pipeline.Execute();

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
