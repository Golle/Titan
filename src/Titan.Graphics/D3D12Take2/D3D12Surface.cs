using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;
using Titan.Platform.Win32.DXGI;
using static Titan.Platform.Win32.Common;

namespace Titan.Graphics.D3D12Take2;

internal struct SwapChainCreationArgs
{
    public const uint DefaultBufferCount = 2;
    public const DXGI_FORMAT DefaultFormat = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;

    public required uint Width;
    public required uint Height;
    public required nint WindowHandle;
    public uint BufferCount = DefaultBufferCount;
    public DXGI_FORMAT Format = DefaultFormat;
    public bool VSync;
    public bool AllowTearing;

    public SwapChainCreationArgs()
    {
    }
}

internal unsafe struct D3D12Surface
{
    private ComPtr<IDXGISwapChain3> _swapChain;
    private GPUHeap _rtvHeap;

    private ComPtr<ID3D12Device4> _device; // needed for re-creation (could request it from the swapchain)

    private BackBuffers _backBuffers;
    private uint _vsync;
    private uint _allowTearing;

    private uint _bufferCount;
    private uint _frameIndex;

    private D3D12_VIEWPORT _viewPort;
    private D3D12_RECT _scissorRect;
    public uint FrameIndex => _frameIndex;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref D3D12Texture GetCurrentBackBuffer() => ref _backBuffers[_frameIndex];

    public bool Initialize(IDXGIFactory7* factory, ID3D12Device4* device, ID3D12CommandQueue* commandQueue, in SwapChainCreationArgs args)
    {
        Debug.Assert(args.BufferCount is 2 or 3, "only buffer counts 2 and 3 are allowed.");

        _allowTearing = args.AllowTearing ? 1u : 0;
        _vsync = args.VSync ? 1u : 0;
        _bufferCount = args.BufferCount;
        _device = new ComPtr<ID3D12Device4>(device);

        DXGI_SWAP_CHAIN_DESC1 swapChainDesc = default;
        swapChainDesc.BufferCount = _bufferCount;
        swapChainDesc.Width = args.Width;
        swapChainDesc.Height = args.Height;
        swapChainDesc.Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
        swapChainDesc.BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT;
        swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD;
        swapChainDesc.SampleDesc.Count = 1;
        swapChainDesc.SampleDesc.Quality = 0;
        swapChainDesc.Flags = args.AllowTearing ? DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING : 0;

        var hr = factory->CreateSwapChainForHwnd((IUnknown*)commandQueue, args.WindowHandle, &swapChainDesc,/*Add full screen config*/null, null, (IDXGISwapChain1**)_swapChain.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to create the {nameof(IDXGISwapChain3)} using {nameof(IDXGIFactory7.CreateSwapChainForHwnd)} with HRESULT {hr}");
            goto Error;
        }

        if (!GPUHeap.Create(device, D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_RTV, _bufferCount, false, out _rtvHeap))
        {
            Logger.Error<D3D12Surface>($"Failed to create the GPUHeap");
            goto Error;
        }

        _rtvHeap.SetName($"{nameof(D3D12Surface)} RTV Heap");

        Logger.Trace<D3D12Surface>($"SwapChain initialized with {_bufferCount} buffers and output format {args.Format}. vsync: {_vsync} allow tearing: {_allowTearing}");
        _frameIndex = _swapChain.Get()->GetCurrentBackBufferIndex();
        if (!InitBackBuffers(args.Width, args.Height, args.Format))
        {
            goto Error;
        }

        return true;

Error:
        Shutdown();
        return false;
    }



    private bool InitBackBuffers(uint width, uint height, DXGI_FORMAT format)
    {
        //NOTE(Jens): Should we replace this with a proper allocator or just keep this separated from dynamic resources? This is only used for backbuffers.
        for (var i = 0u; i < _bufferCount; ++i)
        {
            ref var texture = ref _backBuffers[i];
            var hr = _swapChain.Get()->GetBuffer(i, typeof(ID3D12Resource).GUID, (void**)texture.Resource.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12Surface>($"Failed to get backbuffer at index {i} with HRESULT {hr}");
                return false;
            }

            //NOTE(Jens): This handle is lost here, so we can't release it :O
            var handle = _rtvHeap.Allocate();
            texture.RTV.CpuHandle = handle.CpuHandle;
            // These value are only used for debugging
            texture.Width = width;
            texture.Height = height;
            texture.Format = format;
            // End debug values
            _device.Get()->CreateRenderTargetView(texture.Resource.Get(), null, texture.RTV);

        }
        Logger.Trace<D3D12Surface>($"Created {_bufferCount} backbuffers");


        _viewPort = new D3D12_VIEWPORT
        {
            TopLeftX = 0,
            TopLeftY = 0,
            MinDepth = 0.0f,
            Width = width,
            Height = height,
            MaxDepth = 1.0f
        };
        _scissorRect = new D3D12_RECT
        {
            Bottom = (int)height,
            Right = (int)width,
            Left = 0,
            Top = 0
        };
        return true;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Present()
    {
        _swapChain.Get()->Present(_vsync, _allowTearing);
        _frameIndex = (_frameIndex + 1) % _bufferCount;

        //Logger.Trace<DXGISwapChain>($"BackbufferIndex: {_swapChain.Get()->GetCurrentBackBufferIndex()} FrameIndex: {_frameIndex}");
    }

    public void Shutdown()
    {
        //NOTE(Jens): do we need to wait for a fence here?
        _swapChain.Reset();
        _device.Reset();
        _rtvHeap.Release();
        for (var i = 0u; i < _bufferCount; ++i)
        {
            _backBuffers[i].Resource.Reset();
        }

        this = default;
    }


    private struct BackBuffers
    {
        //NOTE(Jens): this will use more memory, but it's something we can live with for now :)
        // support max 3 buffers
        private D3D12Texture _resource1, _resource2, _resource3;

        public ref D3D12Texture this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var ptr = MemoryUtils.AsPointer(_resource1) + index;
                return ref MemoryUtils.ToRef(ptr);
            }
        }
    }
}
