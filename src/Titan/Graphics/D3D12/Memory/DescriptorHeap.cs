using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;

namespace Titan.Graphics.D3D12.Memory;

internal unsafe struct DescriptorHeap
{
    private uint _handleOffset;
    private uint _numberOfDescriptors;
    private uint _numberOfTempDescriptors;
    private uint _frameIndex;
    private uint _frameCount;
    private volatile uint _tempOffset;
    private bool _shaderVisible;
    public ComPtr<ID3D12DescriptorHeap> _heap;
    private D3D12_CPU_DESCRIPTOR_HANDLE _cpuStart;
    private D3D12_GPU_DESCRIPTOR_HANDLE _gpuStart;
    private D3D12_CPU_DESCRIPTOR_HANDLE _tempCpuStart;
    private D3D12_GPU_DESCRIPTOR_HANDLE _tempGpuStart;

    private uint _incrementSize;
    private TitanArray<uint> _freeList;
    private uint _descriptorCount;

    private SpinLock _lock;

    private DescriptorHeapType _type;
    private IMemoryManager _memoryManager;

    public bool Init(D3D12GraphicsDevice device, IMemoryManager memoryManager, DescriptorHeapType type, uint count, uint temporaryCount, uint frameCount, bool shaderVisible)
    {
        Debug.Assert(_handleOffset == 0);

        _freeList = memoryManager.AllocArray<uint>(count);
        if (!_freeList.IsValid)
        {
            Logger.Error<DescriptorHeap>($"Failed to allocate {count * sizeof(uint)} bytes of memory for a free list.");
            return false;
        }

        var d3d12Type = type switch
        {
            DescriptorHeapType.RenderTargetView => D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_RTV,
            DescriptorHeapType.DepthStencilView => D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_DSV,
            DescriptorHeapType.ShaderResourceView => D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV,
            DescriptorHeapType.UnorderedAccessView => D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV,
            _ => throw new NotSupportedException($"The type {type} is not supported.")
        };

        var totalCount = count + temporaryCount * frameCount;
        _heap = device.CreateDescriptorHeap(d3d12Type, totalCount, shaderVisible);
        if (_heap.Get() == null)
        {
            Logger.Error<DescriptorHeap>($"Failed to init the {nameof(DescriptorHeap)} with {totalCount} descriptors.");
            memoryManager.Free(ref _freeList);
            return false;
        }

        D3D12_CPU_DESCRIPTOR_HANDLE cpuStart;
        _cpuStart = *_heap.Get()->GetCPUDescriptorHandleForHeapStart(&cpuStart);
        if (shaderVisible)
        {
            D3D12_GPU_DESCRIPTOR_HANDLE gpuStart;
            _gpuStart = *_heap.Get()->GetGPUDescriptorHandleForHeapStart(&gpuStart);
        }

        _incrementSize = device.Device->GetDescriptorHandleIncrementSize(d3d12Type);
        _memoryManager = memoryManager;
        _type = type;

        _handleOffset = (uint)Random.Shared.Next(1000, 6_000_000);
        _numberOfDescriptors = count;
        _numberOfTempDescriptors = temporaryCount;
        _frameCount = frameCount;
        _shaderVisible = shaderVisible;
        _lock = new SpinLock();

        //NOTE(Jens): extract temp allocations to its own struct. This is messy
        var tempStart = _incrementSize * _numberOfDescriptors;
        _tempGpuStart = _gpuStart.ptr + tempStart;
        _tempCpuStart = _cpuStart.ptr + tempStart;

        // store the offsets in the freelist
        for (var i = 0u; i < count; ++i)
        {
            _freeList[i] = i * _incrementSize;
        }
        return true;
    }

    public DescriptorHandle Allocate()
    {
        var lockToken = false;
        _lock.Enter(ref lockToken);
        var offset = _freeList[_descriptorCount++];
        _lock.Exit();

        var cpuHandle = _cpuStart;
        cpuHandle.ptr += offset;
        var gpuHandle = _gpuStart;
        //NOTE(Jens): maybe this sohuld be 0 and select a "placeholder" ?
        var index = 0ul; 
        if (_shaderVisible)
        {
            gpuHandle.ptr += offset;
            index = (gpuHandle.ptr - _gpuStart.ptr) / _incrementSize;
        }

        return new DescriptorHandle(_type, offset + _handleOffset, cpuHandle, gpuHandle, index);
    }

    public void Free(in DescriptorHandle handle)
    {
        Debug.Assert(handle.Type == _type);
        Debug.Assert(handle.IsValid);
        Debug.Assert(handle.Offset >= _handleOffset && handle.Offset < _handleOffset + _numberOfDescriptors, $"The handle beeing free is not a part of this {nameof(DescriptorHeap)}");
        var offset = handle.Offset - _handleOffset;
#if DEBUG
        CheckForDuplicateOffsets(_freeList.AsReadOnlySpan()[(int)_descriptorCount..], offset);
#endif
        var lockToken = false;
        _lock.Enter(ref lockToken);
        var index = --_descriptorCount;
        _freeList[index] = offset;
        _lock.Exit();
    }

    public void Shutdown()
    {
        _heap.Dispose();
        _memoryManager?.Free(ref _freeList);
        this = default;
    }

    [Conditional("DEBUG")]
    private static void CheckForDuplicateOffsets(ReadOnlySpan<uint> span, uint offset)
    {
        foreach (var value in span)
        {
            if (value == offset)
            {
                Debug.Fail($"The offset {offset} has been freed multiple times.");
            }
        }
    }

    public D3D12_GPU_DESCRIPTOR_HANDLE GetGPUStart() => _gpuStart;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TempDescriptorHandle AllocateTemp()
    {
        //NOTE(Jens): Move this to the initialization later
        var frameOffset = _frameIndex * _numberOfTempDescriptors;
        var offset = Interlocked.Increment(ref _tempOffset) - 1;

        var index = frameOffset + offset + _numberOfDescriptors;
        var offsetInBytes = index * _incrementSize;

        return new TempDescriptorHandle(index, _cpuStart.ptr + offsetInBytes, _gpuStart.ptr + offsetInBytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndFrame()
    {
        _frameIndex = (_frameIndex + 1) % _frameCount;
        _tempOffset = 0;
    }
}
