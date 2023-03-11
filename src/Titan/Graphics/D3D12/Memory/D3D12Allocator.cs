using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Graphics.D3D12.Utils;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D12;
using Titan.Platform.Win32.DXGI;
using static Titan.Platform.Win32.Win32Common;

namespace Titan.Graphics.D3D12.Memory;
internal unsafe struct TempBufferDescriptor
{
    public ComPtr<ID3D12Resource> Resource;
    public void* CPUAddress;
    public ulong GPUAddress;
    public volatile uint Offset;
    public DescriptorHandle SRV;
}

[SkipLocalsInit]
internal unsafe struct StructuredBuffer<T> where T : unmanaged
{
    private static readonly uint Stride = (uint)sizeof(T);
    public void* CPUAddress;
    public ulong GPUAddress;
    public uint Count;
    public uint DescriptorIndex;

    public void Copy(T* data, uint count, uint offset = 0)
    {
        Debug.Assert(offset + count <= Count);
        Debug.Assert(CPUAddress != null);

        MemoryUtils.Copy((byte*)CPUAddress + offset * Stride, data, Stride * count);
    }
    public void Copy(ReadOnlySpan<T> data, uint offset = 0)
    {
        Debug.Assert(data.Length + offset <= Count);
        Debug.Assert(CPUAddress != null);
        MemoryUtils.Copy((byte*)CPUAddress + offset * Stride, data);
    }
}

[SkipLocalsInit]
internal unsafe struct TempConstantBuffer
{
    public void* CPUAddress;
    public ulong GPUAddress;
    public uint Size;
}

internal unsafe class D3D12Allocator
{
    private const uint ConstantBufferAlignment = 256;
    private TitanArray<TempBufferDescriptor> _buffers;
    private uint _frameIndex;
    private uint _frameCount;
    private uint _bufferSize;


    public DescriptorHeap SRVDescriptorHeap;
    private DescriptorHeap DSVDescriptorHeap;
    private DescriptorHeap UAVDescriptorHeap;
    private DescriptorHeap RTVDescriptorHeap;


    private IMemoryManager _memoryManager;
    private D3D12GraphicsDevice _device;

