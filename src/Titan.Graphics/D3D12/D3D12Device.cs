using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
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
using System.IO;
using System.Runtime.InteropServices;
using Titan.Core.Memory;
using Titan.Windows.Win32;

namespace Titan.Graphics.D3D12;

public unsafe struct D3D12Device
{
    public ComPtr<ID3D12Device4> Device => _instance;
    public ComPtr<IDXGISwapChain3> SwapChain => _swapChain;

    private ComPtr<ID3D12Device4> _instance;

    private ComPtr<IDXGISwapChain3> _swapChain;
    private uint _frameIndex;
    private D3D_FEATURE_LEVEL _fatureLevel;


    private ComPtr<ID3D12CommandQueue> _commandQueue;
    private const int FrameCount = 2;
    private ComPtr<ID3D12CommandAllocator> _commandAllocator;
    private ComPtr<ID3D12DescriptorHeap> _descriptorHeap;
    private uint _descriptorHeapSize;
    private ComPtr<ID3D12RootSignature> _rootSignature;
    private ComPtr<ID3D12PipelineState> _pipelineState;
    private ComPtr<ID3D12GraphicsCommandList> _commandList;
    private ComPtr<ID3D12Resource> _vertexBuffer;
    D3D12_VERTEX_BUFFER_VIEW _vertexBufferView;

    public ComPtr<ID3D12Fence> _fence;
    private ulong _fenceValue;
    private fixed ulong _renderTargets[FrameCount];
    private fixed ulong _fenceValues[FrameCount];
    private fixed ulong _fences[FrameCount];
    private HANDLE _fenceEvent;
    private D3D12_VIEWPORT _viewPort;
    private D3D12_RECT _scissorRect;

    private ref ComPtr<ID3D12Resource> GetRenderTarget(int index) => ref *(ComPtr<ID3D12Resource>*)MemoryUtils.AsPointer(ref _renderTargets[index]);
    private ref ComPtr<ID3D12Fence> GetFence(int index) => ref *(ComPtr<ID3D12Fence>*)MemoryUtils.AsPointer(ref _fences[index]);
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
        using ComPtr<IDXGIAdapter3> adapter = default;
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

