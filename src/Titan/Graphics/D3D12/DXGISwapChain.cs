using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D12.Memory;
using Titan.Graphics.D3D12.Utils;
using Titan.Graphics.Rendering;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;
using Titan.Platform.Win32.DXGI;
using Titan.Windows;
using static Titan.Platform.Win32.DXGI.DXGI_CREATE_FACTORY_FLAGS;
using static Titan.Platform.Win32.DXGI.DXGICommon;
using static Titan.Platform.Win32.Win32Common;

namespace Titan.Graphics.D3D12;


internal unsafe class DXGISwapChain : ISwapChain
{
    private const DXGI_FORMAT DefaultFormat = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
    private BackBuffers _backBuffers;
    private ComPtr<IDXGISwapChain3> _swapChain;
    private D3D12CommandQueue _commandQueue;

    private uint _bufferCount;
    private uint _frameIndex;

    private uint _vsync;
    private bool _tearingSupport;
    private bool _fullscreen;


    private ComPtr<ID3D12Fence> _fence;
    private ulong _cpuFrame = 0;
    private ulong _gpuFrame = 0;
    private HANDLE _fenceEvent;

    private D3D12GraphicsDevice _device;
    private D3D12Allocator _allocator;



    public bool Init(D3D12GraphicsDevice device, D3D12Allocator allocator, D3D12CommandQueue commandQueue, IWindow window, GraphicsConfig config)
    {
        var bufferCount = config.TripleBuffering ? 3u : 2u;
        Logger.Trace<DXGISwapChain>($"Using {(config.TripleBuffering ? "TripleBuffering" : "Double Buffering")} with {bufferCount} frames.");

        var flags = config.Debug ? DXGI_CREATE_FACTORY_DEBUG : 0;
        using ComPtr<IDXGIFactory7> factory = default;
        var hr = CreateDXGIFactory2(flags, factory.UUID, (void**)factory.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<DXGISwapChain>($"Failed to Create the {nameof(IDXGIFactory7)} with HRESULT {hr}");
            return false;
        }

        var tearingSupport = false;
        if (config.AllowTearing)
        {
            var tearing = 0u;
            hr = factory.Get()->CheckFeatureSupport(DXGI_FEATURE.DXGI_FEATURE_PRESENT_ALLOW_TEARING, &tearing, sizeof(uint));
            if (FAILED(hr))
            {
                Logger.Error<DXGISwapChain>($"Failed to check for feature support {DXGI_FEATURE.DXGI_FEATURE_PRESENT_ALLOW_TEARING} with HRESULT {hr}. Tearing will be disabled.");
            }
            tearingSupport = tearing != 0;
        }
        var desc = new DXGI_SWAP_CHAIN_DESC1
        {
            BufferCount = bufferCount,
            Width = window.Width,
            Height = window.Height,
            Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
            BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT,
            SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD,
            SampleDesc =
            {
                Count = 1,
                Quality = 0
            },
            Flags = tearingSupport ? DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING : 0
        };


        //NOTE(Jens): This causes the fps to drop to ~20 in fullscreen
        var fullscreenDesc = new DXGI_SWAP_CHAIN_FULLSCREEN_DESC
        {
            RefreshRate =
            {
                //NOTE(Jens): figure out how to use this
                Denominator = 144,
                Numerator = 1
            },
            Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_CENTERED,
            ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED,
            Windowed = true
        };

        using ComPtr<IDXGISwapChain3> swapChain = default;
        hr = factory.Get()->CreateSwapChainForHwnd((IUnknown*)commandQueue.AsPointer(), window.Handle.Handle, &desc, null /*&fullscreenDesc*/, null, (IDXGISwapChain1**)&swapChain);
        if (FAILED(hr))
        {
            Logger.Error<DXGISwapChain>($"Failed to create the {nameof(IDXGISwapChain3)} with HRESULT {hr}");
            return false;
        }

        // Disable Alt-enter
        {
            hr = factory.Get()->MakeWindowAssociation(window.Handle.Handle, DXGI_MAKE_WINDOW_ASSOCIATION_FLAGS.DXGI_MWA_NO_ALT_ENTER);
            if (FAILED(hr))
            {
                Logger.Error<DXGISwapChain>("Failed to disable Alt+Enter when AllowTearing is enabled.");
            }
        }

        _fence = device.CreateFence();
        _swapChain = new ComPtr<IDXGISwapChain3>(swapChain);
        _commandQueue = commandQueue;
        _bufferCount = bufferCount;
        _tearingSupport = tearingSupport;
        _vsync = config.Vsync ? 1u : 0u;
        _frameIndex = swapChain.Get()->GetCurrentBackBufferIndex();
        _fenceEvent = Kernel32.CreateEventA(null, 0, 0, null);
        _device = device;
        _allocator = allocator;
        _fullscreen = config.Fullscreen;

        if (!InitBackbuffers(window.Width, window.Height))
        {
            Logger.Error<DXGISwapChain>("Failed to init the backbuffers.");
            Shutdown();
            return false;
        }

        return true;
    }

    public ref readonly D3D12Texture GetCurrentBackbuffer() => ref _backBuffers[_frameIndex].Texture;
    public uint GetBackbufferIndex() => _frameIndex;

