using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Titan;
using Titan.Core.Logging;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Sandbox;
using Titan.Windows;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.D3D11.D3D11Common;

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
    
    using var vertexBuffer = new VertexBuffer<Vertex>(device, new[]
    {
        new Vertex{Position = new Vector3(-1f,-1f, 0f), Color = Color.Blue},
        new Vertex{Position = new Vector3(0f, 1f, 0f), Color = Color.Green},
        new Vertex{Position = new Vector3(1f, -1f, 0f), Color = Color.Red},
    });

    var indices = stackalloc ushort[3];
    indices[0] = 0;
    indices[1] = 1;
    indices[2] = 2;

    using var indexBuffer = new IndexBuffer<ushort>(device, indices, 3);

    var floats = stackalloc float[4];
    floats[0] = 1f;
    floats[1] = 0.4f;
    floats[2] = 0.1f;
    floats[3] = 1f;

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
            var result = device.Ptr->CreateVertexShader(vertexShaderBlob->GetBufferPointer(), vertexShaderBlob->GetBufferSize(), null, &vertexShader);
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
            var result = device.Ptr->CreateInputLayout(inputDescs, 2, vertexShaderBlob->GetBufferPointer(), vertexShaderBlob->GetBufferSize(), &inputLayout);
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
    using var immediateContext = new ImmediateContext(device);
    while (window.Update())
    {
        deviceContext->ClearRenderTargetView(device.BackBuffer.Get(), floats);
        //deviceContext->IASetVertexBuffers(0, 1, vertexBuffer.Buffer.GetAddressOf(), &stride, &offset);
        immediateContext.SetVertexBuffer(vertexBuffer, 0);
        immediateContext.SetVertexBuffer(vertexBuffer, 1);
        //deviceContext->IASetVertexBuffers(0, 1, vertexBuffer.Buffer.GetAddressOf(), &stride, &offset);

        deviceContext->IASetInputLayout(inputLayout);
        deviceContext->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
        deviceContext->VSSetShader(vertexShader, null, 0u);
        deviceContext->PSSetShader(pixelShader, null, 0u);

        // Bind BackBuffer rendertarget before anything is drawn
        deviceContext->OMSetRenderTargets(1, device.BackBuffer.GetAddressOf(), pDepthStencilView: null);
        if (drawIndexed)
        {
            deviceContext->IASetIndexBuffer(indexBuffer.Buffer.Get(), indexBuffer.Format, 0);
            deviceContext->DrawIndexed(3, 0, 0);
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
    inputLayout->Release();
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