        DXGI_QUERY_VIDEO_MEMORY_INFO videoMem;
        hr = adapter.Get()->QueryVideoMemoryInfo(0, DXGI_MEMORY_SEGMENT_GROUP.DXGI_MEMORY_SEGMENT_GROUP_LOCAL, &videoMem);
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to query the LOCAL video memory with HRESULT {hr}");
        }
        else
        {
            Logger.Trace<D3D12Device>($"Local memory. Available: {videoMem.AvailableForReservation} Budget: {videoMem.Budget} Current reservation: {videoMem.CurrentReservation} Current usage: {videoMem.CurrentUsage}");
        }

        hr = adapter.Get()->QueryVideoMemoryInfo(0, DXGI_MEMORY_SEGMENT_GROUP.DXGI_MEMORY_SEGMENT_GROUP_NON_LOCAL, &videoMem);
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to query the NON_LOCAL video memory with HRESULT {hr}");
        }
        else
        {
            Logger.Trace<D3D12Device>($"Non Local memory. Available: {videoMem.AvailableForReservation} Budget: {videoMem.Budget} Current reservation: {videoMem.CurrentReservation} Current usage: {videoMem.CurrentUsage}");
        }


        // Create the D3D12 Device
        hr = D3D12CreateDevice(adapter, D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0, typeof(ID3D12Device4).GUID, (void**)device._instance.GetAddressOf());
        if (FAILED(hr))
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

        {

            D3D12_FEATURE_DATA_ARCHITECTURE1 arch = default;
            hr = device._instance.Get()->CheckFeatureSupport(D3D12_FEATURE.D3D12_FEATURE_ARCHITECTURE1, &arch, (uint)sizeof(D3D12_FEATURE_DATA_ARCHITECTURE1));
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to get the {D3D12_FEATURE.D3D12_FEATURE_ARCHITECTURE1} with HRESULT {hr}");
            }
            else
            {
                Logger.Trace<D3D12Device>($"NodeIndex: {arch.NodeIndex} TileBasedRenderer: {arch.TileBasedRenderer} UMA: {arch.UMA} CacheCoherentUMA: {arch.CacheCoherentUMA} IsolatedMMU: {arch.IsolatedMMU}");
            }
        }

        {

            for (var i = 1; i < 4; ++i)
            {
                D3D12_HEAP_PROPERTIES prop = default;
                var type = (D3D12_HEAP_TYPE)i;
                device._instance.Get()->GetCustomHeapProperties(0, type, &prop);
                Logger.Trace<D3D12Device>($"{type}: Type: {prop.Type}: Pool pref: {prop.MemoryPoolPreference} CPU page: {prop.CPUPageProperty}");
            }


        }

        uint allowTearing = 0;
        {

            hr = dxgiFactory.Get()->CheckFeatureSupport(DXGI_FEATURE.DXGI_FEATURE_PRESENT_ALLOW_TEARING, &allowTearing, sizeof(uint));
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to check feature support on {nameof(IDXGIFactory7)} with HRESULT {hr}");
            }
            else
            {
                Logger.Trace<D3D12Device>($"{DXGI_FEATURE.DXGI_FEATURE_PRESENT_ALLOW_TEARING}: {allowTearing != 0}");
            }
        }


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
        swapChainDesc.Flags = allowTearing != 0 ? DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING : 0;

        hr = dxgiFactory.Get()->CreateSwapChainForHwnd(device._commandQueue, windowHandle, &swapChainDesc, null, null, swapChain.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to create the {nameof(IDXGISwapChain1)} using {nameof(IDXGIFactory7.CreateSwapChainForHwnd)} with HRESULT {hr}");
            goto Error;
        }

        // Retarget the IDXGISwapChain1 to IDXGISwapChain3
        device._swapChain = new ComPtr<IDXGISwapChain3>((IDXGISwapChain3*)swapChain.Get());

        dxgiFactory.Release();
        return true;

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

        if (!device.InitBackbuffer())
        {
            Logger.Error<D3D12Device>("Backbuffer failed.. Sad face.");
            goto Error;
        }

        for (var i = 0; i < FrameCount; ++i)
        {
            hr = device._instance.Get()->CreateFence(0, D3D12_FENCE_FLAGS.D3D12_FENCE_FLAG_NONE, typeof(ID3D12Fence).GUID, (void**)device.GetFence(i).GetAddressOf());
        }

        // Create frame resources.
        hr = device._instance.Get()->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT, typeof(ID3D12CommandAllocator).GUID, (void**)device._commandAllocator.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to create the {nameof(ID3D12CommandAllocator)} with HRESULT {hr}");
            goto Error;
        }

        device.UpdateViewPort(width, height);


        return true;


