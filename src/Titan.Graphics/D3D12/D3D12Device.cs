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
    private D3D_FEATURE_LEVEL _fatureLevel;

    public D3D_FEATURE_LEVEL FeatureLevel => _fatureLevel;

    public static bool CreateAndInit(HWND windowHandle, uint width, uint height, bool debug, out D3D12Device device)
    {
        device = default;
        if (debug)
        {
            EnableDebugLayer();
        }

        // Create the DXGI factory (Used to Query hardware devices)
        using ComPtr<IDXGIFactory7> dxgiFactory = default;
        var factoryFlags = debug ? DXGI_CREATE_FACTORY_FLAGS.DXGI_CREATE_FACTORY_DEBUG : 0;
        var hr = CreateDXGIFactory2(factoryFlags, typeof(IDXGIFactory7).GUID, (void**)dxgiFactory.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<ID3D12Device>($"Failed to create {nameof(IDXGIFactory7)} with HRESULT {hr}");
            return false;
        }

        // Get the Adapters on this machine
        using ComPtr<IDXGIAdapter1> adapter = default;
        if (!GetHardwareAdapter(dxgiFactory.Get(), adapter.GetAddressOf()))
        {
            Logger.Error("Failed to find a D3D12 compatible device.");
            return false;
        }

        // Create the D3D12 Device
        if (FAILED(D3D12CreateDevice(adapter, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0, typeof(ID3D12Device).GUID, (void**)device._instance.GetAddressOf())))
        {
            Logger.Error<D3D12Device>($"Failed to create the {nameof(ID3D12Device)} with HRESULT: {hr}");
            return false;
        }

        // Check format support (just for debugging)
        D3D12_FEATURE_DATA_FORMAT_SUPPORT formatSupport = default;
        hr = device._instance.Get()->CheckFeatureSupport(D3D12_FEATURE.D3D12_FEATURE_FORMAT_SUPPORT, &formatSupport, (uint)sizeof(D3D12_FEATURE_DATA_FORMAT_SUPPORT));
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to get the {D3D12_FEATURE.D3D12_FEATURE_FORMAT_SUPPORT} with HRESULT {hr}");
        }
        else
        {
            Logger.Trace<D3D12Device>($"{D3D12_FEATURE.D3D12_FEATURE_FORMAT_SUPPORT} - Format: {formatSupport.Format}. Support1: {formatSupport.Support1} Support2: {formatSupport.Support2}");
        }

        // Check the MaxFeatureLevel
        D3D12_FEATURE_DATA_FEATURE_LEVELS featureLevels = default;
        var levels = stackalloc D3D_FEATURE_LEVEL[4];
        levels[0] = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0;
        levels[1] = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1;
        levels[2] = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_0;
        levels[3] = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_12_1;
        featureLevels.NumFeatureLevels = 4;
        featureLevels.pFeatureLevelsRequested = levels;
        hr = device._instance.Get()->CheckFeatureSupport(D3D12_FEATURE.D3D12_FEATURE_FEATURE_LEVELS, &featureLevels, (uint)sizeof(D3D12_FEATURE_DATA_FEATURE_LEVELS));
        if (FAILED(hr))
        {
            // If this check fails, we don't know what shaders etc to load.
            Logger.Error<D3D12Device>($"Failed to get the feature levels with HRESULT {hr}");
            device._instance.Release();
            return false;
        }


        device._fatureLevel = featureLevels.MaxSupportedFeatureLevel;
        Logger.Trace<D3D12Device>($"{D3D12_FEATURE.D3D12_FEATURE_FEATURE_LEVELS} - MaxSupportedFeatureLevel: {featureLevels.MaxSupportedFeatureLevel}");
        
        return true;


        // Enumerate the Adapters and return the one with Highest Performance that supports D3D12
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

        static void EnableDebugLayer()
        {
            // Enable the Debug layer for D3D12
            using ComPtr<ID3D12Debug> spDebugController0 = default;
            using ComPtr<ID3D12Debug1> spDebugController1 = default;
            var hr = D3D12GetDebugInterface(typeof(ID3D12Debug).GUID, (void**)spDebugController0.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed {nameof(D3D12GetDebugInterface)} with HRESULT: {hr}");
                return;
            }

            hr = spDebugController0.Get()->QueryInterface(typeof(ID3D12Debug1).GUID, (void**)spDebugController1.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to query {nameof(ID3D12Debug1)} interface with HRESULT: {hr}");
                return;
            }
            spDebugController1.Get()->EnableDebugLayer();
            spDebugController1.Get()->SetEnableGPUBasedValidation(true);
        }
    }

    public void Release()
    {
        _instance.Release();
    }
}
