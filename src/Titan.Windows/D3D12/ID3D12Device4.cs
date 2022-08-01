using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Windows.D3D12;

[Guid("e865df17-a9ee-46f9-a463-3098315aa2e5")]
public unsafe struct ID3D12Device4
{
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(in Guid riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, in Guid, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);
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

    //UINT(STDMETHODCALLTYPE* GetNodeCount)(
    // ID3D12Device* This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommandQueue(D3D12_COMMAND_QUEUE_DESC* pDesc, in Guid riid, void** ppCommandQueue)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_COMMAND_QUEUE_DESC*, in Guid, void**, HRESULT>)_vtbl[8])(Unsafe.AsPointer(ref this), pDesc, riid, ppCommandQueue);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE type, in Guid riid, void** ppCommandAllocator)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_COMMAND_LIST_TYPE, in Guid, void**, HRESULT>)_vtbl[9])(Unsafe.AsPointer(ref this), type, riid, ppCommandAllocator);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateGraphicsPipelineState(D3D12_GRAPHICS_PIPELINE_STATE_DESC* pDesc, in Guid riid, void** ppPipelineState)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_GRAPHICS_PIPELINE_STATE_DESC*, in Guid, void**, HRESULT>)_vtbl[10])(Unsafe.AsPointer(ref this), pDesc, riid, ppPipelineState);

    //HRESULT(STDMETHODCALLTYPE* CreateComputePipelineState)(
    // ID3D12Device* This,
    // _In_  const D3D12_COMPUTE_PIPELINE_STATE_DESC* pDesc,
    // REFIID riid,
    //        _COM_Outptr_ void** ppPipelineState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommandList(uint nodeMask, D3D12_COMMAND_LIST_TYPE type, ID3D12CommandAllocator* pCommandAllocator, ID3D12PipelineState* pInitialState, in Guid riid, void** ppCommandList)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_COMMAND_LIST_TYPE, ID3D12CommandAllocator*, ID3D12PipelineState*, in Guid, void**, HRESULT>)_vtbl[12])(Unsafe.AsPointer(ref this), nodeMask, type, pCommandAllocator, pInitialState, riid, ppCommandList);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CheckFeatureSupport(D3D12_FEATURE Feature, void* pFeatureSupportData, uint FeatureSupportDataSize)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_FEATURE, void*, uint, HRESULT>)_vtbl[13])(Unsafe.AsPointer(ref this), Feature, pFeatureSupportData, FeatureSupportDataSize);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateDescriptorHeap(D3D12_DESCRIPTOR_HEAP_DESC* pDescriptorHeapDesc, in Guid riid, void** ppvHeap)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_DESCRIPTOR_HEAP_DESC*, in Guid, void**, HRESULT>)_vtbl[14])(Unsafe.AsPointer(ref this), pDescriptorHeapDesc, riid, ppvHeap);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE DescriptorHeapType)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_DESCRIPTOR_HEAP_TYPE, uint>)_vtbl[15])(Unsafe.AsPointer(ref this), DescriptorHeapType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateRootSignature(uint nodeMask, void* pBlobWithRootSignature, nuint blobLengthInBytes, in Guid riid, void** ppvRootSignature)
        => ((delegate* unmanaged[Stdcall]<void*, uint, void*, nuint, in Guid, void**, HRESULT>)_vtbl[16])(Unsafe.AsPointer(ref this), nodeMask, pBlobWithRootSignature, blobLengthInBytes, riid, ppvRootSignature);

    //void (STDMETHODCALLTYPE* CreateConstantBufferView ) (
    //    ID3D12Device* This,
    //    _In_opt_  const D3D12_CONSTANT_BUFFER_VIEW_DESC* pDesc,
    //    _In_  D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor);

    //void (STDMETHODCALLTYPE* CreateShaderResourceView ) (
    //    ID3D12Device* This,
    //    _In_opt_ ID3D12Resource * pResource,
    //    _In_opt_  const D3D12_SHADER_RESOURCE_VIEW_DESC* pDesc,
    //    _In_  D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor);

    //void (STDMETHODCALLTYPE* CreateUnorderedAccessView ) (
    //    ID3D12Device* This,
    //    _In_opt_ ID3D12Resource * pResource,
    //    _In_opt_  ID3D12Resource* pCounterResource,
    //    _In_opt_  const D3D12_UNORDERED_ACCESS_VIEW_DESC* pDesc,
    //    _In_  D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateRenderTargetView(ID3D12Resource* pResource, D3D12_RENDER_TARGET_VIEW_DESC* pDesc, D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, D3D12_RENDER_TARGET_VIEW_DESC*, D3D12_CPU_DESCRIPTOR_HANDLE, void>)_vtbl[20])(Unsafe.AsPointer(ref this), pResource, pDesc, DestDescriptor);