// Error Handling goes here (log any debug info)
Error:
        Logger.Error<D3D12Device>($"{nameof(D3D12Device)} setup failed.");
        return false;

        // Enumerate the Adapters and return the one with Highest Performance that supports D3D12
        static bool GetHardwareAdapter(IDXGIFactory7* factory, IDXGIAdapter3** adapter)
        {
            DXGI_ADAPTER_DESC1 adapterDesc = default;
            for (var i = 0u; ; ++i)
            {
                ComPtr<IDXGIAdapter3> pAdapter = default;
                if (factory->EnumAdapterByGpuPreference(i, DXGI_GPU_PREFERENCE_HIGH_PERFORMANCE, typeof(IDXGIAdapter3).GUID, (void**)pAdapter.ReleaseAndGetAddressOf()) == DXGI_ERROR.DXGI_ERROR_NOT_FOUND)
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


    /// <summary>
    /// This is just to create a triangle and test the API
    /// </summary>
    public bool LoadAssets()
    {

        // Create an empty root signature.
        {
            D3D12_ROOT_SIGNATURE_DESC rootSignatureDesc = default;
            rootSignatureDesc.Flags = D3D12_ROOT_SIGNATURE_FLAGS.D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT;

            using ComPtr<ID3DBlob> signature = default;
            using ComPtr<ID3DBlob> error = default;
            var hr = D3D12SerializeRootSignature(&rootSignatureDesc, D3D_ROOT_SIGNATURE_VERSION.D3D_ROOT_SIGNATURE_VERSION_1, signature.GetAddressOf(), error.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to {nameof(D3D12SerializeRootSignature)} with HRESULT {hr}");
                goto Error;
            }


            hr = _instance.Get()->CreateRootSignature(0, signature.Get()->GetBufferPointer(), signature.Get()->GetBufferSize(), typeof(ID3D12RootSignature).GUID, (void**)_rootSignature.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to {nameof(ID3D12Device.CreateRootSignature)} with HRESULT {hr}");
                goto Error;
            }
        }

        {
            ComPtr<ID3DBlob> vertexShader = default;
            ComPtr<ID3DBlob> pixelShader = default;
            HRESULT hr;

#if DEBUG
            // Enable better shader debugging with the graphics debugging tools.
            D3D_COMPILE_FLAGS compileFlags = D3D_COMPILE_FLAGS.D3DCOMPILE_DEBUG | D3D_COMPILE_FLAGS.D3DCOMPILE_SKIP_OPTIMIZATION;
#else
            D3D_COMPILE_FLAGS compileFlags = 0;
#endif

            var vsMain = Encoding.UTF8.GetBytes("VSMain");
            var psMain = Encoding.UTF8.GetBytes("PSMain");
            var vs50 = Encoding.UTF8.GetBytes("vs_5_0");
            var ps50 = Encoding.UTF8.GetBytes("ps_5_0");

            //NOTE(Jens): change this path to test it out.
            const string shaderPath = @"F:\Git\Titan\samples\Titan.Sandbox\dx12sampleshader.hlsl";
            var path = File.Exists("dx12sampleshader.hlsl") ? "dx12sampleshader.hlsl" : shaderPath;


            fixed (byte* pVsMain = vsMain)
            fixed (byte* pPsMain = psMain)
            fixed (byte* pPs50 = ps50)
            fixed (byte* pVs50 = vs50)
            fixed (char* pShaderPath = path)
            {
                hr = D3DCompiler.D3DCompileFromFileNew(pShaderPath, null, null, pVsMain, pVs50, compileFlags, 0, vertexShader.GetAddressOf(), null);
                if (FAILED(hr))
                {
                    Logger.Error<D3D12Device>($"Failed to compile VertexShader from path {shaderPath} with HRESULT {hr}");
                    goto Error;
                }
                hr = D3DCompiler.D3DCompileFromFileNew(pShaderPath, null, null, pPsMain, pPs50, compileFlags, 0, pixelShader.GetAddressOf(), null);
                if (FAILED(hr))
                {
                    Logger.Error<D3D12Device>($"Failed to compile PixelShader from path {shaderPath} with HRESULT {hr}");
                    goto Error;
                }

            }


            var inputElementDescs = stackalloc D3D12_INPUT_ELEMENT_DESC[2];
            var position = Encoding.UTF8.GetBytes("POSITION");
            fixed (byte* pPosition = position)
            {
                inputElementDescs[0] = new D3D12_INPUT_ELEMENT_DESC
                {
                    SemanticName = pPosition,
                    Format = DXGI_FORMAT_R32G32B32_FLOAT,
                    SemanticIndex = 0,
                    InputSlotClass = D3D12_INPUT_CLASSIFICATION.D3D12_INPUT_CLASSIFICATION_PER_VERTEX_DATA,
                    AlignedByteOffset = 0
                };
            }

            var color = Encoding.UTF8.GetBytes("COLOR");
            fixed (byte* pColor = color)
            {
                inputElementDescs[1] = new D3D12_INPUT_ELEMENT_DESC
                {
                    SemanticName = pColor, //"COLOR",
                    Format = DXGI_FORMAT_R32G32B32A32_FLOAT,
                    SemanticIndex = 0,
                    InputSlotClass = D3D12_INPUT_CLASSIFICATION.D3D12_INPUT_CLASSIFICATION_PER_VERTEX_DATA,
                    AlignedByteOffset = 12
                };
            }


            // Describe and create the graphics pipeline state object (PSO).
            D3D12_GRAPHICS_PIPELINE_STATE_DESC psoDesc = default;
            psoDesc.InputLayout.pInputElementDescs = inputElementDescs;
            psoDesc.InputLayout.NumElements = 2;
            psoDesc.pRootSignature = _rootSignature.Get();
            psoDesc.VS = new D3D12_SHADER_BYTECODE
            {
                BytecodeLength = vertexShader.Get()->GetBufferSize(),
                pShaderBytecode = vertexShader.Get()->GetBufferPointer()
            };
            psoDesc.PS = new D3D12_SHADER_BYTECODE
            {
                BytecodeLength = pixelShader.Get()->GetBufferSize(),
                pShaderBytecode = pixelShader.Get()->GetBufferPointer()
            };
            psoDesc.RasterizerState = D3D12_RASTERIZER_DESC.Default();
            psoDesc.BlendState = D3D12_BLEND_DESC.Default();
            psoDesc.DepthStencilState.DepthEnable = 0;
            psoDesc.DepthStencilState.StencilEnable = 0;
            psoDesc.SampleMask = uint.MaxValue;
            psoDesc.PrimitiveTopologyType = D3D12_PRIMITIVE_TOPOLOGY_TYPE.D3D12_PRIMITIVE_TOPOLOGY_TYPE_TRIANGLE;
            psoDesc.NumRenderTargets = 1;
            psoDesc.RTVFormats[0] = DXGI_FORMAT_R8G8B8A8_UNORM;
            psoDesc.SampleDesc.Count = 1;
            hr = _instance.Get()->CreateGraphicsPipelineState(&psoDesc, typeof(ID3D12PipelineState).GUID, (void**)_pipelineState.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to the {nameof(ID3D12PipelineState)} with HRESULT {hr}");
                goto Error;
            }
        }
        {

            var hr = _instance.Get()->CreateCommandList1(0, D3D12_COMMAND_LIST_TYPE_DIRECT, D3D12_COMMAND_LIST_FLAGS.D3D12_COMMAND_LIST_FLAG_NONE, typeof(ID3D12GraphicsCommandList).GUID, (void**)_commandList.GetAddressOf());
            //var hr = _instance.Get()->CreateCommandList(0, D3D12_COMMAND_LIST_TYPE_DIRECT, _commandAllocator.Get(), _pipelineState.Get(), typeof(ID3D12GraphicsCommandList).GUID, (void**)_commandList.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to create the {nameof(ID3D12GraphicsCommandList)} with HRESULT {hr}");
                goto Error;
            }

            //hr = _commandList.Get()->Close();
            //if (FAILED(hr))
            //{
            //    Logger.Error<D3D12Device>($"Failed to Close the {nameof(ID3D12GraphicsCommandList)} with HRESULT {hr}");
            //    goto Error;
            //}

        }


        // Create the vertex buffer.
        {

            // Define the geometry for a triangle.
            var triangleVertices = stackalloc Vertex[3];
            var aspectRatio = 1f;// 1024f / 768f;

            triangleVertices[0] = new Vertex
            {
                Position = new(0, 0.25f * aspectRatio, 0),
                Color = Color.Red
            };
            triangleVertices[1] = new Vertex
            {
                Position = new(0.25f, -0.25f * aspectRatio, 0),
                Color = Color.Green
            };
            triangleVertices[2] = new Vertex
            {
                Position = new(-0.25f, -0.25f * aspectRatio, 0),
                Color = Color.Blue
            };
            var vertexBufferSize = sizeof(Vertex) * 3;




            // Note: using upload heaps to transfer static data like vert buffers is not 
            // recommended. Every time the GPU needs it, the upload heap will be marshalled 
            // over. Please read up on Default Heap usage. An upload heap is used here for 
            // code simplicity and because there are very few verts to actually transfer.

            // This is default Upload. maybe add helper methods for these?
            D3D12_HEAP_PROPERTIES heapProperties = new()
            {
                Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_UPLOAD,
                CreationNodeMask = 1,
                VisibleNodeMask = 1,
                CPUPageProperty = D3D12_CPU_PAGE_PROPERTY.D3D12_CPU_PAGE_PROPERTY_UNKNOWN,
                MemoryPoolPreference = D3D12_MEMORY_POOL.D3D12_MEMORY_POOL_UNKNOWN
            };

            D3D12_RESOURCE_DESC resourceDesc = new()
            {
                Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_BUFFER,
                Flags = D3D12_RESOURCE_FLAGS.D3D12_RESOURCE_FLAG_NONE,
                Width = (ulong)vertexBufferSize,
                Height = 1,
                Alignment = 0,
                DepthOrArraySize = 1,
                Format = DXGI_FORMAT_UNKNOWN,
                Layout = D3D12_TEXTURE_LAYOUT.D3D12_TEXTURE_LAYOUT_ROW_MAJOR,
                MipLevels = 1,
                SampleDesc = new DXGI_SAMPLE_DESC
                {
                    Count = 1,
                    Quality = 0
                }
            };
            var hr = _instance.Get()->CreateCommittedResource(
                &heapProperties,
                D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE,
                &resourceDesc,
                D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_GENERIC_READ,
                null,
                typeof(ID3D12Resource).GUID,
                (void**)_vertexBuffer.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to create the CommittedResource with HRESULT {hr}");
                goto Error;
            }

            //    // Copy the triangle data to the vertex buffer.
            byte* pVertexDataBegin = default;
            D3D12_RANGE readRange = default;
            hr = _vertexBuffer.Get()->Map(0, &readRange, (void**)&pVertexDataBegin);
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to Map vertex buffer with HRESULT {hr}");
                goto Error;
            }
            MemoryUtils.Copy(pVertexDataBegin, triangleVertices, sizeof(Vertex) * 3);
            _vertexBuffer.Get()->Unmap(0, null);
            // Initialize the vertex buffer view.
            _vertexBufferView.BufferLocation = _vertexBuffer.Get()->GetGPUVirtualAddress();
            _vertexBufferView.StrideInBytes = (uint)sizeof(Vertex);
            _vertexBufferView.SizeInBytes = (uint)vertexBufferSize;
            //// Create synchronization objects and wait until assets have been uploaded to the GPU.
            hr = _instance.Get()->CreateFence(0, D3D12_FENCE_FLAGS.D3D12_FENCE_FLAG_NONE, typeof(ID3D12Fence).GUID, (void**)_fence.GetAddressOf());
            _fenceValue = 1;

            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to Create {nameof(ID3D12Fence)} with HRESULT {hr}");
                goto Error;
            }
            //    // Create an event handle to use for frame synchronization.
            _fenceEvent = Kernel32.CreateEventA(null, 0, 0, null);
            if (_fenceEvent.Value == 0)
            {
                var lastError = Marshal.GetLastWin32Error();
                Logger.Error<D3D12Device>($"Failed to create an event handle with win32 error: {lastError} (0x{lastError:x8})");
                goto Error;
            }
            // Wait for the command list to execute; we are reusing the same command 
            // list in our main loop but for now, we just want to wait for setup to 
            // complete before continuing.
            WaitForPreviousFrame();
        }

        return true;
