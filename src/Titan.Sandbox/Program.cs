using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Titan;
using Titan.Graphics.D3D11;
using Titan.Sandbox;
using Titan.Windows;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11Common;
using static Titan.Windows.Win32.D3D11.D3D_DRIVER_TYPE;
using static Titan.Windows.Win32.D3D11.D3D_FEATURE_LEVEL;


var pixelShaderPath = @"F:\Git\Titan\resources\shaders\SimplePixelShader.hlsl";
var vertexShaderPath = @"F:\Git\Titan\resources\shaders\SimpleVertexShader.hlsl";


using var window = Bootstrapper
    .Container
    .GetInstance<IWindowFactory>()
    .Create(1920, 1080, "Donkey box #2!");


{
    using var d3dDevice = new GraphicsDevice(window);

    using var vertexBuffer1 = new VertexBuffer<Vertex>(d3dDevice, numberOfVertices: 1000);
}


unsafe
{
    ID3D11Device * device;
    ID3D11DeviceContext * deviceContext;
    IDXGISwapChain* swapChain;
    DXGI_SWAP_CHAIN_DESC desc = default;
    desc.BufferCount = 1;
    desc.BufferDesc.Width = 1920;
    desc.BufferDesc.Height = 1080;
    desc.BufferDesc.Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM;
    desc.BufferDesc.RefreshRate.Denominator = 144;
    desc.BufferDesc.Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_UNSPECIFIED;
    desc.BufferDesc.ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED;
    
    desc.SampleDesc.Count = 1;
    desc.SampleDesc.Quality = 0;

    desc.Flags = 0;
    desc.BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT;
    desc.OutputWindow = window.Handle;
    desc.Windowed = true;
    desc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD;
    desc.Flags = DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;

    {
        var flags = 2u; // debug
        var featureLevel = D3D_FEATURE_LEVEL_11_1;
        var result = D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE_HARDWARE, 0, flags, &featureLevel, 1, D3D11_SDK_VERSION, &desc, &swapChain, &device, null, &deviceContext);
        if (FAILED(result)) throw new InvalidOperationException($"Failed to create the device with code: {result}");
    }

    ID3D11Buffer* backBuffer;
    fixed (Guid* resourcePointer = &D3D11Resource)
    {
        var result = swapChain->GetBuffer(0, resourcePointer, (void**)&backBuffer);
        if (FAILED(result)) throw new InvalidOperationException($"Failed to get backbuffer: {result}");
    }

    ID3D11RenderTargetView* renderTargetView;
    {
        var result = device->CreateRenderTargetView((ID3D11Resource*)backBuffer, null, &renderTargetView);
        if (FAILED(result)) throw new InvalidOperationException($"Failed to create render target view: {result}");
    }

    Console.WriteLine($"Release backbuffer:  {backBuffer->Release()}");

    var floats = stackalloc float[4];
    floats[0] = 1f;
    floats[1] = 0.4f;
    floats[2] = 0.1f;
    floats[3] = 1f;

    ID3D11Buffer * vertexBuffer;
    {
        var vertices = new []
        {
            new Vertex{Position = new Vector3(-1f,-1f, 0f), Color = Color.Blue},
            new Vertex{Position = new Vector3(0f, 1f, 0f), Color = Color.Green},
            new Vertex{Position = new Vector3(1f, -1f, 0f), Color = Color.Red},
        };

        D3D11_BUFFER_DESC bufferDesc = default;
        bufferDesc.BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER;
        bufferDesc.StructureByteStride = (uint)sizeof(Vertex);
        bufferDesc.Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT;
        bufferDesc.ByteWidth = (uint) (sizeof(Vertex) * vertices.Length);
        D3D11_SUBRESOURCE_DATA vertexData;
        fixed (void* verticesPointer = vertices)
        {
            vertexData.pSysMem = verticesPointer;
        }

        var result = device->CreateBuffer(&bufferDesc, &vertexData, &vertexBuffer);
        if (FAILED(result)) throw new InvalidOperationException($"Failed to create vertex buffer: {result}");
    }

    // Drawing a triangle doesn't require a index buffer. this is just a sample
    ID3D11Buffer* indexBuffer;
    {
        var indices = stackalloc int[3];
        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;
        D3D11_BUFFER_DESC bufferDesc = default;
        bufferDesc.BindFlags = D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER;
        bufferDesc.Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT;
        bufferDesc.StructureByteStride = sizeof(int);
        bufferDesc.ByteWidth = sizeof(int) * 6;
        D3D11_SUBRESOURCE_DATA indexData;
        indexData.pSysMem = indices;

        var result = device->CreateBuffer(&bufferDesc, &indexData, &indexBuffer);
        if (FAILED(result)) throw new InvalidOperationException($"Failed to create index buffer: {result}");
    }
    
    
    
    ID3D11VertexShader *vertexShader;
    ID3D11InputLayout* inputLayout;
    {
        ID3DBlob* vertexShaderBlob;
        ID3DBlob* errors = null;
        fixed (byte* entryPoint = "main".AsBytes())
        fixed (byte* target = "vs_5_0".AsBytes())
        fixed (char* vertexShaderBlobPath = vertexShaderPath)
        {
            var result = D3DCompileFromFile(vertexShaderBlobPath, null, null, (sbyte*)entryPoint, (sbyte*)target, 0, 0, &vertexShaderBlob, &errors);
            if (FAILED(result)) throw new Win32Exception(result, $"Failed to compile vertex shader: {result}");
        }
        {
            var result = device->CreateVertexShader(vertexShaderBlob->GetBufferPointer(), vertexShaderBlob->GetBufferSize(), null, &vertexShader);
            if (FAILED(result)) throw new Win32Exception(result, $"Failed to create vertex shader: {result}");
        }
        {
            var inputDescs = stackalloc D3D11_INPUT_ELEMENT_DESC[2];
            inputDescs[0].Format = DXGI_FORMAT.DXGI_FORMAT_R32G32B32_FLOAT;
            fixed (byte* semanticName = "Position".AsBytes())
            {
                inputDescs[0].SemanticName = (sbyte*)semanticName;
            }

            inputDescs[1].AlignedByteOffset = (uint)sizeof(Vector3);
            inputDescs[1].Format = DXGI_FORMAT.DXGI_FORMAT_R32G32B32A32_FLOAT;
            fixed (byte* semanticName = "Color".AsBytes())
            {
                inputDescs[1].SemanticName = (sbyte*)semanticName;
            }
            var result = device->CreateInputLayout(inputDescs, 2, vertexShaderBlob->GetBufferPointer(), vertexShaderBlob->GetBufferSize(), &inputLayout);
            if (FAILED(result)) throw new Win32Exception(result, $"Failed to create input layout: {result}");
        }
        vertexShaderBlob->Release();
    }

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
            var result = device->CreatePixelShader(pixelShaderBlob->GetBufferPointer(), pixelShaderBlob->GetBufferSize(), null, &pixelShader);
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

    deviceContext->RSSetViewports(1, &viewport);

    var drawIndexed = true;
    while (window.Update())
    {
        deviceContext->ClearRenderTargetView(renderTargetView, floats);
        deviceContext->OMSetRenderTargets(1, &renderTargetView, pDepthStencilView: null);
        deviceContext->IASetVertexBuffers(0, 1, &vertexBuffer, &stride, &offset);
        
        deviceContext->IASetInputLayout(inputLayout);
        deviceContext->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
        deviceContext->VSSetShader(vertexShader, null, 0u);
        deviceContext->PSSetShader(pixelShader, null, 0u);

        

        if (drawIndexed)
        {
            deviceContext->IASetIndexBuffer(indexBuffer, DXGI_FORMAT.DXGI_FORMAT_R32_UINT, 0);
            deviceContext->DrawIndexed(3, 0, 0);
        }
        else
        {
            deviceContext->Draw(3, 0);
        }
        
        
        swapChain->Present(1, 0);
        GC.Collect();
    }


    Console.WriteLine($"Release: {vertexBuffer->Release()}");
    Console.WriteLine($"Release: {vertexShader->Release()}");
    Console.WriteLine($"Release: {pixelShader->Release()}");
    Console.WriteLine($"Release: {indexBuffer->Release()}");
    Console.WriteLine($"Release: {inputLayout->Release()}");
    Console.WriteLine($"Release: {renderTargetView->Release()}");
    Console.WriteLine($"Release: {swapChain->Release()}");
    Console.WriteLine($"Release: {deviceContext->Release()}");
    

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
    Console.WriteLine($"Release: {device->Release()}");

}


[StructLayout(LayoutKind.Sequential, Size = 32)]
public unsafe struct Vertex
{
    public Vector3 Position;
    public Color Color;
}

public struct Color
{
    public float R;
    public float G;
    public float B;
    public float A;

    public Color(float r, float g, float b, float a = 1f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    public static readonly Color Red = new Color(1f,0, 0);
    public static readonly Color Green = new Color(0, 1f, 0);
    public static readonly Color Blue = new Color(0, 0, 1f);
    public static readonly Color White = new Color(1f, 1f, 1f);
    public static readonly Color Black = new Color(0f, 0, 0);
}



namespace Titan.Sandbox
{
    static class StringExtensions
    {
        public static byte[] AsBytes(this string s) => Encoding.ASCII.GetBytes(s);
    }
}
