using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D;
using Titan.Platform.Win32.D3D12;
using Titan.Platform.Win32.DXGI;
using static Titan.Platform.Win32.D3D12.D3D12Common;
using static Titan.Platform.Win32.DXGI.DXGI_CREATE_FACTORY_FLAGS;
using static Titan.Platform.Win32.DXGI.DXGI_GPU_PREFERENCE;
using static Titan.Platform.Win32.DXGI.DXGICommon;
using static Titan.Platform.Win32.Win32Common;

namespace Titan.Graphics.D3D12;

internal unsafe class DXGIAdapter : IGraphicsAdapter
{
    public string Name { get; private set; } = string.Empty;
    public IDXGIAdapter3* Adapter { get; private set; }
    public bool Init(bool debug, D3D_FEATURE_LEVEL minFeatureLevel)
    {
        Debug.Assert(Adapter == null);

        var flags = debug ? DXGI_CREATE_FACTORY_DEBUG : 0;
        IDXGIFactory7* factory;
        var hr = CreateDXGIFactory2(flags, typeof(IDXGIFactory7).GUID, (void**)&factory);
        if (FAILED(hr))
        {
            Logger.Error<DXGIAdapter>($"Failed to create a {nameof(IDXGIFactory7)} with {nameof(CreateDXGIFactory2)} with HRESULT: {hr}");
            return false;
        }


        var adapterIndex = 0u;
        while (true)
        {
            IDXGIAdapter3* adapter;
            hr = factory->EnumAdapterByGpuPreference(adapterIndex, DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE, typeof(IDXGIAdapter3).GUID, (void**)&adapter);
            if (hr == DXGI_ERROR.DXGI_ERROR_NOT_FOUND)
            {
                // no more adapters
                break;
            }

            DXGI_ADAPTER_DESC1 desc;
            if (SUCCEEDED(adapter->GetDesc1(&desc)))
            {
                if ((desc.Flags & DXGI_ADAPTER_FLAG.DXGI_ADAPTER_FLAG_SOFTWARE) != 0)
                {
                    Logger.Trace<DXGIAdapter>("Found a software device, ignoring.");
                }
                else
                {
                    Logger.Trace<DXGIAdapter>($"Found a Hardware adapter ({desc.DescriptionString()}), trying to create a {nameof(ID3D12Device)}");
                    hr = D3D12CreateDevice((IUnknown*)adapter, minFeatureLevel, typeof(ID3D12Device4).GUID, null);
                    if (SUCCEEDED(hr))
                    {
                        Adapter = adapter;
                        Name = new string(desc.DescriptionString());
                        Logger.Info<DXGIAdapter>($"Found a D3D12 compatible Device. {Name}");
                        factory->Release();
                        return true;
                    }
                    Logger.Error<DXGIAdapter>($"Failed to create the D3D12Device for adapter {desc.DescriptionString()}");
                }
            }
            adapter->Release();
        }
        factory->Release();
        Logger.Error<DXGIAdapter>("No compatible adapters were found");
        return false;
    }

    public void Shutdown()
    {
        if (Adapter != null)
        {
            Adapter->Release();
            Adapter = null;
        }
    }
}
