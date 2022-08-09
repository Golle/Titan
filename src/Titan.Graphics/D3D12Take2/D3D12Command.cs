using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.D3D12;
using Titan.Windows.Win32;

namespace Titan.Graphics.D3D12Take2;

internal unsafe struct D3D12Command
{
    private CommandFramesFixedBuffer _commandFrames;
    private ComPtr<ID3D12Fence> _fence;
    private ComPtr<ID3D12GraphicsCommandList> _commandList; // only a single list at the moment
    private ComPtr<ID3D12CommandQueue> _commandQueue;
    private ulong _fenceValue;
    private uint _frameIndex;
    private uint _bufferCount;
    private HANDLE _fenceEvent;

    public bool Initialize(ID3D12Device4* device, ID3D12CommandQueue* queue, uint bufferCount)
    {
        Debug.Assert(bufferCount is 2 or 3, "BufferCount must be 2 or 3.");

        var hr = device->CreateFence(0, D3D12_FENCE_FLAGS.D3D12_FENCE_FLAG_NONE, typeof(ID3D12Fence).GUID, (void**)_fence.GetAddressOf());
        if (Common.FAILED(hr))
        {
            Logger.Error<D3D12Command>($"Failed to create {nameof(ID3D12Fence)} with HRESULT {hr}");
            goto Error;
        }

        hr = device->CreateCommandList1(0, D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT, D3D12_COMMAND_LIST_FLAGS.D3D12_COMMAND_LIST_FLAG_NONE, typeof(ID3D12GraphicsCommandList).GUID, (void**)_commandList.GetAddressOf());
        if (Common.FAILED(hr))
        {
            Logger.Error<D3D12Command>($"Failed to create {nameof(ID3D12GraphicsCommandList)} with HRESULT {hr}");
            goto Error;
        }

        for (var i = 0u; i < bufferCount; ++i)
        {
            hr = device->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT, typeof(ID3D12CommandAllocator).GUID, (void**)_commandFrames[i].Allocator.ReleaseAndGetAddressOf());
            if (Common.FAILED(hr))
            {
                Logger.Error<D3D12Command>($"Failed to create {nameof(ID3D12CommandAllocator)} #{i} with HRESULT {hr}");
                goto Error;
            }
        }

        _commandQueue = new ComPtr<ID3D12CommandQueue>(queue);
        _bufferCount = bufferCount;
        _fenceEvent = Kernel32.CreateEventExW(null, null, 0, AccessRights.EVENT_ALL_ACCESS);

        return true;

        Error:
        Logger.Error<D3D12Command>($"Failed to setup the {nameof(D3D12Command)}");
        return false;

    }

    public void BeginFrame()
    {
        ref var frame = ref _commandFrames[_frameIndex];
        frame.Wait(_fenceEvent, _fence.Get());
        frame.Allocator.Get()->Reset();
        _commandList.Get()->Reset(frame.Allocator, null);

    }

    public void EndFrame()
    {
        _commandList.Get()->Close();

        // just temporary to get it up and running.
        _commandQueue.Get()->ExecuteCommandLists(1, (ID3D12CommandList**)_commandList.GetAddressOf());
        _fenceValue++;
        _commandFrames[_frameIndex].FenceValue = _fenceValue;
        _commandQueue.Get()->Signal(_fence, _fenceValue);

        _frameIndex = (_frameIndex + 1) % _bufferCount;
    }


    public void Shutdown()
    {
        for (var i = 0u; i < _bufferCount; ++i)
        {
            _commandFrames[i].Release();
        }
        _commandList.Release();
        _commandQueue.Release();
        _fence.Release();
    }

    private struct CommandFrame
    {
        public ComPtr<ID3D12CommandAllocator> Allocator;
        public ulong FenceValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Wait(HANDLE fenceEvent, ID3D12Fence* fence)
        {
            if (fence->GetCompletedValue() < FenceValue)
            {
                fence->SetEventOnCompletion(FenceValue, fenceEvent);
                Kernel32.WaitForSingleObject(fenceEvent, -1);
            }
        }
        public void Release()
        {
            Allocator.Release();
        }
    }

    private struct CommandFramesFixedBuffer
    {
        private CommandFrame _frame1, _frame2, _frame3;
        public ref CommandFrame this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref *((CommandFrame*)Unsafe.AsPointer(ref _frame1) + index);
        }
    }
}
