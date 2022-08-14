using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Memory;
using Titan.Windows;
using Titan.Windows.D3D12;
using Titan.Windows.DXGI;
using Titan.Windows.Win32;
using static Titan.Windows.Common;

namespace Titan.Graphics.D3D12;

public unsafe struct D3D12RenderContext
{
    //private const int BufferCount = 2; // Double buffering, maybe we want to configure this?
    private const int BufferCount = 3; // Tripple buffering, maybe we want to configure this?
    private const int MaxCommandLists = 10;

    private ComPtr<IDXGISwapChain3> _swapChain;
    private ComPtr<ID3D12Device4> _device;
    private ComPtr<ID3D12CommandQueue> _commandQueue;
    private ComPtr<ID3D12GraphicsCommandList> _commandList;

    private ComPtr<ID3D12Fence> _fence;
    private ulong _fenceValue;
    private HANDLE _fenceEvent;


    private CommandFrame* _commandFrames; // This is the number of frames that we support.
    private int _maxCommandFrameCount;
    private int _frameIndex;

    private PlatformAllocator _allocator;



    public static bool CreateAndInit(in PlatformAllocator allocator, in D3D12Device device, out D3D12RenderContext context)
    {
        //NOTE(Jens): Should we use the same struct for Compute shaders? lets 
        var type = D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT;

        context = default;
        context._allocator = allocator;


        // copy the pointers and increase the ref count
        context._swapChain = ComPtr<IDXGISwapChain3>.Wrap(device.SwapChain);
        context._device = ComPtr<ID3D12Device4>.Wrap(device.Device);


        var devicePtr = context._device.Get();
        // Create the Command Queue
        {
            D3D12_COMMAND_QUEUE_DESC desc = default;
            desc.Flags = D3D12_COMMAND_QUEUE_FLAGS.D3D12_COMMAND_QUEUE_FLAG_NONE;
            desc.Type = type;
            var hr = devicePtr->CreateCommandQueue(&desc, typeof(ID3D12CommandQueue).GUID, (void**)context._commandQueue.GetAddressOf());
            if (FAILED(hr))
            {

                Logger.Error<D3D12RenderContext>($"Failed to create {nameof(ID3D12CommandQueue)} with HRESULT {hr}");
                goto Error;

            }
            fixed (char* pName = $"{nameof(D3D12RenderContext)}.{nameof(_commandQueue)}")
            {
                context._commandQueue.Get()->SetName(pName);
            }
        }

        // Construct command allocators
        context._commandFrames = allocator.Allocate<CommandFrame>(BufferCount);
        context._maxCommandFrameCount = BufferCount;
        for (var i = 0; i < context._maxCommandFrameCount; ++i)
        {
            ref var frame = ref context._commandFrames[i];
            var hr = devicePtr->CreateCommandAllocator(type, typeof(ID3D12CommandAllocator).GUID, (void**)frame.CommandAllocator.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12RenderContext>($"Failed to create {nameof(ID3D12CommandAllocator)} at frame {i} with HRESULT {hr}");

                goto Error;
            }
            fixed (char* pName = $"CommandAllocator for Frame {i}")
            {
                frame.CommandAllocator.Get()->SetName(pName);
            }
        }

        //NOTE(Jens): currently only a single command list
        {
            var hr = devicePtr->CreateCommandList1(0, type, D3D12_COMMAND_LIST_FLAGS.D3D12_COMMAND_LIST_FLAG_NONE, typeof(ID3D12GraphicsCommandList).GUID, (void**)context._commandList.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12RenderContext>($"Failed to create {nameof(ID3D12GraphicsCommandList)} with HRESULT {hr}");
                goto Error;
            }
        }

        {
            var hr = devicePtr->CreateFence(0, D3D12_FENCE_FLAGS.D3D12_FENCE_FLAG_NONE, typeof(ID3D12Fence).GUID, (void**)context._fence.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12RenderContext>($"Failed to create {nameof(ID3D12Fence)} with HRESULT {hr}");
                goto Error;
            }
            context._fenceEvent = Kernel32.CreateEventExA(null, null, 0, AccessRights.EVENT_ALL_ACCESS);
        }


        return true;


// In case of an error, release all resources and reset the context and return false.
Error:

        context.Release();
        context = default;

        return false;
    }

    public void BeginFrame()
    {
        // get current frame
        ref var frame = ref GetCurrentFrame();
        frame.Wait(_fenceEvent, _fence);

        ID3D12CommandAllocator* commandAllocator = frame.CommandAllocator;
        var hr = commandAllocator->Reset();
        Debug.Assert(SUCCEEDED(hr), $"Failed to reset the CommandAllocator with HRESULT {hr}");

        hr = _commandList.Get()->Reset(commandAllocator, null);
        Debug.Assert(SUCCEEDED(hr), $"Failed to reset the CommandList with HRESULT {hr}");
    }

    public void EndFrame()
    {
        ref var frame = ref GetCurrentFrame();

        var commandListCount = 1u; // hardcoded now
        var commandLists = stackalloc ID3D12CommandList*[1];
        for (var i = 0u; i < commandListCount; ++i)
        {
            _commandList.Get()->Close();
            commandLists[i] = (ID3D12CommandList*)_commandList.Get();
        }
        _commandQueue.Get()->ExecuteCommandLists(commandListCount, commandLists);

        frame.FenceValue = ++_fenceValue;
        var hr = _commandQueue.Get()->Signal(_fence, _fenceValue);
        Debug.Assert(SUCCEEDED(hr), $"Failed to Signal the fence with value {_fenceValue}");

        //Debug.Assert(SUCCEEDED(hr), $"Failed to reset the CommandAllocator with HRESULT {hr}");

        hr = _swapChain.Get()->Present(1, 0);
        Debug.Assert(SUCCEEDED(hr), $"Failed to present the frame with the SwapChain with HRESULT {hr}");


        _frameIndex = (_frameIndex + 1) % BufferCount;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ref CommandFrame GetCurrentFrame() => ref _commandFrames[_frameIndex];

    public void Release()
    {
        FlushGPU();
        _fence.Reset();
        _fenceValue = 0;
        if (_fenceEvent.Value != 0)
        {
            Kernel32.CloseHandle(_fenceEvent);
            _fenceEvent = default;
        }

        _commandQueue.Reset();
        _commandList.Reset();
        _swapChain.Reset();
        _device.Reset();

        for (var i = 0; i < _maxCommandFrameCount; ++i)
        {
            _commandFrames[i].Release();
        }

        if (_commandFrames != null)
        {
            _allocator.Free(_commandFrames);
            _commandFrames = null;
        }
    }


    private void FlushGPU()
    {
        for (var i = 0; i > _maxCommandFrameCount; ++i)
        {
            _commandFrames[i].Wait(_fenceEvent, _fence);
        }
        _frameIndex = 0;
    }

    private struct CommandFrame
    {
        public ComPtr<ID3D12CommandAllocator> CommandAllocator;
        public ulong FenceValue;

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
            CommandAllocator.Get()->Release();
        }
    }
}
