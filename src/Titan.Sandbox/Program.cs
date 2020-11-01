using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Titan;
using Titan.Core.Common;
using Titan.Core.Messaging;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.State;
using Titan.Graphics.Meshes;
using Titan.Graphics.Pipeline;
using Titan.Graphics.Pipeline.Configuration;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Pipeline.Renderers;
using Titan.Graphics.Shaders;
using Titan.Graphics.Textures;
using Titan.Input;
using Titan.Windows.Events;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

//var simpleMesh = @"F:\Git\GameDev\resources\models\cube.dat";
var simpleMesh = @"F:\Git\GameDev\resources\models\sphere.dat";

using var engine = EngineBuilder.CreateDefaultBuilder()
    .ConfigureResourcesBasePath(() => @"F:\Git\Titan\resources\")
    .ConfigureWindow(1920, 1080, "Donkey box #2")
    .Build();

var window = engine.Window;
var device = engine.Device;
var container = engine.Container;



unsafe
{

    var textureLoader = container.GetInstance<ITextureLoader>();
    var shaderCompiler = container.GetInstance<IShaderCompiler>();
    var meshLoader = container.GetInstance<IMeshLoader>();
    var input = container.GetInstance<IInputHandler>();
    var eventQueue = container.GetInstance<IEventQueue>();
    var shaderManager = container.GetInstance<IShaderManager>();

    using var mesh = meshLoader.LoadMesh(simpleMesh);
    //using var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\tree01.png");
    using var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\blue.png");


    using var samplerState = new SamplerState(device);
    
    using var immediateContext = new ImmediateContext(device);
    immediateContext.SetViewport(new Viewport(window.Width, window.Height));

    using var depthStencilState = new DepthStencilState(device);
    immediateContext.SetDepthStencilState(depthStencilState);

    var modelPosition = new Vector3(0, 0, 0);
    var modelRot = new Vector2();
    var cameraRot = new Vector2();
    var position = new Vector3(0, 0, -5);
    var projectionMatrix = MatrixExtensions.CreatePerspectiveLH(1f, window.Height / (float)window.Width, 0.5f, 10000f);

    var backbuffer = new BackBufferRenderTargetView(device);

    

    
    // full screen stuff 
    using var backbufferVertexBuffer = new VertexBuffer<BackbufferVertex>(device, new BackbufferVertex[]
    {
        new BackbufferVertex{Position = new Vector2(-1, 0), Texture = new Vector2(0, 1)},
        new BackbufferVertex{Position = new Vector2(-1, 1), Texture = new Vector2(0, 0)},
        new BackbufferVertex{Position = new Vector2(0, 1), Texture = new Vector2(1, 0)},
        new BackbufferVertex{Position = new Vector2(0, 0), Texture = new Vector2(1, 1)},
        new BackbufferVertex{Position = new Vector2(0, 0), Texture = new Vector2(0, 1)},
        new BackbufferVertex{Position = new Vector2(0, 1), Texture = new Vector2(0, 0)},
        new BackbufferVertex{Position = new Vector2(1, 1), Texture = new Vector2(1, 0)},
        new BackbufferVertex{Position = new Vector2(1, 0), Texture = new Vector2(1, 1)},
        new BackbufferVertex{Position = new Vector2(-1, -1), Texture = new Vector2(0, 1)},
        new BackbufferVertex{Position = new Vector2(-1, 0), Texture = new Vector2(0, 0)},
        new BackbufferVertex{Position = new Vector2(0, 0), Texture = new Vector2(1, 0)},
        new BackbufferVertex{Position = new Vector2(0, -1), Texture = new Vector2(1, 1)},
        new BackbufferVertex{Position = new Vector2(0, -1), Texture = new Vector2(0, 1)},
        new BackbufferVertex{Position = new Vector2(0, 0), Texture = new Vector2(0, 0)},
        new BackbufferVertex{Position = new Vector2(1, 0), Texture = new Vector2(1, 0)},
        new BackbufferVertex{Position = new Vector2(1, -1), Texture = new Vector2(1, 1)}
    });
    using var backbufferIndexBuffer = new IndexBuffer<ushort>(device, new ushort[] {0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8, 9, 10, 8, 10, 11, 12, 13, 14, 12, 14, 15});

    
    #region DEFERRED SHADING

    using var deferredShadingTexture = new Texture2D(device, (uint)window.Width/1, (uint)window.Height / 1, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
    using var deferredShadingRenderTarget = new RenderTargetView(device, deferredShadingTexture);
    using var deferredShadingResourceView = new ShaderResourceView(device, deferredShadingTexture);

    using var deferredShadingVertexBuffer = new VertexBuffer<BackbufferVertex>(device, new[]
    {
        new BackbufferVertex{Position = new Vector2(-1, -1), Texture = new Vector2(0, 1)},
        new BackbufferVertex{Position = new Vector2(-1, 1), Texture = new Vector2(0, 0)},
        new BackbufferVertex{Position = new Vector2(1, 1), Texture = new Vector2(1, 0)},
        new BackbufferVertex{Position = new Vector2(1, -1), Texture = new Vector2(1, 1)},
    });
    using var deferredShadingIndexbuffer = new IndexBuffer<ushort>(device, new ushort[] { 0, 1, 2, 0, 2, 3});


    #endregion
    using var deferredContext = new DeferredContext(device);

    using var deferredShading = new DeferredContext(device);
    
    using var swapchain = new Swapchain(device, true);

    var gbufferDeferredContext = new DeferredContext(device);

    var meshRenderQueue = container.GetInstance<IMeshRenderQueue>();
    meshRenderQueue.Submit(mesh, Matrix4x4.Identity);

    var gbufferRenderPass = new GBufferRenderPass(container.GetInstance<IBufferManager>(), container.GetInstance<IShaderManager>(), container.GetInstance<DefaultSceneRenderer>());

    var lightPosition = new Vector3(0, 0, -1);
    var lightVelocity = 0.05f;

    var deferredShaderProgram = shaderManager.Get(shaderManager.GetHandle("DeferredShadingDefault"));
    var backbufferShaderProgram = shaderManager.Get(shaderManager.GetHandle("BackbufferDefault"));
    while (window.Update())
    {
        #region HiddenEventLogic
        // Begin engine stuff
        // Should be handled by engine class 
        eventQueue.Update();
        input.Update();
        // End engine stuff


        foreach (ref readonly var @event in eventQueue.GetEvents<WindowResizedEvent>())
        {
            // if the window is resized release the current backbuffer object
            backbuffer.Dispose();
            // Resize the buffers on the device
            device.ResizeBuffers();
            // Update viewport
            immediateContext.SetViewport(new Viewport(@event.Width, @event.Height));
            // Update camera matrix
            projectionMatrix = MatrixExtensions.CreatePerspectiveLH(1f, @event.Height / (float)@event.Width, 0.5f, 10000f);
            // Create a new backbuffer render target
            backbuffer = new BackBufferRenderTargetView(device);
        }

        var speed = 0.1f;
        var distance = Vector3.Zero;
        if (input.IsKeyDown(KeyCode.W)) distance.Z -= speed; 
        if (input.IsKeyDown(KeyCode.S)) distance.Z += speed; 
        if (input.IsKeyDown(KeyCode.A)) distance.X += speed; 
        if (input.IsKeyDown(KeyCode.D)) distance.X -= speed; 
        if (input.IsKeyDown(KeyCode.V)) distance.Y += speed; 
        if (input.IsKeyDown(KeyCode.C)) distance.Y -= speed; 


        ref readonly var delta = ref input.MouseDeltaPosition;
        const float constant = 0.003f;
        if (delta.X != 0 && delta.Y != 0)
        {
            cameraRot.X -= delta.X * constant;
            cameraRot.Y += delta.Y * constant;
        }

        {
            modelRot.X += 0.03f;
            modelRot.Y -= 0.02f;
        }

        window.SetTitle($"[{input.MousePosition.X}, {input.MousePosition.Y}]");
        var modelRotation = Quaternion.CreateFromYawPitchRoll(modelRot.X, modelRot.Y, 0);

        ////var rotation = Quaternion.CreateFromYawPitchRoll(cameraRot.X, cameraRot.Y, 0);
        //var rotation = Quaternion.CreateFromYawPitchRoll(3,0, 0);
        //var forward = Vector3.Transform(new Vector3(0, 0, 1f), rotation);
        //var up = Vector3.Transform(new Vector3(0, 1, 0), rotation);
        //position += Vector3.Transform(distance, rotation);
        //var viewMatrix = Matrix4x4.CreateLookAt(position, position + forward, up);
        //var viewProjectionMatrix = viewMatrix * projectionMatrix;
        ////var viewProjectionMatrix = new Matrix4x4(-1, 0, 0, 0, 0, 1.77777779f, 0, 0, 0, 0, -1.00005f, -1, 0, 0, -0.5f, 0);
        //var camera = new Camera
        //{
        //    ViewMatrix = viewMatrix,
        //    ViewProjectMatrix = Matrix4x4.Transpose(viewProjectionMatrix)
        //};
        //using var cameraBuffer = new ConstantBuffer<Camera>(device, camera);

        var modelMatrix = Matrix4x4.Transpose(Matrix4x4.CreateScale(new Vector3(1, 1, 1)) *
                                              Matrix4x4.CreateFromQuaternion(modelRotation) *
                                              Matrix4x4.CreateTranslation(modelPosition));
        using var modelBuffer = new ConstantBuffer<Matrix4x4>(device, modelMatrix);

        #endregion


        if (lightPosition.X > 5.0f)
        {
            lightPosition.X = 4.99f;
            lightVelocity = -0.05f;
        }
        else if(lightPosition.X < -5.0f)
        {
            lightPosition.X = -4.99f;
            lightVelocity = 0.05f;
        }
        lightPosition.X += lightVelocity;

        using var lightSource = new ConstantBuffer<LightSource>(device, new LightSource {Position = lightPosition});

        // Render to the backbuffer (set up command list for drawing 3 squares)
        {
            deferredContext.SetViewport(new Viewport(window.Width, window.Height));
            deferredContext.ClearRenderTargetView(backbuffer, new Color(0.3f, 0, 0));
            deferredContext.SetRenderTarget(backbuffer);
            deferredContext.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            deferredContext.SetIndexBuffer(backbufferIndexBuffer);
            deferredContext.SetVertexBuffer(backbufferVertexBuffer);
            backbufferShaderProgram.Bind(deferredContext);
            deferredContext.SetPixelShaderSampler(samplerState);
            deferredContext.SetPixelShaderResource(gbufferRenderPass.NormalResourceView);
            deferredContext.DrawIndexed(6, 0);
            deferredContext.SetPixelShaderResource(gbufferRenderPass.AlbedoResourceView);
            deferredContext.DrawIndexed(6, 6);
            deferredContext.SetPixelShaderResource(gbufferRenderPass.DepthBufferView);
            deferredContext.DrawIndexed(6, 12);
            deferredContext.SetPixelShaderResource(deferredShadingResourceView);
            deferredContext.DrawIndexed(6, 18);
        }
        

        {
            deferredShading.SetViewport(new Viewport(window.Width / 1f, window.Height / 1f));
            deferredShading.ClearRenderTargetView(deferredShadingRenderTarget, new Color(0, 0.2f, 0.2f));
            deferredShading.SetVertexBuffer(deferredShadingVertexBuffer);
            deferredShading.SetIndexBuffer(deferredShadingIndexbuffer);
            deferredShaderProgram.Bind(deferredShading);
            deferredShading.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            deferredShading.SetPixelShaderSampler(samplerState);
            deferredShading.SetPixelShaderResource(gbufferRenderPass.AlbedoResourceView);
            deferredShading.SetPixelShaderResource(gbufferRenderPass.NormalResourceView, 1u);
            deferredShading.SetRenderTarget(deferredShadingRenderTarget);
            deferredShading.SetPixelShaderConstantBuffer(lightSource);
            deferredShading.DrawIndexed(deferredShadingIndexbuffer.NumberOfIndices, 0, 0);
        }


        // render the GBuffer
        gbufferDeferredContext.SetViewport(new Viewport(window.Width, window.Height));
        gbufferDeferredContext.SetPixelShaderSampler(samplerState);
        gbufferDeferredContext.SetPixelShaderResource(texture.ResourceView);
        gbufferDeferredContext.SetVertexShaderConstantBuffer(modelBuffer, 1);
        
        using var gbufferCommandList = gbufferRenderPass.Render(gbufferDeferredContext);
        using var deferredShadingCommandList = deferredShading.FinishCommandList();
        using var backbufferCommandList = deferredContext.FinishCommandList();

        immediateContext.ExecuteCommandList(gbufferCommandList);
        immediateContext.ExecuteCommandList(deferredShadingCommandList);
        immediateContext.ExecuteCommandList(backbufferCommandList);

        swapchain.Present();

        GC.Collect(); // Force garbage collection to see if we have any interop pointers that needs to be pinned.
    }

    //{
    //    using ComPtr<ID3D11Debug> d3dDebug = default;
    //    fixed (Guid* debugGuidPtr = &D3D11Common.D3D11Debug)
    //    {
    //        CheckAndThrow(device.Ptr->QueryInterface(debugGuidPtr, (void**)d3dDebug.GetAddressOf()), "QueryInterface");
    //    }
    //    CheckAndThrow(d3dDebug.Get()->ReportLiveDeviceObjects(D3D11_RLDO_FLAGS.D3D11_RLDO_DETAIL), "ReportLiveDeviceObjects");
    //}
    //deferredShadingPixelShader.Dispose();
    backbuffer.Dispose();
}

[StructLayout(LayoutKind.Sequential)]
struct Camera
{
    //[FieldOffset(0)]
    public Matrix4x4 ViewMatrix;
    //[FieldOffset(64)]
    public Matrix4x4 ViewProjectMatrix;
    //public Matrix4x4 WorldMatrix;
}

[StructLayout(LayoutKind.Sequential)]
public struct BackbufferVertex
{
    public Vector2 Position;
    public Vector2 Texture;
}

[StructLayout(LayoutKind.Sequential, Size = 48)]
public struct LightSource
{
    public Vector3 Position;
}
