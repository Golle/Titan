using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Runtime.InteropServices;
using Titan;
using Titan.Core.Common;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.State;
using Titan.Graphics.Meshes;
using Titan.Graphics.Textures;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

var pixelShaderPath = @"F:\Git\Titan\resources\shaders\SimplePixelShader.hlsl";
var vertexShaderPath = @"F:\Git\Titan\resources\shaders\SimpleVertexShader.hlsl";
var instanceVertexShaderPath = @"F:\Git\Titan\resources\shaders\InstancedVertexShader.hlsl";

var meshVertexShaderPath = @"F:\Git\Titan\resources\shaders\MeshVertexShader.hlsl";
var meshPixelShaderPath = @"F:\Git\Titan\resources\shaders\MeshPixelShader.hlsl";
var simpleMesh = @"F:\Git\GameDev\resources\models\cube.dat";
//var simpleMesh = @"F:\Git\GameDev\resources\models\sponza.dat";

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

    using var mesh = meshLoader.LoadMesh(simpleMesh);
    //using var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\temp_models\door\print2.png");
    //using var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\blue.png");
    //using var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\temp_models\sponza\textures\spnza_bricks_a_diff.png");
    using var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\tree01.png");



    //using var testVertexBuffer = new VertexBuffer<Titan.Graphics.Meshes.Vertex>(device, new[]
    //{
    //    new Titan.Graphics.Meshes.Vertex{Position = new Vector3(-1f,-1f, 1f), Texture = new Vector2(0,1)},
    //    new Titan.Graphics.Meshes.Vertex{Position = new Vector3(-1f, 1f, 1f), Texture = new Vector2(0,0)},
    //    new Titan.Graphics.Meshes.Vertex{Position = new Vector3(1f, 1f, 1f),  Texture = new Vector2(1,0)},
    //    new Titan.Graphics.Meshes.Vertex{Position = new Vector3(1f, -1f, 1f), Texture = new Vector2(1,1)},
    //});


    //using var vertexBuffer = new VertexBuffer<Vertex>(device, new[]
    //{
    //    new Vertex{Position = new Vector3(-1f,-1f, 0f),  Texture = new Vector2(0,1)},
    //    new Vertex{Position = new Vector3(-1f, 1f, 0f),  Texture = new Vector2(0,0)},
    //    new Vertex{Position = new Vector3(1f, 1f, 0f), Texture = new Vector2(1,0)},
    //    new Vertex{Position = new Vector3(1f, -1f, 0f), Texture = new Vector2(1,1)},
    //});
    //using var vertexBuffer2 = new VertexBuffer<Vertex>(device, new[]
    //{
    //    new Vertex{Position = new Vector3(-1f,-1f, 0f)/3f, Color = Color.Blue, Texture = new Vector2(0,1)},
    //    new Vertex{Position = new Vector3(-1f, 1f, 0f)/3f, Color = Color.Green, Texture = new Vector2(0,0)},
    //    new Vertex{Position = new Vector3(1f, 1f, 0f)/3f, Color = Color.White, Texture = new Vector2(1,0)},
    //    new Vertex{Position = new Vector3(1f, -1f, 0f)/3f, Color = Color.Red, Texture = new Vector2(1,1)},
    //});

    //using var indexBuffer = new IndexBuffer<ushort>(device, new ushort[] { 0, 1, 2, 0, 2, 3 });
    //using var indexBuffer = new IndexBuffer<ushort>(device, new ushort[] { 0, 2, 1, 0, 3, 2 });

    //// Vertex Shader
    //using var compiledVertexShader = shaderCompiler.CompileShaderFromFile(vertexShaderPath, "main", "vs_5_0");
    //using var vertexShader = new VertexShader(device, compiledVertexShader);
    //using var inputLayout = new InputLayout(device, compiledVertexShader, new[]
    //{
    //    new InputLayoutDescriptor("Position", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT),
    //    new InputLayoutDescriptor("Texture", DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT),
    //    new InputLayoutDescriptor("Color", DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT)
    //});



    //using var compiledInstancedVertexShader = shaderCompiler.CompileShaderFromFile(instanceVertexShaderPath, "main", "vs_5_0");
    //using var instancedVertexShader = new VertexShader(device, compiledInstancedVertexShader);
    //using var instancedInputLayout = new InputLayout(device, compiledInstancedVertexShader, new[]
    //{
    //    new InputLayoutDescriptor("Position", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT),
    //    new InputLayoutDescriptor("Texture", DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT),
    //    new InputLayoutDescriptor("Color", DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT),
    //    new InputLayoutDescriptor("InstancePosition", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT, D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA)
    //});


    using var compiledMeshVertexShader = shaderCompiler.CompileShaderFromFile(meshVertexShaderPath, "main", "vs_5_0");
    using var meshVertexShader = new VertexShader(device, compiledMeshVertexShader);
    using var meshInputLayout = new InputLayout(device, compiledMeshVertexShader, new[]
    {
        new InputLayoutDescriptor("Position", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT),
        new InputLayoutDescriptor("Normal", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT),
        new InputLayoutDescriptor("Texture", DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT)
    });


    using var compiledMeshPixelShader = shaderCompiler.CompileShaderFromFile(meshPixelShaderPath, "main", "ps_5_0");
    using var meshPixelShader = new PixelShader(device, compiledMeshPixelShader);


    //var instanceVector = new Vector3[10000];
    //var random = new Random();
    //for (var i = 0; i < instanceVector.Length; ++i)
    //{
    //    instanceVector[i] = new Vector3(random.Next(-1000, 1000) / 1000f, random.Next(-1000, 1000) / 1000f, 0f);
    //}
    //using var instanceDataVertexBuffer = new VertexBuffer<Vector3>(device, instanceVector);

    // Pixel Shader
    //using var compiledPixelShader = shaderCompiler.CompileShaderFromFile(pixelShaderPath, "main", "ps_5_0");
    //using var pixelShader = new PixelShader(device, compiledPixelShader);

    using var samplerState = new SamplerState(device);
    using var immediateContext = new ImmediateContext(device);
    immediateContext.SetViewport(new Viewport(window.Width, window.Height));

    //using var deferredContext = new DeferredContext(device);
    //deferredContext.SetViewport(new Viewport(window.Width, window.Height));

    using var backbuffer = new BackBufferRenderTargetView(device);

    //using var tempTexture = new Texture2D(device, (uint)window.Width, (uint)window.Height, DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT, D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);
    //using var tempTextureView = new ShaderResourceView(device, tempTexture);
    //using var textureRenderTarget = new RenderTargetView(device, tempTexture);

    //using var depthStencilState = new DepthStencilState(device);
    //immediateContext.SetDepthStencilState(depthStencilState);

    //var depthStencilTexture = new Texture2D(device, (uint)window.Width, (uint)window.Height, DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT, D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL);
    //using var depthStencilView = new DepthStencilView(device, depthStencilTexture);

    ////using var blendState = new BlendState(device);
    ////immediateContext.SetBlendState(blendState);


    var modelPosition = new Vector3(0, 0, 0);
    

    var _rotation = new Vector2();

    var position = new Vector3(0, 0, -5);
    var projectionMatrix = MatrixExtensions.CreatePerspectiveLH(1f, window.Height / (float)window.Width, 0.5f, 10000f);
    while (window.Update())
    {
        _rotation.X += 0.01f;
        _rotation.Y += 0.02f;
        var rotation = Quaternion.CreateFromYawPitchRoll(3, 0, 0);
        var modelRotation = Quaternion.CreateFromYawPitchRoll(_rotation.X, _rotation.Y, 0);
        //var rotation = Quaternion.Identity;
        //rotation.W = 1f;
        //modelPosition.Z += 0.1f;
        //position.Z -= 1f;
        var forward = Vector3.Transform(new Vector3(0, 0, 1f), rotation);
        var up = Vector3.Transform(new Vector3(0, 1, 0), rotation);

        var viewMatrix = Matrix4x4.CreateLookAt(position, position + forward, up);
        var viewProjectionMatrix = viewMatrix * projectionMatrix;
        //var viewProjectionMatrix = new Matrix4x4(-1, 0, 0, 0, 0, 1.77777779f, 0, 0, 0, 0, -1.00005f, -1, 0, 0, -0.5f, 0);
        var camera = new Camera
        {
            ViewMatrix = viewMatrix,
            ViewProjectMatrix = Matrix4x4.Transpose(viewProjectionMatrix),
            WorldMatrix = 
            Matrix4x4.Transpose(Matrix4x4.CreateScale(new Vector3(1, 1, 1)) *
            Matrix4x4.CreateFromQuaternion(modelRotation) *
            Matrix4x4.CreateTranslation(modelPosition))
        //ViewProjectMatrix = Matrix4x4.Identity
        //ViewProjectMatrix = viewProjectionMatrix
    };
        using var cameraBuffer = new ConstantBuffer<Camera>(device, camera);

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

        ////// Render to a texture
        //immediateContext.SetViewport(new Viewport(window.Width, window.Height));
        //immediateContext.ClearRenderTargetView(textureRenderTarget, new Color(0, 1, 1));
        //immediateContext.SetRenderTarget(textureRenderTarget);
        //immediateContext.SetVertexBuffer(vertexBuffer);
        //immediateContext.SetInputLayout(inputLayout);
        //immediateContext.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
        //immediateContext.SetPixelShader(pixelShader);
        //immediateContext.SetVertexShader(vertexShader);
        //immediateContext.SetPixelShaderSampler(samplerState);
        //immediateContext.SetPixelShaderResource(texture.ResourceView);
        //immediateContext.SetIndexBuffer(indexBuffer);
        //immediateContext.DrawIndexed(6);

        //immediateContext.ExecuteCommandList(commandList);
        // Render to the backbuffer
        immediateContext.ClearRenderTargetView(backbuffer, new Color(0.3f, 0, 0));
        //immediateContext.SetRenderTarget(backbuffer, depthStencilView);
        immediateContext.SetRenderTarget(backbuffer);



        //immediateContext.SetVertexBuffer(instanceDataVertexBuffer, 1);

        //immediateContext.SetRenderTarget(backbuffer);
        immediateContext.SetPixelShader(meshPixelShader);
        //immediateContext.SetPixelShader(meshPixelShader);

        immediateContext.SetPixelShaderResource(texture.ResourceView);
        immediateContext.SetIndexBuffer(mesh.IndexBuffer);
        immediateContext.SetVertexBuffer(mesh.VertexBuffer);
        //immediateContext.SetIndexBuffer(indexBuffer);
        //immediateContext.SetVertexBuffer(vertexBuffer);

        immediateContext.SetInputLayout(meshInputLayout);
        //immediateContext.SetInputLayout(instancedInputLayout);
        immediateContext.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

        immediateContext.SetVertexShader(meshVertexShader);
        //immediateContext.SetVertexShader(instancedVertexShader);
        immediateContext.SetPixelShaderSampler(samplerState);
        immediateContext.SetVertexShaderConstantBuffer(cameraBuffer);
        //immediateContext.DrawIndexedInstanced(indexBuffer.NumberOfIndices, (uint) instanceVector.Length);
        //immediateContext.DrawIndexed(6);

        if (mesh.SubSets.Length > 1)
        {
            for (var i = 0; i < mesh.SubSets.Length; ++i)
            {
                ref var subset = ref mesh.SubSets[i];
                //immediateContext.SetPixelShaderResource(_sponzaTextures[subset.MaterialIndex]);
                immediateContext.DrawIndexed((uint)subset.Count, (uint)subset.StartIndex, 0);
            }
        }
        else
        {
            immediateContext.DrawIndexed(mesh.IndexBuffer.NumberOfIndices, 0, 0);
        }


        device.SwapChain.Get()->Present(1, 0);

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
}


[StructLayout(LayoutKind.Sequential)]
struct Camera
{
    //[FieldOffset(0)]
    public Matrix4x4 ViewMatrix;
    //[FieldOffset(64)]
    public Matrix4x4 ViewProjectMatrix;
    public Matrix4x4 WorldMatrix;
}
