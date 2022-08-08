using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.D3D12;
using static Titan.Windows.Common;

namespace Titan.Graphics.D3D12Take2;

internal unsafe struct D3D12GraphicsQueue
{
    private ComPtr<ID3D12CommandQueue> _queue;
    private ComPtr<ID3D12Device4> _device;

    public readonly ID3D12CommandQueue* GetCommandQueue() => _queue.Get();




    public bool Initialize(ID3D12Device4* device)
    {
        var desc = new D3D12_COMMAND_QUEUE_DESC
        {
            Flags = D3D12_COMMAND_QUEUE_FLAGS.D3D12_COMMAND_QUEUE_FLAG_NONE,
            NodeMask = 0,
            Priority = 0,
            Type = D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT
        };

        var hr = device->CreateCommandQueue(&desc, typeof(ID3D12CommandQueue).GUID, (void**)_queue.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsQueue>($"Failed to create the {nameof(ID3D12CommandQueue)} with HRESULT {hr}");
            goto Error;
        }

        _device = new ComPtr<ID3D12Device4>(device);

        return true;

Error:
        Shutdown();
        return false;
    }

    public void Shutdown()
    {
        _queue.Release();
        _device.Release();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ID3D12CommandQueue*(in D3D12GraphicsQueue queue) => queue.GetCommandQueue();
}