    private bool InitBackbuffers(uint width, uint height, bool createDescriptor = true)
    {
        for (var i = 0u; i < _bufferCount; ++i)
        {
            ref var backbuffer = ref _backBuffers[i];
            ref var texture = ref backbuffer.Texture;
            var hr = _swapChain.Get()->GetBuffer(i, texture.Resource.UUID, (void**)texture.Resource.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<DXGISwapChain>($"Failed to get backbuffer at index {i} with HRESULT {hr}");
                return false;
            }

            if (createDescriptor)
            {
                Debug.Assert(!texture.RTV.IsValid);
                texture.RTV = _allocator.Allocate(DescriptorHeapType.RenderTargetView);
            }

            texture.Texture.Width = width;
            texture.Texture.Height = height;
            texture.Texture.Format = (TextureFormat)DefaultFormat;
            _device.Device->CreateRenderTargetView(texture.Resource.Get(), null, texture.RTV);
            D3D12Helpers.SetName(texture.Resource, $"Backbuffer_{i}");
        }
        return true;
    }

    public void Shutdown()
    {
        FlushGpu();
        for (var i = 0u; i < _bufferCount; ++i)
        {
            _allocator.Free(_backBuffers[i].Texture.RTV);
            _backBuffers[i].Release();
        }
        _swapChain.Dispose();
        _fence.Dispose();
        Kernel32.CloseHandle(_fenceEvent);
    }


    private void FlushGpu()
    {
        //NOTE(Jens): this method can be used for resizing the buffers as well. 
        for (var i = 0u; i < _bufferCount; ++i)
        {
            _cpuFrame++;
            _commandQueue.Signal(_fence, _cpuFrame);
            if (_fence.Get()->GetCompletedValue() < _cpuFrame)
            {
                _fence.Get()->SetEventOnCompletion(_cpuFrame, _fenceEvent);
                Kernel32.WaitForSingleObject(_fenceEvent, INFINITE);
            }
        }
        _frameIndex = _swapChain.Get()->GetCurrentBackBufferIndex();
    }

    public void Present()
    {
        _cpuFrame++;
        var flags = _tearingSupport && _vsync == 0 && !_fullscreen ? DXGI_PRESENT.DXGI_PRESENT_ALLOW_TEARING : 0;
        var hr = _swapChain.Get()->Present(_vsync, (uint)flags);
        _commandQueue.Signal(_fence, _cpuFrame);
        var diff = _cpuFrame - _gpuFrame;
        if (diff >= _bufferCount)
        {
            var waitFrame = _gpuFrame + 1;
            if (_fence.Get()->GetCompletedValue() < waitFrame)
            {
                _fence.Get()->SetEventOnCompletion(waitFrame, _fenceEvent);
                Kernel32.WaitForSingleObject(_fenceEvent, INFINITE);
            }
            _gpuFrame = _fence.Get()->GetCompletedValue();
        }
        Debug.Assert(SUCCEEDED(hr));
        _frameIndex = (_frameIndex + 1) % _bufferCount;
    }

    public void Resize(uint width, uint height)
    {
        FlushGpu();
        for (var i = 0u; i < _bufferCount; i++)
        {
            _backBuffers[i].Texture.Destroy();
        }

        DXGI_SWAP_CHAIN_DESC1 desc;
        var hr = _swapChain.Get()->GetDesc1(&desc);
        Debug.Assert(SUCCEEDED(hr));
        hr = _swapChain.Get()->ResizeBuffers(_bufferCount, width, height, desc.Format, (uint)desc.Flags);
        if (FAILED(hr))
        {
            Logger.Error<DXGISwapChain>($"Failed to resize the backbuffers with HRESULT {hr}. This will probably cause a crash.");
            return;
        }

        var fullscreen = false;
        hr = _swapChain.Get()->GetFullscreenState(&fullscreen, null);
        Debug.Assert(SUCCEEDED(hr));

        if (!InitBackbuffers(width, height, false))
        {
            Logger.Error<DXGISwapChain>("Failed to init the backbuffers.");
        }
        _frameIndex = _swapChain.Get()->GetCurrentBackBufferIndex();
    }

    public void ToggleFullscreen()
    {
        var swapchain = _swapChain.Get();

        var fullscreen = false;
        var hr = swapchain->GetFullscreenState(&fullscreen, null);
        Debug.Assert(SUCCEEDED(hr));
        hr = swapchain->SetFullscreenState(!fullscreen, null);
        Debug.Assert(SUCCEEDED(hr));
        
        _fullscreen = !fullscreen;
    }

    //NOTE(Jens): Workaround for non fixed size buffers
    private struct BackBuffers
    {
        //NOTE(Jens): this will use more memory, but it's something we can live with for now :)
        // support max 3 buffers
#pragma warning disable CS0169, CS0649
        private BackBuffer _resource1, _resource2, _resource3;
#pragma warning restore CS0169, CS0649

        public ref BackBuffer this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *(MemoryUtils.AsPointer(_resource1) + index);
        }
    }

    private struct BackBuffer
    {
        public D3D12Texture Texture;
        public void Release()
        {
            Texture.Resource.Dispose();
        }
    }
}
