using System.Diagnostics;
using System.Text;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Platform.Win32;
using Titan.Platform.Win32.D3D;
using Titan.Platform.Win32.D3D12;
using Titan.Platform.Win32.DXGI;
using static Titan.Platform.Win32.D3D12.D3D12_COMMAND_LIST_FLAGS;
using static Titan.Platform.Win32.D3D12.D3D12_COMMAND_QUEUE_FLAGS;
using static Titan.Platform.Win32.D3D12.D3D12_FENCE_FLAGS;
using static Titan.Platform.Win32.D3D12.D3D12_RESOURCE_FLAGS;
using static Titan.Platform.Win32.D3D12.D3D12_RESOURCE_STATES;
using static Titan.Platform.Win32.D3D12.D3D12Common;
using static Titan.Platform.Win32.Win32Common;

namespace Titan.Graphics.D3D12;

internal unsafe class D3D12GraphicsDevice : IGraphicsDevice
{
    //NOTE(Jens): These heaps should be managed by the caller, and not by the device
    private static readonly D3D12_HEAP_PROPERTIES DefaultHeap = new()
    {
        Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT,
        CPUPageProperty = D3D12_CPU_PAGE_PROPERTY.D3D12_CPU_PAGE_PROPERTY_UNKNOWN,
        MemoryPoolPreference = D3D12_MEMORY_POOL.D3D12_MEMORY_POOL_UNKNOWN,
        CreationNodeMask = 0,
        VisibleNodeMask = 0
    };

    private static readonly D3D12_HEAP_PROPERTIES UploadHeap = new()
    {
        Type = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_UPLOAD,
        CPUPageProperty = D3D12_CPU_PAGE_PROPERTY.D3D12_CPU_PAGE_PROPERTY_UNKNOWN,
        MemoryPoolPreference = D3D12_MEMORY_POOL.D3D12_MEMORY_POOL_UNKNOWN,
        CreationNodeMask = 0,
        VisibleNodeMask = 0
    };


    private ComPtr<ID3D12Device4> _device;
    public ID3D12Device4* Device => _device.Get();
    public bool Init(DXGIAdapter adapter, D3D_FEATURE_LEVEL minFeatureLevel, bool debug)
    {
        Debug.Assert(_device.Get() == null);

        if (debug)
        {
            if (InitDebugLayer())
            {
                Logger.Info<D3D12GraphicsDevice>("D3D12Debug layer initialized.");
            }
            else
            {
                Logger.Warning<D3D12GraphicsDevice>("Failed to init the DebugLayer. This won't break anything but memory tracking wont work.");
            }
        }

        var hr = D3D12CreateDevice((IUnknown*)adapter.Adapter, minFeatureLevel, _device.UUID, (void**)_device.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to create the {nameof(ID3D12Device4)} with HRESULT {hr}");
            return false;
        }
        return true;
    }

    private static bool InitDebugLayer()
    {
        using ComPtr<ID3D12Debug> spDebugController0 = default;
        using ComPtr<ID3D12Debug1> spDebugController1 = default;
        var hr = D3D12GetDebugInterface(typeof(ID3D12Debug).GUID, (void**)spDebugController0.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed {nameof(D3D12GetDebugInterface)} with HRESULT: {hr}");
            return false;
        }

        hr = spDebugController0.Get()->QueryInterface(typeof(ID3D12Debug1).GUID, (void**)spDebugController1.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to query {nameof(ID3D12Debug1)} interface with HRESULT: {hr}");
            return false;
        }
        spDebugController1.Get()->EnableDebugLayer();
        spDebugController1.Get()->SetEnableGPUBasedValidation(true);
        return true;
    }

    public ID3D12Resource* CreateBuffer(uint size, bool isCpuVisible = false, D3D12_RESOURCE_STATES state = D3D12_RESOURCE_STATE_COMMON, D3D12_RESOURCE_FLAGS flags = D3D12_RESOURCE_FLAG_NONE)
    {
        var heap = isCpuVisible ? UploadHeap : DefaultHeap;
        D3D12_RESOURCE_DESC resourceDesc = new()
        {
            Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_BUFFER,
            Width = size,
            Height = 1,
            MipLevels = 1,
            DepthOrArraySize = 1,
            Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
            SampleDesc =
            {
                Count = 1,
                Quality = 0
            },
            Alignment = 0,
            Layout = D3D12_TEXTURE_LAYOUT.D3D12_TEXTURE_LAYOUT_ROW_MAJOR,
            Flags = isCpuVisible ? D3D12_RESOURCE_FLAG_NONE : flags
        };

        var resourceState = isCpuVisible ? D3D12_RESOURCE_STATE_GENERIC_READ : state;
        ComPtr<ID3D12Resource> resource = default;
        var hr = _device.Get()->CreateCommittedResource1(&heap, D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE, &resourceDesc, resourceState, null, null, resource.UUID, (void**)resource.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to create the {nameof(ID3D12Resource)} with HRESULT {hr}");
            return null;
        }
        return resource;
    }

