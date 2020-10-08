using System;
using Titan;
using Titan.D3D11;
using Titan.Windows;

using static Titan.D3D11.D3D11Common;
using static Titan.D3D11.D3D_DRIVER_TYPE;
using static Titan.D3D11.D3D_FEATURE_LEVEL;

using var window = Bootstrapper
    .Container
    .GetInstance<IWindowFactory>()
    .Create(1920, 1080, "Donkey box #2!");


unsafe
{
    ID3D11Device * device;
    ID3D11DeviceContext * deviceContext;
    var featureLevel = D3D_FEATURE_LEVEL_11_1;
    var result = D3D11CreateDevice(null, D3D_DRIVER_TYPE_HARDWARE, 0, 0, &featureLevel,1, D3D11_SDK_VERSION, &device, null, &deviceContext);
    if (FAILED(result))
    {
        Console.WriteLine($"Failed to create the device with code: {result}");
    }
    else
    {
        Console.WriteLine("Omgz, SUCCESS!");
    }

    Console.WriteLine($"Release: {deviceContext->Release()}");
    Console.WriteLine($"Release: {device->Release()}");
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
    desc.OutputWindow = window.NativeHandle;
    desc.Windowed = true;
    desc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD;
    desc.Flags = DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;

    var featureLevel = D3D_FEATURE_LEVEL_11_1;
    var result = D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE_HARDWARE, 0, 0, &featureLevel, 1, D3D11_SDK_VERSION, &desc, &swapChain, &device, null, &deviceContext);
    if (FAILED(result))
    {
        Console.WriteLine($"Failed to create the device with code: {result}");
    }
    else
    {
        Console.WriteLine("Omgz, SUCCESS!");
    }

    Console.WriteLine($"Release: {swapChain->Release()}");
    Console.WriteLine($"Release: {deviceContext->Release()}");
    Console.WriteLine($"Release: {device->Release()}");
}


while (window.Update())
{
    GC.Collect();
}