    //void (STDMETHODCALLTYPE* CreateDepthStencilView ) (
    //    ID3D12Device* This,
    //    _In_opt_ ID3D12Resource * pResource,
    //    _In_opt_  const D3D12_DEPTH_STENCIL_VIEW_DESC* pDesc,
    //    _In_  D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public D3D12_HEAP_PROPERTIES* GetCustomHeapProperties(uint nodeMask, D3D12_HEAP_TYPE heapType, D3D12_HEAP_PROPERTIES* retVal)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_HEAP_PROPERTIES*, uint, D3D12_HEAP_TYPE, D3D12_HEAP_PROPERTIES*>)_vtbl[26])(Unsafe.AsPointer(ref this), retVal, nodeMask, heapType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommittedResource(D3D12_HEAP_PROPERTIES* pHeapProperties, D3D12_HEAP_FLAGS HeapFlags, D3D12_RESOURCE_DESC* pDesc, D3D12_RESOURCE_STATES InitialResourceState, D3D12_CLEAR_VALUE* pOptimizedClearValue, in Guid riidResource, void** ppvResource)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_HEAP_PROPERTIES*, D3D12_HEAP_FLAGS, D3D12_RESOURCE_DESC*, D3D12_RESOURCE_STATES, D3D12_CLEAR_VALUE*, in Guid, void**, HRESULT>)_vtbl[27])(Unsafe.AsPointer(ref this), pHeapProperties, HeapFlags, pDesc, InitialResourceState, pOptimizedClearValue, riidResource, ppvResource);

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
    public HRESULT CreateFence(ulong initialValue, D3D12_FENCE_FLAGS flags, in Guid riid, void** ppFence)
        => ((delegate* unmanaged[Stdcall]<void*, ulong, D3D12_FENCE_FLAGS, in Guid, void**, HRESULT>)_vtbl[36])(Unsafe.AsPointer(ref this), initialValue, flags, riid, ppFence);

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

    //DECLSPEC_XFGVIRT(ID3D12Device1, CreatePipelineLibrary)
    //    HRESULT(STDMETHODCALLTYPE* CreatePipelineLibrary)(
    //        ID3D12Device4* This,
    //        _In_reads_(BlobLength) const void* pLibraryBlob,
    //        SIZE_T BlobLength,
    //        REFIID riid,
    //        _COM_Outptr_  void** ppPipelineLibrary);

    //DECLSPEC_XFGVIRT(ID3D12Device1, SetEventOnMultipleFenceCompletion)
    //    HRESULT(STDMETHODCALLTYPE* SetEventOnMultipleFenceCompletion)(
    //        ID3D12Device4* This,
    //        _In_reads_(NumFences) ID3D12Fence *const * ppFences,
    //        _In_reads_(NumFences)  const UINT64* pFenceValues,
    //        UINT NumFences,
    //        D3D12_MULTIPLE_FENCE_WAIT_FLAGS Flags,
    //        HANDLE hEvent);

    //    DECLSPEC_XFGVIRT(ID3D12Device1, SetResidencyPriority)
    //    HRESULT(STDMETHODCALLTYPE* SetResidencyPriority)(
    //        ID3D12Device4* This,
    //        UINT NumObjects,
    //        _In_reads_(NumObjects) ID3D12Pageable *const * ppObjects,
    //        _In_reads_(NumObjects)  const D3D12_RESIDENCY_PRIORITY* pPriorities);

    //DECLSPEC_XFGVIRT(ID3D12Device2, CreatePipelineState)
    //    HRESULT(STDMETHODCALLTYPE* CreatePipelineState)(
    //        ID3D12Device4* This,
    //        const D3D12_PIPELINE_STATE_STREAM_DESC* pDesc,
    //        REFIID riid,
    //        _COM_Outptr_ void** ppPipelineState);

    //DECLSPEC_XFGVIRT(ID3D12Device3, OpenExistingHeapFromAddress)
    //    HRESULT(STDMETHODCALLTYPE* OpenExistingHeapFromAddress)(
    //        ID3D12Device4* This,
    //        _In_  const void* pAddress,
    //        REFIID riid,
    //        _COM_Outptr_ void** ppvHeap);

