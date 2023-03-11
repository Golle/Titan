#nullable disable
using System.Diagnostics;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;
using static Titan.Platform.Win32.D3D12.D3D12_COMMAND_LIST_TYPE;
using static Titan.Platform.Win32.Win32Common;

namespace Titan.Graphics.D3D12.Upload;

internal unsafe class D3D12UploadQueue
{
    private const uint MaxUploadFrames = 10;
    private TitanArray<UploadFrame> _frames;
    private ComPtr<ID3D12CommandQueue> _commandQueue;
    private ComPtr<ID3D12Fence> _fence;

    private IMemoryManager _memoryManager;
    private D3D12GraphicsDevice _device;

    private SpinLock _frameLock;
    private SpinLock _queueLock;
    
    private HANDLE _fenceEvent;
    private ulong _fenceValue;


    public bool Init(IMemoryManager memoryManager, D3D12GraphicsDevice device, uint uploadFrameCount)
    {
        Debug.Assert(uploadFrameCount > 0);
        uploadFrameCount = Math.Min(uploadFrameCount, MaxUploadFrames);

        Logger.Trace<D3D12UploadQueue>($"Creating {uploadFrameCount} {nameof(UploadFrame)}(s).");
        var frames = memoryManager.AllocArray<UploadFrame>(uploadFrameCount);
        if (!frames.IsValid)
        {
            Logger.Error<D3D12UploadQueue>($"Failed to allocate memory for the {nameof(UploadFrame)}s ()");
            return false;
        }

        using ComPtr<ID3D12CommandQueue> commandQueue = device.CreateCommandQueue(D3D12_COMMAND_LIST_TYPE_COPY);
        if (commandQueue.Get() == null)
        {
            Logger.Error<D3D12UploadQueue>($"Failed to create the {nameof(ID3D12CommandQueue)}");
            memoryManager.Free(ref frames);
            return false;
        }

        using ComPtr<ID3D12Fence> fence = device.CreateFence();
        if (fence.Get() == null)
        {
            Logger.Error($"Failed to create the {nameof(ID3D12Fence)}");
            memoryManager.Free(ref frames);
            return false;
        }
        
        // Create the command lists and allocators for each frame.
        for (var i = 0; i < frames.Length; ++i)
        {
            ref var frame = ref frames[i];
            var allocator = device.CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_COPY);
            if (allocator == null)
            {
                Logger.Error<D3D12UploadQueue>($"Failed to create the {nameof(ID3D12CommandAllocator)}[{i}]");
                goto Error;
            }

            frame.Allocator = allocator;
            fixed (char* pName = $"UploadFrame.Allocator[{i}]")
            {
                allocator->SetName(pName);
            }

            var commandList = device.CreateCommandList(D3D12_COMMAND_LIST_TYPE_COPY);
            if (commandList == null)
            {
                Logger.Error<D3D12UploadQueue>($"Failed to create a the {nameof(ID3D12GraphicsCommandList)}[{i}]");
                goto Error;
            }
            frame.CommandList = commandList;
            fixed (char* pName = $"UploadFrame.CommandList[{i}]")
            {
                commandList->SetName(pName);
            }
        }

        _frames = frames;
        _memoryManager = memoryManager;
        _device = device;
        _commandQueue = new ComPtr<ID3D12CommandQueue>(commandQueue);
        _fence = new ComPtr<ID3D12Fence>(fence);
        _fenceEvent = Kernel32.CreateEventA(null, 0, 0, null);

