using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

[Guid("189819f1-1db6-4b57-be54-1821339b85f7")]
//[Guid("e865df17-a9ee-46f9-a463-3098315aa2e5")]
public unsafe struct ID3D12Device
{
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //    HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    //     ID3D12Device* This,
    //     _In_ REFGUID guid,
    //     _Inout_  UINT* pDataSize,
    //     _Out_writes_bytes_opt_( *pDataSize )  void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    // ID3D12Device* This,
    // _In_ REFGUID guid,
    // _In_  UINT DataSize,
    // _In_reads_bytes_opt_( DataSize )  const void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    // ID3D12Device* This,
    // _In_ REFGUID guid,
    // _In_opt_  const IUnknown* pData);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetName(char* Name)
        => ((delegate* unmanaged[Stdcall]<void*, char*, HRESULT>)_vtbl[6])(Unsafe.AsPointer(ref this), Name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint GetNodeCount()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[7])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommandQueue(D3D12_COMMAND_QUEUE_DESC* pDesc, Guid* riid, void** ppCommandQueue)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_COMMAND_QUEUE_DESC*, Guid*, void**, HRESULT>)_vtbl[8])(Unsafe.AsPointer(ref this), pDesc, riid, ppCommandQueue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE type, Guid* riid, void** ppCommandAllocator)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_COMMAND_LIST_TYPE, Guid*, void**, HRESULT>)_vtbl[9])(Unsafe.AsPointer(ref this), type, riid, ppCommandAllocator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateGraphicsPipelineState(D3D12_GRAPHICS_PIPELINE_STATE_DESC* pDesc, Guid* riid, void** ppPipelineState)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_GRAPHICS_PIPELINE_STATE_DESC*, Guid*, void**, HRESULT>)_vtbl[10])(Unsafe.AsPointer(ref this), pDesc, riid, ppPipelineState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateComputePipelineState(D3D12_COMPUTE_PIPELINE_STATE_DESC* pDesc, Guid* riid, void** ppPipelineState)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_COMPUTE_PIPELINE_STATE_DESC*, Guid*, void**, HRESULT>)_vtbl[11])(Unsafe.AsPointer(ref this), pDesc, riid, ppPipelineState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommandList(uint nodeMask, D3D12_COMMAND_LIST_TYPE type, ID3D12CommandAllocator* pCommandAllocator, ID3D12PipelineState* pInitialState, Guid* riid, void** ppCommandList)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_COMMAND_LIST_TYPE, ID3D12CommandAllocator*, ID3D12PipelineState*, Guid*, void**, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), nodeMask, type, pCommandAllocator, pInitialState, riid, ppCommandList);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CheckFeatureSupport(D3D12_FEATURE Feature, void* pFeatureSupportData, uint FeatureSupportDataSize)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_FEATURE, void*, uint, HRESULT>)_vtbl[13])(Unsafe.AsPointer(ref this), Feature, pFeatureSupportData, FeatureSupportDataSize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateDescriptorHeap(D3D12_DESCRIPTOR_HEAP_DESC* pDescriptorHeapDesc, Guid* riid, void** ppvHeap)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_DESCRIPTOR_HEAP_DESC*, Guid*, void**, HRESULT>)_vtbl[14])(Unsafe.AsPointer(ref this), pDescriptorHeapDesc, riid, ppvHeap);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE DescriptorHeapType)
         => ((delegate* unmanaged[Stdcall]<void*, D3D12_DESCRIPTOR_HEAP_TYPE, uint>)_vtbl[15])(Unsafe.AsPointer(ref this), DescriptorHeapType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateRootSignature(uint nodeMask, void* pBlobWithRootSignature, nuint blobLengthInBytes, Guid* riid, void** ppvRootSignature)
        => ((delegate* unmanaged[Stdcall]<void*, uint, void*, nuint, Guid*, void**, HRESULT>)_vtbl[16])(Unsafe.AsPointer(ref this), nodeMask, pBlobWithRootSignature, blobLengthInBytes, riid, ppvRootSignature);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateConstantBufferView(D3D12_CONSTANT_BUFFER_VIEW_DESC* pDesc, D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_CONSTANT_BUFFER_VIEW_DESC*, D3D12_CPU_DESCRIPTOR_HANDLE, void>)_vtbl[17])(Unsafe.AsPointer(ref this), pDesc, DestDescriptor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateShaderResourceView(ID3D12Resource* pResource, D3D12_SHADER_RESOURCE_VIEW_DESC* pDesc, D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, D3D12_SHADER_RESOURCE_VIEW_DESC*, D3D12_CPU_DESCRIPTOR_HANDLE, void>)_vtbl[18])(Unsafe.AsPointer(ref this), pResource, pDesc, DestDescriptor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateUnorderedAccessView(ID3D12Resource* pResource, ID3D12Resource* pCounterResource, D3D12_UNORDERED_ACCESS_VIEW_DESC* pDesc, D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, ID3D12Resource*, D3D12_UNORDERED_ACCESS_VIEW_DESC*, D3D12_CPU_DESCRIPTOR_HANDLE, void>)_vtbl[19])(Unsafe.AsPointer(ref this), pResource, pCounterResource, pDesc, DestDescriptor);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateRenderTargetView(ID3D12Resource* pResource, D3D12_RENDER_TARGET_VIEW_DESC* pDesc, D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, D3D12_RENDER_TARGET_VIEW_DESC*, D3D12_CPU_DESCRIPTOR_HANDLE, void>)_vtbl[20])(Unsafe.AsPointer(ref this), pResource, pDesc, DestDescriptor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateDepthStencilView(ID3D12Resource* pResource, D3D12_DEPTH_STENCIL_VIEW_DESC* pDesc, D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, D3D12_DEPTH_STENCIL_VIEW_DESC*, D3D12_CPU_DESCRIPTOR_HANDLE, void>)_vtbl[21])(Unsafe.AsPointer(ref this), pResource, pDesc, DestDescriptor);

    //void (STDMETHODCALLTYPE* CreateSampler ) (
    //    ID3D12Device* This,
    //    _In_  const D3D12_SAMPLER_DESC* pDesc,
    //    _In_  D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor);

    //void (STDMETHODCALLTYPE* CopyDescriptors ) (
    //    ID3D12Device* This,
    //    _In_ UINT NumDestDescriptorRanges,
    //    _In_reads_(NumDestDescriptorRanges)  const D3D12_CPU_DESCRIPTOR_HANDLE* pDestDescriptorRangeStarts,
    //    _In_reads_opt_(NumDestDescriptorRanges)  const UINT* pDestDescriptorRangeSizes,
    //    _In_  UINT NumSrcDescriptorRanges,
    //    _In_reads_(NumSrcDescriptorRanges)  const D3D12_CPU_DESCRIPTOR_HANDLE* pSrcDescriptorRangeStarts,
    //    _In_reads_opt_(NumSrcDescriptorRanges)  const UINT* pSrcDescriptorRangeSizes,
    //    _In_  D3D12_DESCRIPTOR_HEAP_TYPE DescriptorHeapsType);

    //void (STDMETHODCALLTYPE* CopyDescriptorsSimple ) (
    //    ID3D12Device* This,
    //    _In_ UINT NumDescriptors,
    //    _In_  D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptorRangeStart,
    //    _In_  D3D12_CPU_DESCRIPTOR_HANDLE SrcDescriptorRangeStart,
    //    _In_  D3D12_DESCRIPTOR_HEAP_TYPE DescriptorHeapsType);

    //D3D12_RESOURCE_ALLOCATION_INFO(STDMETHODCALLTYPE* GetResourceAllocationInfo)(
    // ID3D12Device* This,
    // _In_ UINT visibleMask,
    // _In_  UINT numResourceDescs,
    // _In_reads_(numResourceDescs)  const D3D12_RESOURCE_DESC* pResourceDescs);

    //D3D12_HEAP_PROPERTIES(STDMETHODCALLTYPE* GetCustomHeapProperties)(
    // ID3D12Device* This,
    // _In_ UINT nodeMask,
    // D3D12_HEAP_TYPE heapType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommittedResource(D3D12_HEAP_PROPERTIES* pHeapProperties, D3D12_HEAP_FLAGS HeapFlags, D3D12_RESOURCE_DESC* pDesc, D3D12_RESOURCE_STATES InitialResourceState, D3D12_CLEAR_VALUE* pOptimizedClearValue, Guid* riidResource, void** ppvResource)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_HEAP_PROPERTIES*, D3D12_HEAP_FLAGS, D3D12_RESOURCE_DESC*, D3D12_RESOURCE_STATES, D3D12_CLEAR_VALUE*, Guid*, void**, HRESULT>)_vtbl[27])(Unsafe.AsPointer(ref this), pHeapProperties, HeapFlags, pDesc, InitialResourceState, pOptimizedClearValue, riidResource, ppvResource);

    //HRESULT(STDMETHODCALLTYPE* CreateHeap)(
    // ID3D12Device* This,
    // _In_  const D3D12_HEAP_DESC* pDesc,
    // REFIID riid,
    //        _COM_Outptr_opt_ void** ppvHeap);

    //HRESULT(STDMETHODCALLTYPE* CreatePlacedResource)(
    // ID3D12Device* This,
    // _In_ ID3D12Heap * pHeap,
    // UINT64 HeapOffset,
    //        _In_ const D3D12_RESOURCE_DESC* pDesc,
    //       D3D12_RESOURCE_STATES InitialState,
    //        _In_opt_ const D3D12_CLEAR_VALUE* pOptimizedClearValue,
    //       REFIID riid,
    //        _COM_Outptr_opt_ void** ppvResource);

    //HRESULT(STDMETHODCALLTYPE* CreateReservedResource)(
    // ID3D12Device* This,
    // _In_  const D3D12_RESOURCE_DESC* pDesc,
    // D3D12_RESOURCE_STATES InitialState,
    //        _In_opt_ const D3D12_CLEAR_VALUE* pOptimizedClearValue,
    //       REFIID riid,
    //        _COM_Outptr_opt_ void** ppvResource);

    //HRESULT(STDMETHODCALLTYPE* CreateSharedHandle)(
    // ID3D12Device* This,
    // _In_ ID3D12DeviceChild * pObject,
    // _In_opt_  const SECURITY_ATTRIBUTES* pAttributes,
    // DWORD Access,
    //        _In_opt_ LPCWSTR Name,
    //        _Out_ HANDLE *pHandle);

    //    HRESULT(STDMETHODCALLTYPE* OpenSharedHandle)(
    //     ID3D12Device* This,
    //     _In_ HANDLE NTHandle,
    //     REFIID riid,
    //        _COM_Outptr_opt_ void** ppvObj);

    //HRESULT(STDMETHODCALLTYPE* OpenSharedHandleByName)(
    // ID3D12Device* This,
    // _In_ LPCWSTR Name,
    // DWORD Access,
    //        /* [annotation][out] */ 
    //        _Out_ HANDLE *pNTHandle);

    //    HRESULT(STDMETHODCALLTYPE* MakeResident)(
    //     ID3D12Device* This,
    //     UINT NumObjects,
    //     _In_reads_(NumObjects) ID3D12Pageable *const * ppObjects);

    //HRESULT(STDMETHODCALLTYPE* Evict)(
    // ID3D12Device* This,
    // UINT NumObjects,
    // _In_reads_(NumObjects) ID3D12Pageable *const * ppObjects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateFence(ulong initialValue, D3D12_FENCE_FLAGS flags, Guid* riid, void** ppFence) 
        => ((delegate* unmanaged[Stdcall]<void*, ulong, D3D12_FENCE_FLAGS, Guid*, void**, HRESULT>)_vtbl[36])(Unsafe.AsPointer(ref this), initialValue, flags, riid, ppFence);

    //HRESULT(STDMETHODCALLTYPE* GetDeviceRemovedReason)(
    // ID3D12Device* This);

    //    void (STDMETHODCALLTYPE* GetCopyableFootprints ) (
    //        ID3D12Device* This,
    //        _In_  const D3D12_RESOURCE_DESC* pResourceDesc,
    //        _In_range_(0, D3D12_REQ_SUBRESOURCES)  UINT FirstSubresource,
    //         _In_range_(0, D3D12_REQ_SUBRESOURCES - FirstSubresource)  UINT NumSubresources,
    //            UINT64 BaseOffset,
    //        _Out_writes_opt_(NumSubresources)  D3D12_PLACED_SUBRESOURCE_FOOTPRINT* pLayouts,
    //        _Out_writes_opt_(NumSubresources)  UINT* pNumRows,
    //        _Out_writes_opt_(NumSubresources)  UINT64* pRowSizeInBytes,
    //        _Out_opt_  UINT64* pTotalBytes);

    //HRESULT(STDMETHODCALLTYPE* CreateQueryHeap)(
    // ID3D12Device* This,
    //_In_  const D3D12_QUERY_HEAP_DESC* pDesc,
    //    REFIID riid,
    //_COM_Outptr_opt_ void** ppvHeap);

    //HRESULT(STDMETHODCALLTYPE* SetStablePowerState)(
    //    ID3D12Device* This,
    //    BOOL Enable);

    //HRESULT(STDMETHODCALLTYPE* CreateCommandSignature)(
    //ID3D12Device* This,
    //_In_  const D3D12_COMMAND_SIGNATURE_DESC* pDesc,
    //    _In_opt_  ID3D12RootSignature* pRootSignature,
    //    REFIID riid,
    //_COM_Outptr_opt_ void** ppvCommandSignature);

    //void (STDMETHODCALLTYPE* GetResourceTiling ) (
    //ID3D12Device* This,
    //_In_ ID3D12Resource * pTiledResource,
    //_Out_opt_  UINT* pNumTilesForEntireResource,
    //_Out_opt_  D3D12_PACKED_MIP_INFO* pPackedMipDesc,
    //_Out_opt_  D3D12_TILE_SHAPE* pStandardTileShapeForNonPackedMips,
    //_Inout_opt_  UINT* pNumSubresourceTilings,
    //_In_  UINT FirstSubresourceTilingToGet,
    //_Out_writes_(*pNumSubresourceTilings)  D3D12_SUBRESOURCE_TILING* pSubresourceTilingsForNonPackedMips);

    //LUID(STDMETHODCALLTYPE* GetAdapterLuid)(
    //    ID3D12Device* This);
}
