using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.D3D;
using Titan.Windows.D3D12;
using Titan.Windows.DXGI;
using static Titan.Windows.Common;
using static Titan.Windows.D3D12.D3D12_COMMAND_LIST_TYPE;
using static Titan.Windows.D3D12.D3D12_COMMAND_QUEUE_FLAGS;
using static Titan.Windows.D3D12.D3D12_DESCRIPTOR_HEAP_FLAGS;
using static Titan.Windows.D3D12.D3D12_DESCRIPTOR_HEAP_TYPE;
using static Titan.Windows.D3D12.D3D12Common;
using static Titan.Windows.DXGI.DXGICommon;
using static Titan.Windows.DXGI.DXGI_ADAPTER_FLAG;
using static Titan.Windows.DXGI.DXGI_FORMAT;
using static Titan.Windows.DXGI.DXGI_GPU_PREFERENCE;
using static Titan.Windows.DXGI.DXGI_MAKE_WINDOW_ASSOCIATION_FLAGS;
using static Titan.Windows.DXGI.DXGI_SWAP_EFFECT;
using static Titan.Windows.DXGI.DXGI_USAGE;

namespace Titan.Graphics.D3D12;

public unsafe struct D3D12Device
{
    private const int FrameCount = 2;
    private ComPtr<ID3D12Device> _instance;
    private ComPtr<ID3D12CommandQueue> _commandQueue;
    private ComPtr<IDXGISwapChain3> _swapChain;
    private uint _frameIndex;
    private D3D_FEATURE_LEVEL _fatureLevel;


    private ComPtr<ID3D12CommandAllocator> _commandAllocator;
    private ComPtr<ID3D12DescriptorHeap> _descriptorHeap;
    private uint _descriptorHeapSize;

    private fixed ulong _renderTargets[FrameCount];

    private ref ComPtr<ID3D12Resource> GetRenderTarget(int index) => ref *(ComPtr<ID3D12Resource>*)Unsafe.AsPointer(ref _renderTargets[index]);
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
        using ComPtr<IDXGIAdapter1> adapter = default;
        using ComPtr<IDXGISwapChain1> swapChain = default;

        var factoryFlags = debug ? DXGI_CREATE_FACTORY_FLAGS.DXGI_CREATE_FACTORY_DEBUG : 0;
        var hr = CreateDXGIFactory2(factoryFlags, typeof(IDXGIFactory7).GUID, (void**)dxgiFactory.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<ID3D12Device>($"Failed to create {nameof(IDXGIFactory7)} with HRESULT {hr}");
            goto Error;
        }

        // Get the Adapters on this machine
        if (!GetHardwareAdapter(dxgiFactory.Get(), adapter.GetAddressOf()))
        {
            Logger.Error("Failed to find a D3D12 compatible device.");
            goto Error;
        }

        // Create the D3D12 Device
        if (FAILED(D3D12CreateDevice(adapter, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0, typeof(ID3D12Device).GUID, (void**)device._instance.GetAddressOf())))
        {
            Logger.Error<D3D12Device>($"Failed to create the {nameof(ID3D12Device)} with HRESULT: {hr}");
            goto Error;
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
            goto Error;
        }
        device._fatureLevel = featureLevels.MaxSupportedFeatureLevel;
        Logger.Trace<D3D12Device>($"{D3D12_FEATURE.D3D12_FEATURE_FEATURE_LEVELS} - MaxSupportedFeatureLevel: {featureLevels.MaxSupportedFeatureLevel}");


        // Create the command queue that is needed to set up the swapchain
        D3D12_COMMAND_QUEUE_DESC desc = default;
        desc.Flags = D3D12_COMMAND_QUEUE_FLAG_NONE;
        desc.Type = D3D12_COMMAND_LIST_TYPE_DIRECT;
        hr = device._instance.Get()->CreateCommandQueue(&desc, typeof(ID3D12CommandQueue).GUID, (void**)device._commandQueue.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to create a {nameof(ID3D12CommandQueue)} with HRESULT {hr}");
            goto Error;
        }

