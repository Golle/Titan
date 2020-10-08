using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
namespace Titan.D3D11
{
    public static unsafe class D3D11Common
    {
        public const int D3D11_SDK_VERSION = (7);
        public static readonly Guid D3D11Texture2D = new Guid("6f15aaf2-d208-4e89-9ab4-489535d34f9c");
        public static readonly Guid D3D11Resource = new Guid("dc8e63f3-d12b-4952-b47b-5e45026a862d");
        public static readonly Guid D3D11InfoQueue = new Guid("6543dbb6-1b48-42f5-ab82-e97ec74326f6");

        public static bool FAILED(in HRESULT result) => result.Value != 0;
        public static bool SUCCEEDED(in HRESULT result) => result.Value == 0;

        
        [DllImport("d3d11", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern HRESULT D3D11CreateDevice(
            IDXGIAdapter* pAdapter,
            D3D_DRIVER_TYPE driverType,
            HMODULE software,
            uint flags,
            D3D_FEATURE_LEVEL* pFeatureLevels,
            uint featureLevels,
            uint sdkVersion,
            ID3D11Device** ppDevice,
            D3D_FEATURE_LEVEL* pFeatureLevel,
            ID3D11DeviceContext** ppImmediateContext
        );
        
        [DllImport("d3d11", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern HRESULT D3D11CreateDeviceAndSwapChain(
            IDXGIAdapter* pAdapter,
            D3D_DRIVER_TYPE driverType,
            HMODULE software,
            uint flags,
            D3D_FEATURE_LEVEL* pFeatureLevels,
            uint featureLevels,
            uint sdkVersion,
            DXGI_SWAP_CHAIN_DESC* pSwapChainDesc,
            IDXGISwapChain **ppSwapChain,
            ID3D11Device** ppDevice,
            D3D_FEATURE_LEVEL *pFeatureLevel,
            ID3D11DeviceContext** ppImmediateContext
        );
    }
}