    public bool Init(D3D12GraphicsDevice device, IMemoryManager memoryManager, uint frameCount, GPUMemoryConfig config)
    {
        Debug.Assert(frameCount is >= 2 and <= 3);

        // Set up the descriptor heaps
        if (!SRVDescriptorHeap.Init(device, memoryManager, DescriptorHeapType.ShaderResourceView, config.SRVCount, config.SRVCount / 4, frameCount, true))
        {
            Logger.Error<D3D12Allocator>($"Failed to init the {nameof(DescriptorHeap)} with count {config.SRVCount} and type {DescriptorHeapType.ShaderResourceView}");
            goto Error;
        }
        if (!RTVDescriptorHeap.Init(device, memoryManager, DescriptorHeapType.RenderTargetView, config.RTVCount, 0, frameCount, false))
        {
            Logger.Error<D3D12Allocator>($"Failed to init the {nameof(DescriptorHeap)}  with count {config.RTVCount} and type {DescriptorHeapType.RenderTargetView}");
            goto Error;
        }
        if (!DSVDescriptorHeap.Init(device, memoryManager, DescriptorHeapType.DepthStencilView, config.DSVCount, 0, frameCount, false))
        {
            Logger.Error<D3D12Allocator>($"Failed to init the {nameof(DescriptorHeap)}  with count {config.DSVCount} and type {DescriptorHeapType.DepthStencilView}");
            goto Error;
        }
        if (!UAVDescriptorHeap.Init(device, memoryManager, DescriptorHeapType.UnorderedAccessView, config.UAVCount, 0, frameCount, false))
        {
            Logger.Error<D3D12Allocator>($"Failed to init the  {nameof(DescriptorHeap)} with count {config.UAVCount} and type {DescriptorHeapType.UnorderedAccessView}");
            goto Error;
        }

        _buffers = memoryManager.AllocArray<TempBufferDescriptor>(frameCount);
        if (!_buffers.IsValid)
        {
            Logger.Error<D3D12Allocator>($"Failed to allocate memory for {frameCount} {nameof(TempBufferDescriptor)}.");
            goto Error;
        }
        // Create the temporary buffers
        var alignedTempBufferSize = MemoryUtils.AlignToUpper(config.TempConstantBufferSize, ConstantBufferAlignment);
        for (var i = 0; i < frameCount; i++)
        {
            var resource = device.CreateBuffer(alignedTempBufferSize, true, D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_VERTEX_AND_CONSTANT_BUFFER);
            if (resource == null)
            {
                Logger.Error<D3D12Allocator>($"Failed to create the temporary buffer at index {i}.");
                goto Error;
            }
            D3D12Helpers.SetName(resource, $"TempBuffer[{i}]");
            var desc = new TempBufferDescriptor
            {
                Resource = resource,
                GPUAddress = resource->GetGPUVirtualAddress(),
            };
            var hr = resource->Map(0, null, &desc.CPUAddress);
            if (FAILED(hr))
            {
                Logger.Error<D3D12Allocator>($"Failed to map the temporary buffer with HRESULT {hr}");
                goto Error;
            }
            desc.SRV = SRVDescriptorHeap.Allocate();
            if (!desc.SRV.IsValid)
            {
                Logger.Error<D3D12Allocator>("Failed to allocate a SRV handle for the temporary buffer.");
                goto Error;
            }
            _buffers[i] = desc;
        }

        _frameIndex = 0;
        _frameCount = frameCount;
        _bufferSize = config.TempConstantBufferSize;
        _memoryManager = memoryManager;
        _device = device;
        return true;

//NOTE(Jens): handle errors and release resources
Error:
        Shutdown();
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StructuredBuffer<T> AllocateTempStructuredBuffer<T>(int count, bool createDescriptor) where T : unmanaged
    {
        Debug.Assert(count >= 0);
        return AllocateTempStructuredBuffer<T>((uint)count, createDescriptor);
    }
    public StructuredBuffer<T> AllocateTempStructuredBuffer<T>(uint count, bool createDescriptor) where T : unmanaged
    {
        //Debug.Assert(int.IsPow2(sizeof(T)), $"A structured buffer must have a size that is a power of 2. Type {typeof(T).Name} has a size of {sizeof(T)}");
        var stride = (uint)sizeof(T);
        var size = stride * count;
        var mappedMemory = GetTempMemory(size, stride);
        var tempBuffer = new StructuredBuffer<T>
        {
            GPUAddress = mappedMemory.GPUAddress,
            CPUAddress = mappedMemory.CPUAddress,
            Count = count
        };

        Debug.Assert(mappedMemory.ResourceOffset % stride == 0);

        if (createDescriptor)
        {
            var handle = SRVDescriptorHeap.AllocateTemp();
            Unsafe.SkipInit(out D3D12_SHADER_RESOURCE_VIEW_DESC srvDesc);
            srvDesc.Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN;
            srvDesc.ViewDimension = D3D12_SRV_DIMENSION.D3D12_SRV_DIMENSION_BUFFER;
            srvDesc.Shader4ComponentMapping = D3D12Constants.D3D12_DEFAULT_SHADER_4_COMPONENT_MAPPING;
            srvDesc.Buffer.FirstElement = mappedMemory.ResourceOffset / stride; // uint32(tempMem.ResourceOffset / stride);
            srvDesc.Buffer.Flags = D3D12_BUFFER_SRV_FLAGS.D3D12_BUFFER_SRV_FLAG_NONE;
            srvDesc.Buffer.NumElements = count;
            srvDesc.Buffer.StructureByteStride = stride;
            _device.Device->CreateShaderResourceView(_buffers[_frameIndex].Resource, &srvDesc, handle.CPU);
            tempBuffer.DescriptorIndex = handle.Index;
        }

        return tempBuffer;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TempConstantBuffer AllocateTempConstantBuffer(int size, uint alignment = ConstantBufferAlignment)
    {
        Debug.Assert(size >= 0);
        return AllocateTempConstantBuffer((uint)size, alignment);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TempConstantBuffer AllocateTempConstantBuffer(uint size, uint alignment = ConstantBufferAlignment)
    {
        Debug.Assert(alignment % ConstantBufferAlignment == 0, "Constant buffers must be aligned with a multiple of 256 bytes.");
        var mappedMemory = GetTempMemory(size, alignment);
        return new TempConstantBuffer
        {
            GPUAddress = mappedMemory.GPUAddress,
            CPUAddress = mappedMemory.CPUAddress,
            Size = size
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MappedTempMemory GetTempMemory(uint size, uint alignment = 0)
    {
        var allocSize = size + alignment;
        var offset = Interlocked.Add(ref _buffers[_frameIndex].Offset, allocSize) - allocSize;
        Debug.Assert(offset + size < _bufferSize);
        if (alignment > 0)
        {
            // align the start offset to the stride of the requested size. 
            offset = MemoryUtils.AlignNotPowerOf2(offset, alignment);
        }

        //NOTE(Jens): if we replace the buffers with a single resource (which we should), we need to update the offset to include the frame index
        return new()
        {
            GPUAddress = _buffers[_frameIndex].GPUAddress + offset,
            CPUAddress = (byte*)_buffers[_frameIndex].CPUAddress + offset,
            ResourceOffset = offset
        };
    }

    public DescriptorHandle Allocate(DescriptorHeapType type) =>
        type switch
        {
            DescriptorHeapType.DepthStencilView => DSVDescriptorHeap.Allocate(),
            DescriptorHeapType.RenderTargetView => RTVDescriptorHeap.Allocate(),
            DescriptorHeapType.UnorderedAccessView => UAVDescriptorHeap.Allocate(),
            DescriptorHeapType.ShaderResourceView => SRVDescriptorHeap.Allocate(),
            _ => DescriptorHandle.Invalid
        };

    public void Free(DescriptorHandle handle)
    {
        switch (handle.Type)
        {
            case DescriptorHeapType.ShaderResourceView:
                SRVDescriptorHeap.Free(handle);
                break;
            case DescriptorHeapType.RenderTargetView:
                RTVDescriptorHeap.Free(handle);
                break;
            case DescriptorHeapType.DepthStencilView:
                DSVDescriptorHeap.Free(handle);
                break;
            case DescriptorHeapType.UnorderedAccessView:
                UAVDescriptorHeap.Free(handle);
                break;
        }
    }

    public void Update()
    {
        _frameIndex = (_frameIndex + 1) % _frameCount;
        _buffers[_frameIndex].Offset = 0;

        // only the SRV have temporary allocations, no need to call end frame on the other ones.
        SRVDescriptorHeap.EndFrame();
    }

    public void Shutdown()
    {
        foreach (ref var buffer in _buffers.AsSpan())
        {
            if (buffer.SRV.IsValid)
            {
                SRVDescriptorHeap.Free(buffer.SRV);
            }
            buffer.Resource.Dispose();
        }
        _memoryManager.Free(ref _buffers);

        SRVDescriptorHeap.Shutdown();
        DSVDescriptorHeap.Shutdown();
        RTVDescriptorHeap.Shutdown();
        UAVDescriptorHeap.Shutdown();
    }

    [SkipLocalsInit]
    private ref struct MappedTempMemory
    {
        public void* CPUAddress;
        public ulong GPUAddress;
        public uint ResourceOffset;
    }
}
