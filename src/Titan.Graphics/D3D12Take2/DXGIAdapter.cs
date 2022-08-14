using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.D3D;
using Titan.Windows.D3D12;
using Titan.Windows.DXGI;
using static Titan.Windows.Common;
using static Titan.Windows.D3D12.D3D12Common;
using static Titan.Windows.DXGI.DXGI_ADAPTER_FLAG;
using static Titan.Windows.DXGI.DXGI_GPU_PREFERENCE;

namespace Titan.Graphics.D3D12Take2;

internal unsafe struct DXGIAdapter
{
    private ComPtr<IDXGIAdapter3> _adapter;
    public bool Initialize(IDXGIFactory7* factory, D3D_FEATURE_LEVEL minFeatureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0)
    {

        DXGI_ADAPTER_DESC1 adapterDesc = default;
        for (var i = 0u; ; ++i)
        {
            using ComPtr<IDXGIAdapter3> queriedAdapter = default;
            if (factory->EnumAdapterByGpuPreference(i, DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE, typeof(IDXGIAdapter3).GUID, (void**)queriedAdapter.GetAddressOf()) == DXGI_ERROR.DXGI_ERROR_NOT_FOUND)
            {
                // no more adapters
                break;
            }

            if (SUCCEEDED(queriedAdapter.Get()->GetDesc1(&adapterDesc)))
            {
                if ((adapterDesc.Flags & DXGI_ADAPTER_FLAG_SOFTWARE) != 0)
                {
                    Logger.Trace<D3D12Device>("Found a Software adapter, ignorning.");
                    continue;
                }
                Logger.Trace<D3D12Device>($"Found a Hardware adapter ({adapterDesc.DescriptionString()}), trying to create a {nameof(ID3D12Device)}");
                var hr = D3D12CreateDevice(queriedAdapter, minFeatureLevel, typeof(D3D12Device).GUID, null);
                if (SUCCEEDED(hr))
                {
                    _adapter = new ComPtr<IDXGIAdapter3>(queriedAdapter);
                    return true;
                }
                Logger.Error<D3D12Device>($"Failed to create a {nameof(ID3D12Device)} with HRESULT {hr}");
            }
        }
        return false;
    }

    public static implicit operator IDXGIAdapter3*(in DXGIAdapter adapter) => adapter._adapter;

    public void Shutdown()
    {
        _adapter.Reset();
    }

    public readonly bool GetLocalMemoryInfo(out DXGI_QUERY_VIDEO_MEMORY_INFO memoryInfo)
    {
        Unsafe.SkipInit(out memoryInfo);
        fixed (DXGI_QUERY_VIDEO_MEMORY_INFO* pMemoryInfo = &memoryInfo)
        {
            return SUCCEEDED(_adapter.Get()->QueryVideoMemoryInfo(0, DXGI_MEMORY_SEGMENT_GROUP.DXGI_MEMORY_SEGMENT_GROUP_LOCAL, pMemoryInfo));
        }
    }

    public readonly bool GetNonLocalMemoryInfo(out DXGI_QUERY_VIDEO_MEMORY_INFO memoryInfo)
    {
        Unsafe.SkipInit(out memoryInfo);
        fixed (DXGI_QUERY_VIDEO_MEMORY_INFO* pMemoryInfo = &memoryInfo)
        {
            return SUCCEEDED(_adapter.Get()->QueryVideoMemoryInfo(0, DXGI_MEMORY_SEGMENT_GROUP.DXGI_MEMORY_SEGMENT_GROUP_NON_LOCAL, pMemoryInfo));
        }
    }
}