Error:

        return false;
    }

    public void Release()
    {
        _commandList.Release();
        _instance.Release();
        _commandQueue.Release();
        _descriptorHeap.Release();
        _swapChain.Release();

        _commandAllocator.Release();
        for (var i = 0; i < FrameCount; ++i)
        {
            GetRenderTarget(i).Release();
            GetFence(i).Release();
        }

        _pipelineState.Release();
        _rootSignature.Release();
        _vertexBuffer.Release();
        _fence.Release();

    }

    private bool InitBackbuffer()
    {
        D3D12_CPU_DESCRIPTOR_HANDLE rtvHandle;
        _descriptorHeap.Get()->GetCPUDescriptorHandleForHeapStart(&rtvHandle);
        // Create a RTV for each frame.
        for (var n = 0; n < FrameCount; n++)
        {
            ref var renderTarget = ref GetRenderTarget(n);
            var hr = _swapChain.Get()->GetBuffer((uint)n, typeof(ID3D12Resource).GUID, (void**)renderTarget.ReleaseAndGetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12Device>($"Failed to GetBuffer for render target {n} with HRESULT {hr}");
                return false;
            }
            _instance.Get()->CreateRenderTargetView(renderTarget.Get(), null, rtvHandle);
            rtvHandle.ptr += _descriptorHeapSize * 1;
        }
        return true;
    }

    private void WaitForPreviousFrame()
    {
        // WAITING FOR THE FRAME TO COMPLETE BEFORE CONTINUING IS NOT BEST PRACTICE.
        // This is code implemented as such for simplicity. The D3D12HelloFrameBuffering
        // sample illustrates how to use fences for efficient resource usage and to
        // maximize GPU utilization.

        // Signal and increment the fence value.
        ulong fence = _fenceValue;
        _commandQueue.Get()->Signal(_fence.Get(), fence);
        _fenceValue++;

        // Wait until the previous frame is finished.
        if (_fence.Get()->GetCompletedValue() < fence)
        {
            _fence.Get()->SetEventOnCompletion(fence, _fenceEvent);
            Kernel32.WaitForSingleObject(_fenceEvent, -1);
        }

        _frameIndex = _swapChain.Get()->GetCurrentBackBufferIndex();
    }

    public void Render()
    {
        // Record all the commands we need to render the scene into the command list.
        PopulateCommandList();

        // Execute the command list.
        _commandQueue.Get()->ExecuteCommandLists(1, (ID3D12CommandList**)_commandList.GetAddressOf());

        // Present the frame.
        _swapChain.Get()->Present(0, 0);

        WaitForPreviousFrame();
    }

    public bool Resize(uint width, uint height)
    {
        Logger.Warning<D3D12Device>("This is some really bad code that doens't even solve the problem :) we'll continue when the device, buffers etc have abstractions.");
        return true;
        UpdateViewPort(width, height);
        WaitForPreviousFrame();
        FlushGpu();
        _commandAllocator.Get()->Reset();
        _commandList.Get()->Reset(_commandAllocator.Get(), null);

        _commandList.Get()->Close();
        for (var i = 0; i < FrameCount; ++i)
        {
            GetRenderTarget(i).Release();
        }
        var hr = _swapChain.Get()->ResizeBuffers(FrameCount, width, height, DXGI_FORMAT_UNKNOWN, 0);
        if (FAILED(hr))
        {
            Logger.Error<ID3D12Device>("Failed to resize buffers on the swapchain");
            return false;
        }
        if (!InitBackbuffer())
        {
            Logger.Error<ID3D12Device>("Failed to recreate backbuffers.");
            return false;
        }
        _commandQueue.Get()->ExecuteCommandLists(1, (ID3D12CommandList**)_commandList.GetAddressOf());
        return true;
    }

    private void UpdateViewPort(uint width, uint height)
    {
        _viewPort = new D3D12_VIEWPORT
        {
            Width = width,
            Height = height,
            MaxDepth = 1,
            MinDepth = 0,
            TopLeftX = 0,
            TopLeftY = 0
        };
        _scissorRect = new D3D12_RECT
        {
            Left = 0,
            Right = (int)width,
            Top = 0,
            Bottom = (int)height
        };
    }

    private void PopulateCommandList()
    {
        HRESULT hr;
        // Command list allocators can only be reset when the associated 
        // command lists have finished execution on the GPU; apps should use 
        // fences to determine GPU execution progress.
        hr = _commandAllocator.Get()->Reset();

        // However, when ExecuteCommandList() is called on a particular command 
        // list, that command list can then be reset at any time and must be before 
        // re-recording.
        hr = _commandList.Get()->Reset(_commandAllocator.Get(), _pipelineState.Get());

        // Set necessary state.
        _commandList.Get()->SetGraphicsRootSignature(_rootSignature.Get());

        _commandList.Get()->RSSetViewports(1, (D3D12_VIEWPORT*)Unsafe.AsPointer(ref _viewPort));
        _commandList.Get()->RSSetScissorRects(1, (D3D12_RECT*)Unsafe.AsPointer(ref _scissorRect));

        // Indicate that the back buffer will be used as a render target.


        var barrier = new D3D12_RESOURCE_BARRIER
        {
            Transition = new D3D12_RESOURCE_TRANSITION_BARRIER
            {
                Subresource = uint.MaxValue,
                StateBefore = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PRESENT,
                StateAfter = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET,
                pResource = GetRenderTarget((int)_frameIndex).Get()
            },
            Flags = D3D12_RESOURCE_BARRIER_FLAGS.D3D12_RESOURCE_BARRIER_FLAG_NONE
        };
        _commandList.Get()->ResourceBarrier(1, &barrier);

        D3D12_CPU_DESCRIPTOR_HANDLE rtvHandle;
        _descriptorHeap.Get()->GetCPUDescriptorHandleForHeapStart(&rtvHandle);
        rtvHandle.ptr += _frameIndex * _descriptorHeapSize;
        _commandList.Get()->OMSetRenderTargets(1, &rtvHandle, 0, null);

        // Record commands.
        //const float clearColor[] = { 0.0f, 0.2f, 0.4f, 1.0f };
        var clearColor = new Color(0, 0.2f, 0.4f);
        _commandList.Get()->ClearRenderTargetView(rtvHandle, (float*)&clearColor, 0, null);
        _commandList.Get()->IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D10_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
        var vertexBufferView = _vertexBufferView;
        _commandList.Get()->IASetVertexBuffers(0, 1, &vertexBufferView);
        _commandList.Get()->DrawInstanced(3, 1, 0, 0);

        // Indicate that the back buffer will now be used to present.

        var barrier2 = new D3D12_RESOURCE_BARRIER
        {
            Transition = new D3D12_RESOURCE_TRANSITION_BARRIER
            {
                Subresource = uint.MaxValue,
                StateBefore = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET,
                StateAfter = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PRESENT,
                pResource = GetRenderTarget((int)_frameIndex).Get()
            },
            Flags = D3D12_RESOURCE_BARRIER_FLAGS.D3D12_RESOURCE_BARRIER_FLAG_NONE
        };
        _commandList.Get()->ResourceBarrier(1, &barrier2);

        _commandList.Get()->Close();
    }


    private void FlushGpu()
    {
        for (int i = 0; i < FrameCount; i++)
        {
            var fenceValue = ++_fenceValue;
            _commandQueue.Get()->Signal(_fence.Get(), fenceValue);
            if (_fence.Get()->GetCompletedValue() < fenceValue)
            {
                _fence.Get()->SetEventOnCompletion(fenceValue, _fenceEvent);
                Kernel32.WaitForSingleObject(_fenceEvent, -1);
            }
        }
        _frameIndex = 0;
    }
}


public struct Vertex
{
    public Vector3 Position;
    public Color Color;
}
