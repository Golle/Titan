using System.Diagnostics;
using System.Runtime.CompilerServices;
using Titan.Core.Logging;
using Titan.Windows;
using Titan.Windows.D3D12;

namespace Titan.Graphics.D3D12Take2;
/// <summary>
/// This is not a good implementation, but it will do for now. (Still learning)
/// </summary>
internal unsafe struct GPUHeap
{
    private ComPtr<ID3D12DescriptorHeap> _heap;
    private D3D12_GPU_DESCRIPTOR_HANDLE _gpuStart;
    private D3D12_CPU_DESCRIPTOR_HANDLE _cpuStart;
    private uint _capacity;
    private uint _count;
    private uint _descriptorSize;
    private D3D12_DESCRIPTOR_HEAP_TYPE _type;

    private bool _shaderVisible;
    //NOTE(Jens): add a free list

    public static bool Create(ID3D12Device4* device, D3D12_DESCRIPTOR_HEAP_TYPE type, uint numberOfDescriptors, bool shaderVisible, out GPUHeap heap)
    {
        heap = default;
        Debug.Assert(!(type == D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_SAMPLER && numberOfDescriptors > D3D12Common.D3D12_MAX_SHADER_VISIBLE_SAMPLER_HEAP_SIZE));
        Debug.Assert(!((type is D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV or D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_DSV or D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_RTV) && numberOfDescriptors > D3D12Common.D3D12_MAX_SHADER_VISIBLE_DESCRIPTOR_HEAP_SIZE_TIER_1));

        D3D12_DESCRIPTOR_HEAP_DESC desc = new()
        {
            Flags = shaderVisible ? D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE : D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_NONE,
            NumDescriptors = numberOfDescriptors,
            Type = type,
            NodeMask = 0
        };

        var hr = device->CreateDescriptorHeap(&desc, typeof(ID3D12DescriptorHeap).GUID, (void**)heap._heap.GetAddressOf());
        if (Common.FAILED(hr))
        {
            Logger.Error<GPUHeap>($"Failed to create {nameof(ID3D12DescriptorHeap)} with HRESULT {hr}");
            return false;
        }

        fixed (char* pName = $"GPUHeap {type}")
        {
            heap._heap.Get()->SetName(pName);
        }


        // CPU handle start
        {
            D3D12_CPU_DESCRIPTOR_HANDLE handle;
            heap._heap.Get()->GetCPUDescriptorHandleForHeapStart(&handle);
            heap._cpuStart = handle;
        }

        // GPU handle start
        if (shaderVisible)
        {
            D3D12_GPU_DESCRIPTOR_HANDLE handle;
            heap._heap.Get()->GetGPUDescriptorHandleForHeapStart(&handle);
            heap._gpuStart = handle;
        }
        heap._capacity = numberOfDescriptors;
        heap._type = type;
        heap._descriptorSize = device->GetDescriptorHandleIncrementSize(type);
        heap._shaderVisible = shaderVisible;

        return true;
    }


    public DescriptorHandle Allocate()
    {
        Debug.Assert(_heap.Get() != null);
        Debug.Assert(_count < _capacity);
        
        var offset = _count++ * _descriptorSize;
       
        Unsafe.SkipInit(out DescriptorHandle handle);
        handle.CpuHandle.ptr =_cpuStart.ptr + offset;
        handle.GpuHandle.ptr = _shaderVisible ? _gpuStart.ptr + offset : 0;
        return handle;
    }

    public void Free(ref DescriptorHandle handle)
    {
        //NOTE(Jens): Make sure this handle belongs to this heap
        //NOTE(Jens): Implement the free list to support alloc and free
        //NOTE(Jens): Must support deferred releases, so we should probably handle this inside the ECS/Asset system
        handle.CpuHandle = default;
        handle.GpuHandle= default;
        Logger.Warning<GPUHeap>("FreeList has not been implemented for the heap yet.");

    }

    public void Release()
    {
        _heap.Release();
    }

    public void SetName(string name)
    {
        fixed (char* pName = name)
        {
            _heap.Get()->SetName(pName);
        }
    }
}
