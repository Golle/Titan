using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D;
using Titan.Platform.Win32.DXGI;

// ReSharper disable InconsistentNaming
namespace Titan.Platform.Win32.D3D11;

public static unsafe class D3D11Common
{
    public const int D3D11_SDK_VERSION = (7);
    public static readonly Guid D3D11Texture2D = new("6f15aaf2-d208-4e89-9ab4-489535d34f9c");
    public static readonly Guid D3D11Resource = new("dc8e63f3-d12b-4952-b47b-5e45026a862d");
    public static readonly Guid D3D11InfoQueue = new("6543dbb6-1b48-42f5-ab82-e97ec74326f6");
    public static readonly Guid D3D11Debug = new("79cf2233-7536-4948-9d36-1e4692dc5760");
    public static readonly Guid DXGISurface1 = new("4AE63092-6327-4c1b-80AE-BFE12EA32B86");

    [DllImport("d3d11", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3D11CreateDevice(
        [In] IDXGIAdapter* pAdapter,
        [In] D3D_DRIVER_TYPE driverType,
        [In] HMODULE software,
        [In] D3D11_CREATE_DEVICE_FLAG flags,
        [In] D3D_FEATURE_LEVEL* pFeatureLevels,
        [In] uint featureLevels,
        [In] uint sdkVersion,
        [Out] ID3D11Device** ppDevice,
        [Out] D3D_FEATURE_LEVEL* pFeatureLevel,
        [Out] ID3D11DeviceContext** ppImmediateContext
    );

    [DllImport("Dxgi", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT CreateSwapChainForHwnd(
        [In] ID3D11Device* pDevice,
        [In] HWND hWnd,
        [In] DXGI_SWAP_CHAIN_DESC1* pDesc,
        [In] DXGI_SWAP_CHAIN_FULLSCREEN_DESC* pFullscreenDesc,
        [In] IDXGIOutput* pRestrictToOutput,
        [In] IDXGISwapChain1** ppSwapChain
    );
    [DllImport("d3d11", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    public static extern HRESULT D3D11CreateDeviceAndSwapChain(
        [In] IDXGIAdapter* pAdapter,
        [In] D3D_DRIVER_TYPE driverType,
        [In] HMODULE software,
        [In] D3D11_CREATE_DEVICE_FLAG flags,
        [In] D3D_FEATURE_LEVEL* pFeatureLevels,
        [In] uint featureLevels,
        [In] uint sdkVersion,
        [In] DXGI_SWAP_CHAIN_DESC* pSwapChainDesc,
        [Out] IDXGISwapChain** ppSwapChain,
        [Out] ID3D11Device** ppDevice,
        [Out] D3D_FEATURE_LEVEL* pFeatureLevel,
        [Out] ID3D11DeviceContext** ppImmediateContext
    );

}