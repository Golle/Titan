using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using Titan;
using Titan.Core.Logging;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.Shaders;
using Titan.Graphics.D3D11.State;
using Titan.Graphics.Textures;
using Titan.Windows;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;

var pixelShaderPath = @"F:\Git\Titan\resources\shaders\SimplePixelShader.hlsl";
var vertexShaderPath = @"F:\Git\Titan\resources\shaders\SimpleVertexShader.hlsl";
var instanceVertexShaderPath = @"F:\Git\Titan\resources\shaders\InstancedVertexShader.hlsl";

var container = Bootstrapper.Container;

LOGGER.InitializeLogger(container.GetInstance<ILog>());

using var window = container
    .GetInstance<IWindowFactory>()
    .Create(1920, 1080, "Donkey box #2!");

unsafe
{
    using var device = (IGraphicsDevice)new GraphicsDevice(window);

    container.RegisterSingleton(device);

    var textureLoader = container.GetInstance<ITextureLoader>();
    var shaderCompiler = container.GetInstance<IShaderCompiler>();

    using var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\temp_models\door\print2.png");
    //using var texture = textureLoader.LoadTexture(@"F:\Git\GameDev\resources\blue.png");


    using var vertexBuffer = new VertexBuffer<Vertex>(device, new[]
    {
        new Vertex{Position = new Vector3(-1f,-1f, 0f)/20f, Color = Color.Blue, Texture = new Vector2(0,1)},
        new Vertex{Position = new Vector3(-1f, 1f, 0f)/20f, Color = Color.Green, Texture = new Vector2(0,0)},
        new Vertex{Position = new Vector3(1f, 1f, 0f)/20f, Color = Color.White, Texture = new Vector2(1,0)},
        new Vertex{Position = new Vector3(1f, -1f, 0f)/20f, Color = Color.Red, Texture = new Vector2(1,1)},
    });
    using var vertexBuffer2 = new VertexBuffer<Vertex>(device, new[]
    {
        new Vertex{Position = new Vector3(-1f,-1f, 0f)/3f, Color = Color.Blue, Texture = new Vector2(0,1)},
        new Vertex{Position = new Vector3(-1f, 1f, 0f)/3f, Color = Color.Green, Texture = new Vector2(0,0)},
        new Vertex{Position = new Vector3(1f, 1f, 0f)/3f, Color = Color.White, Texture = new Vector2(1,0)},
        new Vertex{Position = new Vector3(1f, -1f, 0f)/3f, Color = Color.Red, Texture = new Vector2(1,1)},
    });

    using var indexBuffer = new IndexBuffer<ushort>(device, new ushort[] { 0, 1, 2, 0, 2, 3 });

    // Vertex Shader
    using var compiledVertexShader = shaderCompiler.CompileShaderFromFile(vertexShaderPath, "main", "vs_5_0");
    using var vertexShader = new VertexShader(device, compiledVertexShader);
    using var inputLayout = new InputLayout(device, compiledVertexShader, new[]
    {
        new InputLayoutDescriptor("Position", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT),
        new InputLayoutDescriptor("Texture", DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT),
        new InputLayoutDescriptor("Color", DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT)
    });



    using var compiledInstancedVertexShader = shaderCompiler.CompileShaderFromFile(instanceVertexShaderPath, "main", "vs_5_0");
    using var instancedVertexShader = new VertexShader(device, compiledInstancedVertexShader);
    using var instancedInputLayout = new InputLayout(device, compiledInstancedVertexShader, new[]
    {
        new InputLayoutDescriptor("Position", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT),
        new InputLayoutDescriptor("Texture", DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT),
        new InputLayoutDescriptor("Color", DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT),
        new InputLayoutDescriptor("InstancePosition", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT, D3D11_INPUT_CLASSIFICATION.D3D11_INPUT_PER_INSTANCE_DATA)
    });


    var instanceVector = new Vector3[10000];
    var random = new Random();
    for (var i = 0; i < instanceVector.Length; ++i)
    {
        instanceVector[i] = new Vector3(random.Next(-1000, 1000) / 1000f, random.Next(-1000, 1000) / 1000f, 0f);
    }
    using var instanceDataVertexBuffer = new VertexBuffer<Vector3>(device, instanceVector);

    // Pixel Shader
    using var compiledPixelShader = shaderCompiler.CompileShaderFromFile(pixelShaderPath, "main", "ps_5_0");
    using var pixelShader = new PixelShader(device, compiledPixelShader);

    using var samplerState = new SamplerState(device);

    using var immediateContext = new ImmediateContext(device);
    immediateContext.SetViewport(new Viewport(window.Width, window.Height));

    using var deferredContext = new DeferredContext(device);
    deferredContext.SetViewport(new Viewport(window.Width, window.Height));

    using var backbuffer = new BackBufferRenderTargetView(device);
    using var tempTexture = new Texture2D(device, (uint)window.Width, (uint)window.Height);
    using var tempTextureView = new ShaderResourceView(device, tempTexture);
    using var textureRenderTarget = new RenderTargetView(device, tempTexture);


    while (window.Update())
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
        immediateContext.ClearRenderTargetView(backbuffer, new Color(1, 0, 0));
        immediateContext.SetRenderTarget(backbuffer);

        //immediateContext.SetVertexBuffer(instanceDataVertexBuffer, 1);

        immediateContext.SetRenderTarget(backbuffer);
        immediateContext.SetPixelShader(pixelShader);

        var ss = Stopwatch.StartNew();
        immediateContext.SetPixelShaderResource(texture.ResourceView);
        ss.Stop();
        immediateContext.SetIndexBuffer(indexBuffer);
        immediateContext.SetVertexBuffer(vertexBuffer);

        immediateContext.SetInputLayout(inputLayout);
        //immediateContext.SetInputLayout(instancedInputLayout);
        immediateContext.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

        immediateContext.SetVertexShader(vertexShader);
        //immediateContext.SetVertexShader(instancedVertexShader);
        immediateContext.SetPixelShaderSampler(samplerState);

        //immediateContext.DrawIndexedInstanced(6, (uint) instanceVector.Length);
        immediateContext.DrawIndexed(6);

        device.SwapChain.Get()->Present(0, 0);
    }

    {
        using ComPtr<ID3D11Debug> d3dDebug = default;
        fixed (Guid* debugGuidPtr = &D3D11Common.D3D11Debug)
        {
            CheckAndThrow(device.Ptr->QueryInterface(debugGuidPtr, (void**)d3dDebug.GetAddressOf()), "QueryInterface");
        }
        CheckAndThrow(d3dDebug.Get()->ReportLiveDeviceObjects(D3D11_RLDO_FLAGS.D3D11_RLDO_DETAIL), "ReportLiveDeviceObjects");
    }
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Vertex
{
    public Vector3 Position;
    public Vector2 Texture;
    public Color Color;
}
