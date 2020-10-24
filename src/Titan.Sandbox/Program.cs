using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Runtime.InteropServices;
using Titan;
using Titan.Core.Common;
using Titan.Core.Messaging;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.State;
using Titan.Graphics.Meshes;
using Titan.Graphics.Textures;
using Titan.Input;
using Titan.Windows.Events;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;


var gBufferVertexShaderPath = @"F:\Git\Titan\resources\shaders\GBufferVertexShader.hlsl";
var gBufferPixelShaderPath = @"F:\Git\Titan\resources\shaders\GBufferPixelShader.hlsl";

var backbufferVertexShaderPath = @"F:\Git\Titan\resources\shaders\BackbufferVertexShader.hlsl";
var backbufferPixelShaderPath = @"F:\Git\Titan\resources\shaders\BackbufferPixelShader.hlsl";

var simpleMesh = @"F:\Git\GameDev\resources\models\cube.dat";

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

    using var mesh = meshLoader.LoadMesh(simpleMesh);
    using var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\tree01.png");


    using var gBufferCompiledVertexShader = shaderCompiler.CompileShaderFromFile(gBufferVertexShaderPath, "main", "vs_5_0");
    using var gBuffferVertexShader = new VertexShader(device, gBufferCompiledVertexShader);
    using var gBufferInputLayout = new InputLayout(device, gBufferCompiledVertexShader, new[]
    {
        new InputLayoutDescriptor("Position", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT),
        new InputLayoutDescriptor("Normal", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT),
        new InputLayoutDescriptor("Texture", DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT)
    });


    using var gBufferCompiledPixelShader = shaderCompiler.CompileShaderFromFile(gBufferPixelShaderPath, "main", "ps_5_0");
    using var gBufferPixelShader = new PixelShader(device, gBufferCompiledPixelShader);
    using var samplerState = new SamplerState(device);
    
    using var immediateContext = new ImmediateContext(device);
    immediateContext.SetViewport(new Viewport(window.Width, window.Height));

    
    
    using var gBufferNormals = new Texture2D(device, (uint)window.Width, (uint)window.Height, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
    using var gBufferNormalsTarget = new RenderTargetView(device, gBufferNormals);
    using var gBufferNormalsView = new ShaderResourceView(device, gBufferNormals);

    using var gBufferAlbedo = new Texture2D(device, (uint)window.Width, (uint)window.Height, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
    using var gBufferAlbedoTarget = new RenderTargetView(device, gBufferAlbedo);
    using var gBufferAlbedoView = new ShaderResourceView(device, gBufferAlbedo);

    using var gBufferDepth = new Texture2D(device, (uint)window.Width, (uint)window.Height, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
    using var gBufferDepthTarget = new RenderTargetView(device, gBufferDepth);
    using var gBufferDepthView = new ShaderResourceView(device, gBufferDepth);



    using var depthStencilState = new DepthStencilState(device);
    immediateContext.SetDepthStencilState(depthStencilState);

    var depthStencilTexture = new Texture2D(device, (uint)window.Width, (uint)window.Height, DXGI_FORMAT.DXGI_FORMAT_R24G8_TYPELESS, D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
    using var depthStencilView = new DepthStencilView(device, depthStencilTexture);
    using var depthStencilView2 = new ShaderResourceView(device, depthStencilTexture, DXGI_FORMAT.DXGI_FORMAT_R24_UNORM_X8_TYPELESS);

    #region MoreHiddenStuff
    var modelPosition = new Vector3(0, 0, 0);
    var modelRot = new Vector2();
    var cameraRot = new Vector2();
    var position = new Vector3(0, 0, -5);
    var projectionMatrix = MatrixExtensions.CreatePerspectiveLH(1f, window.Height / (float)window.Width, 0.5f, 10000f);


    #endregion
    var backbuffer = new BackBufferRenderTargetView(device);
    var gBufferRenderTargets = stackalloc ID3D11RenderTargetView*[3];
    gBufferRenderTargets[0] = gBufferAlbedoTarget.Ptr.Get();
    //gBufferRenderTargets[0] = backbuffer.Ptr.Get();
    gBufferRenderTargets[1] = gBufferNormalsTarget.Ptr.Get();
    gBufferRenderTargets[2] = gBufferDepthTarget.Ptr.Get();

    
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
        new BackbufferVertex{Position = new Vector2(0, -1), Texture = new Vector2(1, 1)}
    });
    using var backbufferIndexBuffer = new IndexBuffer<ushort>(device, new ushort[] { 0, 1, 2, 0, 2, 3, 4, 5, 6, 4, 6, 7, 8,9,10, 8, 10, 11 });

    using var backbufferCompiledVertexShader = shaderCompiler.CompileShaderFromFile(backbufferVertexShaderPath, "main", "vs_5_0");
    using var backbufferVertexShader = new VertexShader(device, backbufferCompiledVertexShader);
    using var backbufferInputLayout = new InputLayout(device, backbufferCompiledVertexShader, new[]
    {
        new InputLayoutDescriptor("Position", DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT),
        new InputLayoutDescriptor("Texture", DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT)
    });
    using var backbufferCompiledPixelShader = shaderCompiler.CompileShaderFromFile(backbufferPixelShaderPath, "main", "ps_5_0");
    using var backbufferPixelShader = new PixelShader(device, backbufferCompiledPixelShader);

    
    using var swapchain = new Swapchain(device, true);


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

        //var rotation = Quaternion.CreateFromYawPitchRoll(cameraRot.X, cameraRot.Y, 0);
        var rotation = Quaternion.CreateFromYawPitchRoll(3,0, 0);
        var modelRotation = Quaternion.CreateFromYawPitchRoll(modelRot.X, modelRot.Y, 0);
        var forward = Vector3.Transform(new Vector3(0, 0, 1f), rotation);
        var up = Vector3.Transform(new Vector3(0, 1, 0), rotation);
        position += Vector3.Transform(distance, rotation);
        var viewMatrix = Matrix4x4.CreateLookAt(position, position + forward, up);
        var viewProjectionMatrix = viewMatrix * projectionMatrix;
        //var viewProjectionMatrix = new Matrix4x4(-1, 0, 0, 0, 0, 1.77777779f, 0, 0, 0, 0, -1.00005f, -1, 0, 0, -0.5f, 0);
        var camera = new Camera
        {
            ViewMatrix = viewMatrix,
            ViewProjectMatrix = Matrix4x4.Transpose(viewProjectionMatrix)
        };
        using var cameraBuffer = new ConstantBuffer<Camera>(device, camera);

        var modelMatrix = Matrix4x4.Transpose(Matrix4x4.CreateScale(new Vector3(1, 1, 1)) *
                                              Matrix4x4.CreateFromQuaternion(modelRotation) *
                                              Matrix4x4.CreateTranslation(modelPosition));
        using var modelBuffer = new ConstantBuffer<Matrix4x4>(device, modelMatrix);

        #endregion
        {
            //// Render the texture to the backbuffer in a deferred context
            //deferredContext.SetViewport(new Viewport(window.Width, window.Height));
            //deferredContext.ClearRenderTargetView(backbuffer, new Color(1, 0, 0));
            //deferredContext.SetInputLayout(inputLayout);
            //deferredContext.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            //deferredContext.SetPixelShader(pixelShader);
            //deferredContext.SetVertexShader(vertexShader);
            //deferredContext.SetRenderTarget(backbuffer);
            //deferredContext.SetVertexBuffer(vertexBuffer2);
            //deferredContext.SetPixelShaderSampler(samplerState);
            //deferredContext.SetPixelShaderResource(tempTextureView);
            //deferredContext.SetIndexBuffer(indexBuffer);
            //deferredContext.DrawIndexed(6);
            //using var commandList = deferredContext.FinishCommandList();
}
        ////// Render to a texture
        device.ImmediateContextPtr->ClearDepthStencilView(depthStencilView.Ptr.Get(), 1, 1, 0);
        
        immediateContext.ClearRenderTargetView(gBufferAlbedoTarget, new Color(0.1f, 0.2f, 0.3f));
        immediateContext.ClearRenderTargetView(gBufferNormalsTarget, Color.Black);
        immediateContext.ClearRenderTargetView(gBufferDepthTarget, Color.White);
        //immediateContext.SetRenderTarget(backbuffer, depthStencilView);
        device.ImmediateContextPtr->OMSetRenderTargets(3, gBufferRenderTargets, depthStencilView.Ptr.Get());

        immediateContext.SetVertexBuffer(mesh.VertexBuffer);
        immediateContext.SetInputLayout(gBufferInputLayout);
        immediateContext.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
        immediateContext.SetPixelShader(gBufferPixelShader);
        immediateContext.SetVertexShader(gBuffferVertexShader);
        immediateContext.SetPixelShaderSampler(samplerState);
        immediateContext.SetPixelShaderResource(texture.ResourceView);
        immediateContext.SetPixelShaderResource(depthStencilView2, 1);
        immediateContext.SetIndexBuffer(mesh.IndexBuffer);

        immediateContext.SetVertexShaderConstantBuffer(cameraBuffer);
        immediateContext.SetVertexShaderConstantBuffer(modelBuffer, 1);

        immediateContext.DrawIndexed(mesh.IndexBuffer.NumberOfIndices, 0, 0);

        //immediateContext.ExecuteCommandList(commandList);
        // Render to the backbuffer
        immediateContext.ClearRenderTargetView(backbuffer, new Color(0.3f, 0, 0));
        immediateContext.SetRenderTarget(backbuffer);
        immediateContext.SetIndexBuffer(backbufferIndexBuffer);
        immediateContext.SetVertexBuffer(backbufferVertexBuffer);
        immediateContext.SetPixelShader(backbufferPixelShader);
        immediateContext.SetVertexShader(backbufferVertexShader);
        immediateContext.SetPixelShaderSampler(samplerState);

        //if (input.IsKeyDown(KeyCode.Space))
        //{
        //    immediateContext.SetPixelShaderResource(gBufferNormalsView);
        //}
        //else
        //{
        //    immediateContext.SetPixelShaderResource(gBufferAlbedoView);
        //}
        
        
        //immediateContext.SetPixelShaderResource(texture.ResourceView);

        immediateContext.SetInputLayout(backbufferInputLayout);
        immediateContext.SetPixelShaderResource(gBufferNormalsView);
        immediateContext.DrawIndexed(6, 0);
        immediateContext.SetPixelShaderResource(gBufferAlbedoView);
        immediateContext.DrawIndexed(6, 6);
        immediateContext.SetPixelShaderResource(gBufferDepthView);
        immediateContext.DrawIndexed(6, 12);

        //if (mesh.SubSets.Length > 1)
        //{
        //    for (var i = 0; i < mesh.SubSets.Length; ++i)
        //    {
        //        ref var subset = ref mesh.SubSets[i];
        //        //immediateContext.SetPixelShaderResource(_sponzaTextures[subset.MaterialIndex]);
        //        immediateContext.DrawIndexed((uint)subset.Count, (uint)subset.StartIndex, 0);
        //    }
        //}
        //else
        //{
        //    immediateContext.DrawIndexed(mesh.IndexBuffer.NumberOfIndices, 0, 0);
        //}

        swapchain.Present();

        //GC.Collect(); // Force garbage collection to see if we have any interop pointers that needs to be pinned.
    }

    //{
    //    using ComPtr<ID3D11Debug> d3dDebug = default;
    //    fixed (Guid* debugGuidPtr = &D3D11Common.D3D11Debug)
    //    {
    //        CheckAndThrow(device.Ptr->QueryInterface(debugGuidPtr, (void**)d3dDebug.GetAddressOf()), "QueryInterface");
    //    }
    //    CheckAndThrow(d3dDebug.Get()->ReportLiveDeviceObjects(D3D11_RLDO_FLAGS.D3D11_RLDO_DETAIL), "ReportLiveDeviceObjects");
    //}
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