    public ID3D12Resource* CreateTexture(uint width, uint height, DXGI_FORMAT format)
    {
        D3D12_RESOURCE_DESC resourceDesc = new()
        {
            Width = width,
            Height = height,
            Format = format,
            Flags = D3D12_RESOURCE_FLAG_NONE,
            DepthOrArraySize = 1, // change this when we support other types of textures.
            Alignment = 0,
            Layout = D3D12_TEXTURE_LAYOUT.D3D12_TEXTURE_LAYOUT_UNKNOWN,
            Dimension = D3D12_RESOURCE_DIMENSION.D3D12_RESOURCE_DIMENSION_TEXTURE2D,
            MipLevels = 1,
            SampleDesc = new DXGI_SAMPLE_DESC
            {
                Count = 1,
                Quality = 0
            }
        };

        ComPtr<ID3D12Resource> resource = default;
        var heapProperties = DefaultHeap;
        var hr = _device.Get()->CreateCommittedResource1(&heapProperties, D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_NONE, &resourceDesc, D3D12_RESOURCE_STATE_COMMON, null, null, resource.UUID, (void**)resource.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to create the texture resource with HRESULT {hr}");
            return null;
        }
        return resource.Get();
    }

    public ID3D12CommandAllocator* CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE type)
    {
        ComPtr<ID3D12CommandAllocator> commandAllocator = default;
        var hr = _device.Get()->CreateCommandAllocator(type, commandAllocator.UUID, (void**)commandAllocator.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to create the {nameof(ID3D12CommandAllocator)} with HRESULT {hr}");
            return null;
        }
        return commandAllocator;
    }

    public ID3D12DescriptorHeap* CreateDescriptorHeap(D3D12_DESCRIPTOR_HEAP_TYPE type, uint numberOfDescriptors, bool shaderVisible)
    {
        if (type is D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_RTV or D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_DSV)
        {
            shaderVisible = false;
        }

        D3D12_DESCRIPTOR_HEAP_DESC desc = new()
        {
            Flags = shaderVisible ? D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE : D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_NONE,
            NodeMask = 0,
            NumDescriptors = numberOfDescriptors,
            Type = type
        };
        ComPtr<ID3D12DescriptorHeap> descriptorHeap = default;
        var hr = _device.Get()->CreateDescriptorHeap(&desc, descriptorHeap.UUID, (void**)descriptorHeap.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to create the {nameof(ID3D12DescriptorHeap)} with HRESULT {hr}");
            return null;
        }
        return descriptorHeap;
    }

    public ID3D12CommandQueue* CreateCommandQueue(D3D12_COMMAND_LIST_TYPE type, D3D12_COMMAND_QUEUE_FLAGS flags = D3D12_COMMAND_QUEUE_FLAG_NONE)
    {
        D3D12_COMMAND_QUEUE_DESC desc = new()
        {
            Flags = D3D12_COMMAND_QUEUE_FLAG_NONE,
            NodeMask = 0,
            Priority = 0,
            Type = type
        };
        ComPtr<ID3D12CommandQueue> commandQueue = default;
        var hr = _device.Get()->CreateCommandQueue(&desc, commandQueue.UUID, (void**)commandQueue.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to create a {nameof(ID3D12CommandQueue)} with HRESULT {hr}");
            return null;
        }
        return commandQueue;
    }

    public ID3D12GraphicsCommandList* CreateCommandList(D3D12_COMMAND_LIST_TYPE type, D3D12_COMMAND_LIST_FLAGS flags = D3D12_COMMAND_LIST_FLAG_NONE)
    {
        ComPtr<ID3D12GraphicsCommandList> commandList = default;
        var hr = _device.Get()->CreateCommandList1(0, type, flags, commandList.UUID, (void**)commandList.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to create a {nameof(ID3D12GraphicsCommandList)} wiht HRESULT {hr}");
            return null;
        }
        return commandList;
    }

    public ID3D12Fence* CreateFence(ulong initialValue = 0ul, D3D12_FENCE_FLAGS flags = D3D12_FENCE_FLAG_NONE)
    {
        ComPtr<ID3D12Fence> fence = default;
        var hr = _device.Get()->CreateFence(initialValue, D3D12_FENCE_FLAG_NONE, fence.UUID, (void**)fence.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to create a {nameof(ID3D12Fence)} with HRESULT {hr}");
            return null;
        }

        return fence.Get();
    }

    public void CreateShaderResourceView(ID3D12Resource* resource, DXGI_FORMAT format, D3D12_CPU_DESCRIPTOR_HANDLE descriptorHandle)
    {
        //NOTE(Jens): this method is not really usable. figure out a better way to do this.
        Debug.Assert(resource != null);
        var desc = new D3D12_SHADER_RESOURCE_VIEW_DESC
        {
            Format = format,
            Shader4ComponentMapping = D3D12Constants.D3D12_DEFAULT_SHADER_4_COMPONENT_MAPPING,
            ViewDimension = D3D12_SRV_DIMENSION.D3D12_SRV_DIMENSION_TEXTURE2D,
            Texture2D = new D3D12_TEX2D_SRV
            {
                MipLevels = 1,
                MostDetailedMip = 0,
                PlaneSlice = 0,
                ResourceMinLODClamp = 0
            }
        };
        _device.Get()->CreateShaderResourceView(resource, &desc, descriptorHandle);
    }

    public ID3D12Heap* CreateHeap(uint sizeInBytes, D3D12_HEAP_FLAGS flags = D3D12_HEAP_FLAGS.D3D12_HEAP_FLAG_ALLOW_ALL_BUFFERS_AND_TEXTURES, D3D12_HEAP_TYPE heapType = D3D12_HEAP_TYPE.D3D12_HEAP_TYPE_DEFAULT, D3D12_CPU_PAGE_PROPERTY cpuPageProperty = D3D12_CPU_PAGE_PROPERTY.D3D12_CPU_PAGE_PROPERTY_UNKNOWN, D3D12_MEMORY_POOL memoryPool = D3D12_MEMORY_POOL.D3D12_MEMORY_POOL_UNKNOWN, bool msaa = false)
    {
        var alignment = msaa ? D3D12Constants.D3D12_DEFAULT_MSAA_RESOURCE_PLACEMENT_ALIGNMENT : D3D12Constants.D3D12_DEFAULT_RESOURCE_PLACEMENT_ALIGNMENT;
        D3D12_HEAP_DESC desc = new()
        {
            Flags = flags,
            Alignment = (ulong)alignment,
            Properties = new D3D12_HEAP_PROPERTIES
            {
                CPUPageProperty = cpuPageProperty,
                CreationNodeMask = 0,
                MemoryPoolPreference = memoryPool,
                Type = heapType,
                VisibleNodeMask = 0
            },
            SizeInBytes = MemoryUtils.AlignToUpper(sizeInBytes, (uint)alignment)
        };
        ComPtr<ID3D12Heap> heap = default;
        var hr = _device.Get()->CreateHeap(&desc, heap.UUID, (void**)heap.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to create a {nameof(ID3D12Heap)} with HRESULT {hr}");
            return null;
        }
        return heap;
    }

    public ID3D12PipelineState* CreatePipelineState(D3D12_PIPELINE_STATE_STREAM_DESC desc)
    {
        ComPtr<ID3D12PipelineState> pipelineState = default;
        var hr = _device.Get()->CreatePipelineState(&desc, pipelineState.UUID, (void**)pipelineState.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to create a {nameof(ID3D12PipelineState)} with HRESULT {hr}");
            return null;
        }

        return pipelineState;
    }

    public ID3D12RootSignature* CreateRootSignature(D3D12_ROOT_SIGNATURE_FLAGS flags, ReadOnlySpan<D3D12_ROOT_PARAMETER1> parameters, ReadOnlySpan<D3D12_STATIC_SAMPLER_DESC> staticSamplers)
    {
        HRESULT hr;
        using ComPtr<ID3DBlob> blob = default;
        using ComPtr<ID3DBlob> error = default;

        ComPtr<ID3D12RootSignature> rootSignature = default;

        fixed (D3D12_ROOT_PARAMETER1* pParameters = parameters)
        fixed (D3D12_STATIC_SAMPLER_DESC* pSamplers = staticSamplers)
        {
            var desc = new D3D12_ROOT_SIGNATURE_DESC1
            {
                Flags = flags,
                pParameters = pParameters,
                pStaticSamplers = pSamplers,
                NumParameters = (uint)parameters.Length,
                NumStaticSamplers = (uint)staticSamplers.Length
            };

            var versioned = new D3D12_VERSIONED_ROOT_SIGNATURE_DESC
            {
                Desc_1_1 = desc,
                Version = D3D_ROOT_SIGNATURE_VERSION.D3D_ROOT_SIGNATURE_VERSION_1_1
            };

            hr = D3D12SerializeVersionedRootSignature(&versioned, blob.GetAddressOf(), error.GetAddressOf());
            if (FAILED(hr))
            {
                Logger.Error<D3D12GraphicsDevice>($"Failed to serialize the root signature with HRESULT {hr}");

#if DEBUG
                var span = new ReadOnlySpan<byte>(error.Get()->GetBufferPointer(), (int)error.Get()->GetBufferSize());
                Logger.Error<D3D12GraphicsDevice>($"Error: {Encoding.UTF8.GetString(span)}");
#endif
                return null;
            }
        }

        hr = _device.Get()->CreateRootSignature(0, blob.Get()->GetBufferPointer(), blob.Get()->GetBufferSize(), rootSignature.UUID, (void**)rootSignature.GetAddressOf());
        if (FAILED(hr))
        {
            Logger.Error<D3D12GraphicsDevice>($"Failed to create the RootSignature with HRESULT {hr}");
            return null;
        }
        return rootSignature;
    }

    public void Shutdown()
    {
        Logger.Trace<D3D12GraphicsDevice>("Destroying the device");
        _device.Dispose();
    }
}