        // Create the Swapchain
        DXGI_SWAP_CHAIN_DESC1 swapChainDesc = default;
        swapChainDesc.BufferCount = 2;
        swapChainDesc.Width = width;
        swapChainDesc.Height = height;
        swapChainDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
        swapChainDesc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
        swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT_FLIP_DISCARD;
        swapChainDesc.SampleDesc.Count = 1;

        hr = dxgiFactory.Get()->CreateSwapChainForHwnd(device._commandQueue, windowHandle, &swapChainDesc, null, null, swapChain.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to create the {nameof(IDXGISwapChain1)} using {nameof(IDXGIFactory7.CreateSwapChainForHwnd)} with HRESULT {hr}");
            goto Error;
        }

        // Retarget the IDXGISwapChain1 to IDXGISwapChain3
        device._swapChain = new ComPtr<IDXGISwapChain3>((IDXGISwapChain3*)swapChain.Get());
        device._frameIndex = device._swapChain.Get()->GetCurrentBackBufferIndex();

        hr = dxgiFactory.Get()->MakeWindowAssociation(windowHandle, DXGI_MWA_NO_ALT_ENTER);
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to disable Alt+Enter with HRESULT {hr}");
        }


        // This is just for testing purposes
        {
            // Describe and create a render target view (RTV) descriptor heap.
            D3D12_DESCRIPTOR_HEAP_DESC rtvHeapDesc = default;
            rtvHeapDesc.NumDescriptors = FrameCount;
            rtvHeapDesc.Type = D3D12_DESCRIPTOR_HEAP_TYPE_RTV;
            rtvHeapDesc.Flags = D3D12_DESCRIPTOR_HEAP_FLAG_NONE;
            hr = device._instance.Get()->CreateDescriptorHeap(&rtvHeapDesc, typeof(ID3D12DescriptorHeap).GUID, (void**)device._descriptorHeap.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to create the {nameof(ID3D12DescriptorHeap)} with HRESULT {hr}");
                goto Error;
            }
            device._descriptorHeapSize = device._instance.Get()->GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE_RTV);
        }

        // Create frame resources.
        {
            D3D12_CPU_DESCRIPTOR_HANDLE rtvHandle;
            device._descriptorHeap.Get()->GetCPUDescriptorHandleForHeapStart(&rtvHandle);
            // Create a RTV for each frame.
            for (var n = 0; n < FrameCount; n++)
            {
                ref var renderTarget = ref device.GetRenderTarget(n);
                hr = device._swapChain.Get()->GetBuffer((uint)n, typeof(ID3D12Resource).GUID, (void**)renderTarget.GetAddressOf());
                if (FAILED(hr))
                {
                    Logger.Error<D3D12Device>($"Failed to GetBuffer for render target {n} with HRESULT {hr}");
                    goto Error;
                }
                device._instance.Get()->CreateRenderTargetView(renderTarget.Get(), null, rtvHandle);
                rtvHandle.ptr += device._descriptorHeapSize * 1;
            }
        }

        hr = device._instance.Get()->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT, typeof(ID3D12CommandAllocator).GUID, (void**)device._commandAllocator.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to create the {nameof(ID3D12CommandAllocator)} with HRESULT {hr}");
            goto Error;
        }



        return true;


// Error Handling goes here (log any debug info)
Error:
        Logger.Error<D3D12Device>($"{nameof(D3D12Device)} setup failed.");
        return false;

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



    public readonly bool CreateCommandQueue(ref ComPtr<ID3D12CommandQueue> commandQueue)
    {
        commandQueue = default;
        D3D12_COMMAND_QUEUE_DESC desc = default;
        desc.Flags = D3D12_COMMAND_QUEUE_FLAG_NONE;
        desc.Type = D3D12_COMMAND_LIST_TYPE_DIRECT;

        var hr = _instance.Get()->CreateCommandQueue(&desc, typeof(ID3D12CommandQueue).GUID, (void**)commandQueue.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to create a {nameof(ID3D12CommandQueue)} with HRESULT {hr}");
            return false;
        }
        return true;
    }

    public void Release()
    {
        _instance.Release();
        _commandQueue.Release();
        _descriptorHeap.Release();
        _swapChain.Release();

        _commandAllocator.Release();
        for (var i = 0; i < FrameCount; ++i)
        {
            GetRenderTarget(i).Release();
        }

    }
}