    //DECLSPEC_XFGVIRT(ID3D12Device3, OpenExistingHeapFromFileMapping)
    //    HRESULT(STDMETHODCALLTYPE* OpenExistingHeapFromFileMapping)(
    //        ID3D12Device4* This,
    //        _In_ HANDLE hFileMapping,
    //        REFIID riid,
    //        _COM_Outptr_ void** ppvHeap);

    //DECLSPEC_XFGVIRT(ID3D12Device3, EnqueueMakeResident)
    //    HRESULT(STDMETHODCALLTYPE* EnqueueMakeResident)(
    //        ID3D12Device4* This,
    //        D3D12_RESIDENCY_FLAGS Flags,
    //        UINT NumObjects,
    //        _In_reads_(NumObjects) ID3D12Pageable *const * ppObjects,
    //        _In_  ID3D12Fence* pFenceToSignal,
    //        UINT64 FenceValueToSignal);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommandList1(uint nodeMask, D3D12_COMMAND_LIST_TYPE type, D3D12_COMMAND_LIST_FLAGS flags, in Guid riid, void** ppCommandList)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_COMMAND_LIST_TYPE, D3D12_COMMAND_LIST_FLAGS, in Guid, void**, HRESULT>)_vtbl[51])(Unsafe.AsPointer(ref this), nodeMask, type, flags, riid, ppCommandList);

    //DECLSPEC_XFGVIRT(ID3D12Device4, CreateProtectedResourceSession)
    //    HRESULT(STDMETHODCALLTYPE* CreateProtectedResourceSession)(
    //        ID3D12Device4* This,
    //        _In_  const D3D12_PROTECTED_RESOURCE_SESSION_DESC* pDesc,
    //        _In_  REFIID riid,
    //        _COM_Outptr_  void** ppSession);

    //DECLSPEC_XFGVIRT(ID3D12Device4, CreateCommittedResource1)
    //    HRESULT(STDMETHODCALLTYPE* CreateCommittedResource1)(
    //        ID3D12Device4* This,
    //        _In_  const D3D12_HEAP_PROPERTIES* pHeapProperties,
    //        D3D12_HEAP_FLAGS HeapFlags,
    //        _In_ const D3D12_RESOURCE_DESC* pDesc,
    //        D3D12_RESOURCE_STATES InitialResourceState,
    //        _In_opt_ const D3D12_CLEAR_VALUE* pOptimizedClearValue,
    //        _In_opt_  ID3D12ProtectedResourceSession* pProtectedSession,
    //        REFIID riidResource,
    //        _COM_Outptr_opt_ void** ppvResource);

    //DECLSPEC_XFGVIRT(ID3D12Device4, CreateHeap1)
    //    HRESULT(STDMETHODCALLTYPE* CreateHeap1)(
    //        ID3D12Device4* This,
    //        _In_  const D3D12_HEAP_DESC* pDesc,
    //        _In_opt_  ID3D12ProtectedResourceSession* pProtectedSession,
    //        REFIID riid,
    //        _COM_Outptr_opt_ void** ppvHeap);

    //DECLSPEC_XFGVIRT(ID3D12Device4, CreateReservedResource1)
    //    HRESULT(STDMETHODCALLTYPE* CreateReservedResource1)(
    //        ID3D12Device4* This,
    //        _In_  const D3D12_RESOURCE_DESC* pDesc,
    //        D3D12_RESOURCE_STATES InitialState,
    //        _In_opt_ const D3D12_CLEAR_VALUE* pOptimizedClearValue,
    //        _In_opt_  ID3D12ProtectedResourceSession* pProtectedSession,
    //        REFIID riid,
    //        _COM_Outptr_opt_ void** ppvResource);

    //DECLSPEC_XFGVIRT(ID3D12Device4, GetResourceAllocationInfo1)
    //    D3D12_RESOURCE_ALLOCATION_INFO* (STDMETHODCALLTYPE* GetResourceAllocationInfo1 ) (
    //        ID3D12Device4* This,
    //        D3D12_RESOURCE_ALLOCATION_INFO* RetVal,
    //        UINT visibleMask,
    //        UINT numResourceDescs,
    //        _In_reads_(numResourceDescs) const D3D12_RESOURCE_DESC* pResourceDescs,
    //        _Out_writes_opt_(numResourceDescs)  D3D12_RESOURCE_ALLOCATION_INFO1* pResourceAllocationInfo1);

}
