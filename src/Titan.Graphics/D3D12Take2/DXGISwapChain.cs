using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.D3D12;
using Titan.Windows.DXGI;
using static Titan.Windows.Common;

namespace Titan.Graphics.D3D12Take2;

internal struct SwapChainCreationArgs
{
    public const uint DefaultBufferCount = 2;

    public required uint Width;
    public required uint Height;
    public required nint WindowHandle;
    public required uint BufferCount = DefaultBufferCount;
    public bool VSync;
    public bool AllowTearing;
    public SwapChainCreationArgs()
    {
    }
}
internal unsafe struct DXGISwapChain
{
    private ComPtr<IDXGISwapChain3> _swapChain;
    private ComPtr<ID3D12CommandQueue> _commandQueue;
    private uint _vsync;
    private uint _allowTearing;
    public bool Initialize(IDXGIFactory7* factory, ID3D12Device4* device, in SwapChainCreationArgs args)
    {
        var desc = new D3D12_COMMAND_QUEUE_DESC
        {
            Flags = D3D12_COMMAND_QUEUE_FLAGS.D3D12_COMMAND_QUEUE_FLAG_NONE,
            NodeMask = 0,
            Priority = 0,
            Type = D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT
        };

        var hr = device->CreateCommandQueue(&desc, typeof(ID3D12CommandQueue).GUID, (void**)_commandQueue.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<DXGISwapChain>($"Failed to create {nameof(ID3D12CommandQueue)} with HRESULT {hr}");
            goto Error;
        }

        DXGI_SWAP_CHAIN_DESC1 swapChainDesc = default;
        swapChainDesc.BufferCount = args.BufferCount;
        swapChainDesc.Width = args.Width;
        swapChainDesc.Height = args.Height;
        swapChainDesc.Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
        swapChainDesc.BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT;
        swapChainDesc.SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD;
        swapChainDesc.SampleDesc.Count = 1;
        swapChainDesc.SampleDesc.Quality = 0;
        swapChainDesc.Flags = args.AllowTearing ? DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_TEARING : 0;

        hr = factory->CreateSwapChainForHwnd((IUnknown*)_commandQueue.Get(), args.WindowHandle, &swapChainDesc,/*Add full screen config*/null, null, (IDXGISwapChain1**)_swapChain.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12Device>($"Failed to create the {nameof(IDXGISwapChain3)} using {nameof(IDXGIFactory7.CreateSwapChainForHwnd)} with HRESULT {hr}");
            goto Error;
        }


        _allowTearing = args.AllowTearing ? 1u : 0;
        _vsync = args.VSync ? 1u : 0;


        return true;


//release all resources
Error:
        Shutdown();
        return false;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Present()
    {
        _swapChain.Get()->Present(_vsync, _allowTearing);
    }

    public void Shutdown()
    {
        //NOTE(Jens): do we need to wait for a fence here?
        _commandQueue.Dispose();
        _swapChain.Release();
    }
}
