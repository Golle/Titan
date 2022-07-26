using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.D3D;
using Titan.Windows.D3D12;
using Titan.Windows.DXGI;
using static Titan.Windows.Common;
using static Titan.Windows.D3D12.D3D12Common;
using static Titan.Windows.DXGI.DXGICommon;
using static Titan.Windows.DXGI.DXGI_ADAPTER_FLAG;
using static Titan.Windows.DXGI.DXGI_GPU_PREFERENCE;

namespace Titan.Graphics.D3D12;


public unsafe struct D3D12Device
{
    private ComPtr<ID3D12Device> _instance;

    public static bool CreateAndInit(HWND windowHandle, uint width, uint height, bool debug, out D3D12Device device)
    {
        device = default;

        using ComPtr<IDXGIFactory7> dxgiFactory = default;
        var hr = CreateDXGIFactory1(typeof(IDXGIFactory7).GUID, (void**)dxgiFactory.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<ID3D12Device>($"Failed to create {nameof(IDXGIFactory7)} with HRESULT {hr}");
            return false;
        }

        using ComPtr<IDXGIAdapter1> adapter = default;
        if (!GetHardwareAdapter(dxgiFactory.Get(), adapter.GetAddressOf()))
        {
            Logger.Error("Failed to find a D3D12 compatible device.");
            return false;
        }

        if (FAILED(D3D12CreateDevice(adapter, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0, typeof(ID3D12Device).GUID, (void**)device._instance.GetAddressOf())))
        {
            Logger.Error<D3D12Device>($"Failed to create the {nameof(ID3D12Device)} with HRESULT: {hr}");
            return false;
        }


        return true;

        static bool GetHardwareAdapter(IDXGIFactory7* factory, IDXGIAdapter1** adapter)
        {
            DXGI_ADAPTER_DESC1 adapterDesc = default;
            for (var i = 0u; ; ++i)
            {
                ComPtr<IDXGIAdapter1> pAdapter = default;
                if (factory->EnumAdapterByGpuPreference(i, DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE, typeof(IDXGIAdapter1).GUID, (void**)pAdapter.ReleaseAndGetAddressOf()) == DXGI_ERROR.DXGI_ERROR_NOT_FOUND)
                {
                    // no more adapters
                    break;
                }

                if (SUCCEEDED(pAdapter.Get()->GetDesc1(&adapterDesc)))
                {
                    if ((adapterDesc.Flags & DXGI_ADAPTER_FLAG_SOFTWARE) != 0)
                    {
                        Logger.Trace<D3D12Device>("Found a Software adapter, ignorning.");
                        continue;
                    }

                    Logger.Trace<D3D12Device>($"Found a Hardware adapter ({adapterDesc.DescriptionString()}), trying to create a {nameof(ID3D12Device)}");
                    var hr = D3D12CreateDevice(pAdapter, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0, typeof(D3D12Device).GUID, null);
                    if (SUCCEEDED(hr))
                    {
                        *adapter = pAdapter.Get();
                        return true;
                    }
                    Logger.Error<D3D12Device>($"Failed to create a {nameof(ID3D12Device)} with HRESULT {hr}");
                }
            }

            return false;
        }
    }

    public void Release()
    {
        _instance.Release();
    }
}