        Logger.Trace<D3D12UploadQueue>("Initialization completed");
        return true;

Error:
        Logger.Error<D3D12UploadQueue>("Something failed, releasing all resources.");
        memoryManager.Free(ref frames);
        for (var i = 0; i < frames.Length; ++i)
        {
            frames[i].Allocator.Dispose();
            frames[i].CommandList.Dispose();
        }
        return false;
    }

    public bool Upload(ID3D12Resource* destination, in TitanBuffer buffer)
    {
        Debug.Assert(destination != null && buffer.HasData());

        using ComPtr<ID3D12Resource> gpuBuffer = _device.CreateBuffer((uint)buffer.Length, true);
        if (gpuBuffer.Get() == null)
        {
            Logger.Error<D3D12UploadQueue>("Failed to create the temporary upload buffer.");
            return false;
        }

        var copyResult = CopyToBuffer(gpuBuffer, buffer);
        if (!copyResult)
        {
            Logger.Error<D3D12UploadQueue>("Failed to copy the resource.");
            return false;
        }

        var destinationFootprint = GetSubResourceFootprint(destination);
        D3D12_TEXTURE_COPY_LOCATION dst = new()
        {
            Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_SUBRESOURCE_INDEX,
            pResource = destination
        };

        D3D12_TEXTURE_COPY_LOCATION src = new()
        {
            PlacedFootprint = destinationFootprint,
            Type = D3D12_TEXTURE_COPY_TYPE.D3D12_TEXTURE_COPY_TYPE_PLACED_FOOTPRINT,
            pResource = gpuBuffer
        };

        //A  frame is a shared resource, make sure we've prepared everything before requesting one.
        
        var frame = GetAvailableFrame();
        var allocator = frame->Allocator.Get();
        var commandList = frame->CommandList.Get();
        var hr = allocator->Reset();
        if (FAILED(hr))
        {
            Logger.Error<D3D12UploadQueue>("Failed to reset the command allocator.");
            goto Error;
        }
        hr = commandList->Reset(allocator, null);
        if (FAILED(hr))
        {
            Logger.Error<D3D12UploadQueue>("Failed to reset the command list.");
            goto Error;
        }

        commandList->CopyTextureRegion(&dst, 0, 0, 0, &src, null);

        commandList->Close();
        {
            //NOTE(Jens): this might be a bottle neck if we upload a lot of resources. Can we do this in a nicer way?
            var gotLock = false;
            _queueLock.Enter(ref gotLock);

            var commandQueue = _commandQueue.Get();
            commandQueue->ExecuteCommandLists(1, (ID3D12CommandList**)&commandList);
            frame->FenceValue = ++_fenceValue;
            commandQueue->Signal(_fence, frame->FenceValue);

            frame->WaitAndReset(_fenceEvent, _fence);
            _queueLock.Exit();
        }

        return true;

Error:
        frame->State = UploadState.Available;
        return false;
    }

    private D3D12_PLACED_SUBRESOURCE_FOOTPRINT GetSubResourceFootprint(ID3D12Resource* resource)
    {
        D3D12_RESOURCE_DESC desc;
        resource->GetDesc(&desc);
        ulong totalSize, rowSize = 0ul;
        var rowCount = 0u;

        D3D12_PLACED_SUBRESOURCE_FOOTPRINT footPrint;
        _device.Device->GetCopyableFootprints(&desc, 0, 1, 0, &footPrint, &rowCount, &rowSize, &totalSize);
        return footPrint;
    }

    private static bool CopyToBuffer(ID3D12Resource* resource, in TitanBuffer data)
    {
        Debug.Assert(resource != null);
        void* ptr;
        var hr = resource->Map(0, null, &ptr);
        if (FAILED(hr))
        {
            Logger.Error<D3D12UploadQueue>("Failed to Map the temporary buffer");
            return false;
        }
        MemoryUtils.Copy(ptr, data.AsReadOnlySpan());
        resource->Unmap(0, null);
        return true;
    }

    private UploadFrame* GetAvailableFrame()
    {
        //NOTE(Jens): Should we add a timeout here?
        var wait = new SpinWait();
        while (true)
        {
            var gotLock = false;
            var index = -1;
            _frameLock.Enter(ref gotLock);
            for (var i = 0; i < _frames.Length; ++i)
            {
                if (_frames[i].State == UploadState.Available)
                {
                    _frames[i].State = UploadState.Busy;
                    index = i;
                    break;
                }
            }
            _frameLock.Exit();
            if (index != -1)
            {
                return _frames.GetPointer((uint)index);
            }
            wait.SpinOnce();
        }
    }

    public void Shutdown()
    {
        //NOTE(Jens): Implement flush before releasing
        if (_memoryManager != null)
        {
            foreach (ref var frame in _frames.AsSpan())
            {
                frame.Allocator.Dispose();
                frame.CommandList.Dispose();
            }
            _memoryManager.Free(ref _frames);
            _memoryManager = null;
            _commandQueue.Dispose();
            _fence.Dispose();
            Kernel32.CloseHandle(_fenceEvent);
        }
    }
}
