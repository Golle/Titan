using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D12;
using Titan.Graphics.Resources;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.Rendering;

internal unsafe class D3D12CommandQueue
{
    private IResourceManager _resourceManager;
    private IMemoryManager _memoryManager;
    private ComPtr<ID3D12CommandQueue> _commandQueue;
    private TitanArray<ComPtr<ID3D12CommandAllocator>> _commandAllocators;
    private TitanArray<ComPtr<ID3D12GraphicsCommandList4>> _commandLists;
    private volatile uint _next;

    private uint _bufferIndex;
    private uint _bufferCount;
    private uint _commandListCount;
    internal ID3D12CommandQueue* AsPointer() => _commandQueue;
    public bool Init(D3D12GraphicsDevice device, IMemoryManager memoryManager, IResourceManager resourceManager, uint maxCommandLists, uint bufferCount)
    {
        var totalCount = maxCommandLists * bufferCount;
        Logger.Trace<D3D12CommandQueue>($"Trying to allocate {totalCount} {nameof(ID3D12GraphicsCommandList4)} and {nameof(ID3D12CommandAllocator)}");

        //NOTE(Jens): If there's an error there will be a memory leak. Fix later.
        _commandQueue = device.CreateCommandQueue(D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT);
        if (_commandQueue.Get() == null)
        {
            Logger.Error<D3D12CommandQueue>("Failed to create the Direct CommandQueue");
            return false;
        }

        _commandLists = memoryManager.AllocArray<ComPtr<ID3D12GraphicsCommandList4>>(totalCount);
        if (!_commandLists.IsValid)
        {
            Logger.Error<D3D12CommandQueue>($"Failed to allocate memory for {totalCount} {nameof(ID3D12GraphicsCommandList4)}s");
            return false;
        }

        _commandAllocators = memoryManager.AllocArray<ComPtr<ID3D12CommandAllocator>>(totalCount);
        if (!_commandAllocators.IsValid)
        {
            Logger.Error<D3D12CommandQueue>($"Failed to allocate memory for {totalCount} {nameof(ID3D12CommandAllocator)}s");
            return false;
        }

        for (var i = 0; i < totalCount; ++i)
        {
            var commandList = device.CreateCommandList4(D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT);
            if (commandList == null)
            {
                Logger.Error<D3D12CommandQueue>($"Failed to create {nameof(ID3D12GraphicsCommandList4)} with index {i}");
                return false;
            }

            var allocator = device.CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT);
            if (allocator == null)
            {
                Logger.Error<D3D12CommandQueue>($"Failed to create {nameof(ID3D12CommandAllocator)} with index {i}");
                return false;
            }
            _commandLists[i] = commandList;
            _commandAllocators[i] = allocator;
        }

        Logger.Trace<D3D12CommandQueue>($"Initialized the {nameof(D3D12CommandQueue)} with a {nameof(ID3D12CommandQueue)} and {totalCount} {nameof(ID3D12GraphicsCommandList)}s.");
        _resourceManager = resourceManager;
        _memoryManager = memoryManager;
        _commandListCount = maxCommandLists;
        _bufferIndex = 0;
        _bufferCount = bufferCount;
        return true;
    }

    public D3D12CommandList GetCommandList()
    {
        var index = Interlocked.Increment(ref _next) - 1;
        //NOTE(Jens): fix this assert, its not accurate since we added buffer count
        //Debug.Assert(index < _commandLists.Length, $"Maximum command lists have been requested. ({_commandLists.Length})");
        var commandList = _commandLists[index].Get();
        var allocator = _commandAllocators[index].Get();
        Debug.Assert(commandList != null);
        Debug.Assert(allocator != null);
        return new D3D12CommandList(commandList, allocator, _resourceManager);
    }

    public void ExecuteAndReset()
    {
        var queue = _commandQueue.Get();
        var offset = _bufferIndex * _commandListCount;
        
        queue->ExecuteCommandLists(_next - offset, (ID3D12CommandList**)_commandLists.GetPointer(offset));

        _bufferIndex = (_bufferIndex + 1) % _bufferCount;
        _next = _bufferIndex * _commandListCount;
    }

    public void Shutdown()
    {
        _commandQueue.Dispose();
        foreach (var commandList in _commandLists.AsSpan())
        {
            commandList.Dispose();
        }

        foreach (var allocator in _commandAllocators.AsSpan())
        {
            allocator.Dispose();
        }
        _memoryManager.Free(ref _commandLists);
        _memoryManager.Free(ref _commandAllocators);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Signal(ID3D12Fence* fence, ulong fenceValue)
        => _commandQueue.Get()->Signal(fence, fenceValue);
}
