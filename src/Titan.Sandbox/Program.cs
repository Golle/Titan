using System;
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
using Titan.Windows.Win32.D3D11;

var pixelShaderPath = @"F:\Git\Titan\resources\shaders\SimplePixelShader.hlsl";
var vertexShaderPath = @"F:\Git\Titan\resources\shaders\SimpleVertexShader.hlsl";

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
    
    using var vertexBuffer = new VertexBuffer<Vertex>(device, new[]
    {
        new Vertex{Position = new Vector3(-1f,-1f, 0f)/2f, Color = Color.Blue, Texture = new Vector2(0,1)},
        new Vertex{Position = new Vector3(-1f, 1f, 0f)/2f, Color = Color.Green, Texture = new Vector2(0,0)},
        new Vertex{Position = new Vector3(1f, 1f, 0f)/2f, Color = Color.White, Texture = new Vector2(1,0)},
        new Vertex{Position = new Vector3(1f, -1f, 0f)/2f, Color = Color.Red, Texture = new Vector2(1,1)},
    });
    using var vertexBuffer2 = new VertexBuffer<Vertex>(device, new[]
    {
        new Vertex{Position = new Vector3(-1f,-1f, 0f)/3f, Color = Color.Blue, Texture = new Vector2(0,1)},
        new Vertex{Position = new Vector3(-1f, 1f, 0f)/3f, Color = Color.Green, Texture = new Vector2(0,0)},
        new Vertex{Position = new Vector3(1f, 1f, 0f)/3f, Color = Color.White, Texture = new Vector2(1,0)},
        new Vertex{Position = new Vector3(1f, -1f, 0f)/3f, Color = Color.Red, Texture = new Vector2(1,1)},
    });

    using var indexBuffer = new IndexBuffer<ushort>(device, new ushort[]{0,1,2,0,2,3});

    // Vertex Shader
    using var compiledVertexShader = shaderCompiler.CompileShaderFromFile(vertexShaderPath, "main", "vs_5_0");
    using var vertexShader = new VertexShader(device, compiledVertexShader);
    using var inputLayout = new InputLayout(device, compiledVertexShader, new[]
    {
        new InputLayoutDescriptor("Position", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT),
        new InputLayoutDescriptor("Texture", DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT),
        new InputLayoutDescriptor("Color", DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT)
    });


    // Pixel Shader
    using var compiledPixelShader = shaderCompiler.CompileShaderFromFile(pixelShaderPath, "main", "ps_5_0");
    using var pixelShader = new PixelShader(device, compiledPixelShader);
    
    using var samplerState = new SamplerState(device);

    using var immediateContext = new ImmediateContext(device);
    immediateContext.SetViewport(new Viewport(window.Width, window.Height));

    using var backbuffer = new BackBufferRenderTargetView(device);
    using var tempTexture = new Texture2D(device, (uint) window.Width, (uint) window.Height);
    using var tempTextureView = new ShaderResourceView(device, tempTexture);
    using var textureRenderTarget = new RenderTargetView(device, tempTexture);

    while (window.Update())
    {
        // Render to a texture
        immediateContext.ClearRenderTargetView(textureRenderTarget, new Color(0,1,1));
        
        immediateContext.SetRenderTarget(textureRenderTarget);
        immediateContext.SetVertexBuffer(vertexBuffer);
        immediateContext.SetInputLayout(inputLayout);
        immediateContext.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
        immediateContext.SetPixelShader(pixelShader);
        immediateContext.SetVertexShader(vertexShader);
        immediateContext.SetPixelShaderSampler(samplerState);
        immediateContext.SetPixelShaderResource(texture.ResourceView);
        immediateContext.SetIndexBuffer(indexBuffer);
        immediateContext.DrawIndexed(6);

        // Render the texture to the backbuffer
        immediateContext.ClearRenderTargetView(backbuffer, new Color(1, 0, 0));
        immediateContext.SetRenderTarget(backbuffer);
        immediateContext.SetVertexBuffer(vertexBuffer2);
        immediateContext.SetPixelShaderResource(tempTextureView);
        immediateContext.DrawIndexed(6);

        device.SwapChain.Get()->Present(1, 0);

        GC.Collect();
    }

    //{
    //    ID3D11Debug* d3dDebug;
    //    fixed (Guid* debugGuidPtr = &D3D11Debug)
    //    {
    //        var result = device->QueryInterface(debugGuidPtr, (void**)&d3dDebug);
    //        if (FAILED(result)) throw new InvalidOperationException($"Failed to query for debug interface: {result}");
    //    }

    //    {
    //        var result = d3dDebug->ReportLiveDeviceObjects(D3D11_RLDO_FLAGS.D3D11_RLDO_DETAIL);
    //        if (FAILED(result)) throw new InvalidOperationException($"Failed to ReportLiveDeviceObjects: {result}");
    //    }

    //    d3dDebug->Release();
    //}
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct Vertex
{
    public Vector3 Position;
    public Vector2 Texture;
    public Color Color;
}
