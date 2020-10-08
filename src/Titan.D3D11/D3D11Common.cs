using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming
namespace Titan.D3D11
{
    public static unsafe class D3D11Common
    {
        public const int D3D11_SDK_VERSION = (7);

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
