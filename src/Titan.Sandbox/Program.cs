using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Titan;
using Titan.Core.Logging;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.D3D11.State;
using Titan.Graphics.Textures;
using Titan.Sandbox;
using Titan.Windows;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11Common;
using static Titan.Windows.Win32.Common;

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

    using var factory = new ImagingFactory();

    //using var decoder = factory.CreateDecoderFromFilename(@"F:\Git\GameDev\resources\temp_models\sponza\textures\sponza_curtain_blue_diff.png");
    using var decoder = factory.CreateDecoderFromFilename(@"F:\Git\GameDev\resources\link.png");
    
    var buffer = Marshal.AllocHGlobal((int) decoder.ImageSize);
    decoder.CopyPixels(buffer, decoder.ImageSize);
    
    using var texture = new Texture2D(device, decoder.Width, decoder.Height, (byte*) buffer.ToPointer(), decoder.ImageSize);
    using var textureView = new ShaderResourceView(device, texture);

    Marshal.FreeHGlobal(buffer);

    
    using var vertexBuffer = new VertexBuffer<Vertex>(device, new[]
    {
        new Vertex{Position = new Vector3(-1f,-1f, 0f), Color = Color.Blue, Texture = new Vector2(0,1)},
        new Vertex{Position = new Vector3(-1f, 1f, 0f), Color = Color.Green, Texture = new Vector2(0,0)},
        new Vertex{Position = new Vector3(1f, 1f, 0f), Color = Color.White, Texture = new Vector2(1,0)},
        new Vertex{Position = new Vector3(1f, -1f, 0f), Color = Color.Red, Texture = new Vector2(1,1)},
        
    });

    var indices = stackalloc ushort[6];
    indices[0] = 0;
    indices[1] = 1;
    indices[2] = 2;
    indices[3] = 0;
    indices[4] = 2;
    indices[5] = 3;

    using var indexBuffer = new IndexBuffer<ushort>(device, indices, 6);

    
    
    ID3DBlob* vertexShaderBlob;
    ID3DBlob* errors1 = null;
    fixed (byte* entryPoint = "main".AsBytes())
    fixed (byte* target = "vs_5_0".AsBytes())
    fixed (char* vertexShaderBlobPath = vertexShaderPath)
    {
        CheckAndThrow(D3DCompileFromFile(vertexShaderBlobPath, null, null, (sbyte*)entryPoint, (sbyte*)target, 0, 0, &vertexShaderBlob, &errors1), "D3DCompileFromFile");
    }

    ID3D11VertexShader* vertexShader;
    CheckAndThrow(device.Ptr->CreateVertexShader(vertexShaderBlob->GetBufferPointer(), vertexShaderBlob->GetBufferSize(), null, &vertexShader), "CreateVertexShader");

    using var inputLayout = new InputLayout(device, vertexShaderBlob, new[]
    {
        new InputLayoutDescriptor("Position", DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT),
        new InputLayoutDescriptor("Texture", DXGI_FORMAT.DXGI_FORMAT_R32G32_FLOAT),
        new InputLayoutDescriptor("Color", DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT)
    });
    vertexShaderBlob->Release();

    ID3D11PixelShader* pixelShader;
    {
        ID3DBlob* pixelShaderBlob;
        ID3DBlob* errors = null;
        fixed (byte* entryPoint = "main".AsBytes())
        fixed (byte* target = "ps_5_0".AsBytes())
        fixed (char* pixelShaderBlobPath = pixelShaderPath)
        {
            var result = D3DCompileFromFile(pixelShaderBlobPath, null, null, (sbyte*)entryPoint, (sbyte*)target, 0, 0, &pixelShaderBlob, &errors);
            if (FAILED(result)) throw new Win32Exception(result, $"Failed to compile pixel shader: {result}");
        }
        {
            var result = device.Ptr->CreatePixelShader(pixelShaderBlob->GetBufferPointer(), pixelShaderBlob->GetBufferSize(), null, &pixelShader);
            if (FAILED(result)) throw new Win32Exception(result, $"Failed to create pixel shader: {result}");
        }
        pixelShaderBlob->Release();
    }


    var stride = (uint)sizeof(Vertex);
    var offset = 0u;

    var viewport = new D3D11_VIEWPORT
    {
        TopLeftX = 0,
        TopLeftY = 0,
        Width = window.Width,
        Height = window.Height,
        MinDepth = 0f,
        MaxDepth = 1f
    };

    var deviceContext = device.ImmediateContextPtr;
    deviceContext->RSSetViewports(1, &viewport);
    
    var drawIndexed = true;
    
    var floats = stackalloc float[4];
    floats[0] = 1f;
    floats[1] = 0.4f;
    floats[2] = 0.1f;
    floats[3] = 1f;

    var clearColor = Color.Red;
    using var samplerState = new SamplerState(device);

    using var immediateContext = new ImmediateContext(device);
    using var backbuffer = new BackBufferRenderTargetView(device);

    while (window.Update())
    {
        immediateContext.ClearRenderTargetView(backbuffer, clearColor);
        immediateContext.SetVertexBuffer(vertexBuffer);
        immediateContext.SetInputLayout(inputLayout);
        immediateContext.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

        deviceContext->VSSetShader(vertexShader, null, 0u);
        deviceContext->PSSetShader(pixelShader, null, 0u);
        deviceContext->PSSetSamplers(0, 1, samplerState.Ptr.GetAddressOf());
        deviceContext->PSSetShaderResources(0, 1, textureView.Ptr.GetAddressOf());
        // Bind BackBuffer rendertarget be
        // fore anything is drawn

        deviceContext->OMSetRenderTargets(1, device.BackBuffer.GetAddressOf(), pDepthStencilView: null);
        if (drawIndexed)
        {
            deviceContext->IASetIndexBuffer(indexBuffer.Buffer.Get(), indexBuffer.Format, 0);
            deviceContext->DrawIndexed(6, 0, 0);
        }
        else
        {
            deviceContext->Draw(3, 0);
        }
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
    pixelShader->Release();
    vertexShader->Release();
}


[StructLayout(LayoutKind.Sequential)]
public unsafe struct Vertex
{
    public Vector3 Position;
    public Vector2 Texture;
    public Color Color;
}

[StructLayout(LayoutKind.Sequential, Size = 16)]
struct TehConstants
{

}


namespace Titan.Sandbox
{
    static class StringExtensions
    {
        public static byte[] AsBytes(this string s) => Encoding.ASCII.GetBytes(s);
    }

    
    
}


