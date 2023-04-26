using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace Titan.Platform.Win32.D3D12;

public unsafe interface INativeGuid
{
    static abstract Guid* Guid { get; }
}

[Guid("e865df17-a9ee-46f9-a463-3098315aa2e5")]
public unsafe struct ID3D12Device4 : INativeGuid
{
    public static Guid* Guid => IID.IID_ID3D12Device7;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CreateSampler(D3D12_SAMPLER_DESC* pDesc, D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_SAMPLER_DESC*, D3D12_CPU_DESCRIPTOR_HANDLE, void>)_vtbl[22])(Unsafe.AsPointer(ref this), pDesc, DestDescriptor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyDescriptors(uint NumDestDescriptorRanges, D3D12_CPU_DESCRIPTOR_HANDLE* pDestDescriptorRangeStarts, uint* pDestDescriptorRangeSizes, uint NumSrcDescriptorRanges, D3D12_CPU_DESCRIPTOR_HANDLE* pSrcDescriptorRangeStarts, uint* pSrcDescriptorRangeSizes, D3D12_DESCRIPTOR_HEAP_TYPE DescriptorHeapsType)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_CPU_DESCRIPTOR_HANDLE*, uint*, uint, D3D12_CPU_DESCRIPTOR_HANDLE*, uint*, D3D12_DESCRIPTOR_HEAP_TYPE, void>)_vtbl[23])(Unsafe.AsPointer(ref this), NumDestDescriptorRanges, pDestDescriptorRangeStarts, pDestDescriptorRangeSizes, NumSrcDescriptorRanges, pSrcDescriptorRangeStarts, pSrcDescriptorRangeSizes, DescriptorHeapsType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyDescriptorsSimple(uint NumDescriptors, D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptorRangeStart, D3D12_CPU_DESCRIPTOR_HANDLE SrcDescriptorRangeStart, D3D12_DESCRIPTOR_HEAP_TYPE DescriptorHeapsType)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_CPU_DESCRIPTOR_HANDLE, D3D12_CPU_DESCRIPTOR_HANDLE, D3D12_DESCRIPTOR_HEAP_TYPE, void>)_vtbl[24])(Unsafe.AsPointer(ref this), NumDescriptors, DestDescriptorRangeStart, SrcDescriptorRangeStart, DescriptorHeapsType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public D3D12_RESOURCE_ALLOCATION_INFO GetResourceAllocationInfo(uint visibleMask, uint numResourceDescs, D3D12_RESOURCE_DESC* pResourceDescs)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, D3D12_RESOURCE_DESC*, D3D12_RESOURCE_ALLOCATION_INFO>)_vtbl[25])(Unsafe.AsPointer(ref this), visibleMask, numResourceDescs, pResourceDescs);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public D3D12_HEAP_PROPERTIES* GetCustomHeapProperties(uint nodeMask, D3D12_HEAP_TYPE heapType, D3D12_HEAP_PROPERTIES* retVal)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_HEAP_PROPERTIES*, uint, D3D12_HEAP_TYPE, D3D12_HEAP_PROPERTIES*>)_vtbl[26])(Unsafe.AsPointer(ref this), retVal, nodeMask, heapType);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommittedResource(D3D12_HEAP_PROPERTIES* pHeapProperties, D3D12_HEAP_FLAGS HeapFlags, D3D12_RESOURCE_DESC* pDesc, D3D12_RESOURCE_STATES InitialResourceState, D3D12_CLEAR_VALUE* pOptimizedClearValue, Guid* riidResource, void** ppvResource)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_HEAP_PROPERTIES*, D3D12_HEAP_FLAGS, D3D12_RESOURCE_DESC*, D3D12_RESOURCE_STATES, D3D12_CLEAR_VALUE*, Guid*, void**, HRESULT>)_vtbl[27])(Unsafe.AsPointer(ref this), pHeapProperties, HeapFlags, pDesc, InitialResourceState, pOptimizedClearValue, riidResource, ppvResource);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateHeap(D3D12_HEAP_DESC* pDesc, Guid* riid, void** ppvHeap)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_HEAP_DESC*, Guid*, void**, HRESULT>)_vtbl[28])(Unsafe.AsPointer(ref this), pDesc, riid, ppvHeap);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreatePlacedResource(ID3D12Heap* pHeap, ulong HeapOffset, D3D12_RESOURCE_DESC* pDesc, D3D12_RESOURCE_STATES InitialState, D3D12_CLEAR_VALUE* pOptimizedClearValue, Guid* riid, void** ppvResource)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Heap*, ulong, D3D12_RESOURCE_DESC*, D3D12_RESOURCE_STATES, D3D12_CLEAR_VALUE*, Guid*, void**, HRESULT>)_vtbl[29])(Unsafe.AsPointer(ref this), pHeap, HeapOffset, pDesc, InitialState, pOptimizedClearValue, riid, ppvResource);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateReservedResource(D3D12_RESOURCE_DESC* pDesc, D3D12_RESOURCE_STATES InitialState, D3D12_CLEAR_VALUE* pOptimizedClearValue, Guid* riid, void** ppvResource)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_RESOURCE_DESC*, D3D12_RESOURCE_STATES, D3D12_CLEAR_VALUE*, Guid*, void**, HRESULT>)_vtbl[30])(Unsafe.AsPointer(ref this), pDesc, InitialState, pOptimizedClearValue, riid, ppvResource);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateSharedHandle(ID3D12DeviceChild* pObject, SECURITY_ATTRIBUTES* pAttributes, uint Access, char* Name, HANDLE* pHandle)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12DeviceChild*, SECURITY_ATTRIBUTES*, uint, char*, HANDLE*, HRESULT>)_vtbl[31])(Unsafe.AsPointer(ref this), pObject, pAttributes, Access, Name, pHandle);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT MakeResident(uint NumObjects, ID3D12Pageable** ppObjects)
        => ((delegate* unmanaged[Stdcall]<void*, uint, ID3D12Pageable**, HRESULT>)_vtbl[34])(Unsafe.AsPointer(ref this), NumObjects, ppObjects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Evict(uint NumObjects, ID3D12Pageable** ppObjects)
        => ((delegate* unmanaged[Stdcall]<void*, uint, ID3D12Pageable**, HRESULT>)_vtbl[35])(Unsafe.AsPointer(ref this), NumObjects, ppObjects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateFence(ulong initialValue, D3D12_FENCE_FLAGS flags, Guid* riid, void** ppFence)
        => ((delegate* unmanaged[Stdcall]<void*, ulong, D3D12_FENCE_FLAGS, Guid*, void**, HRESULT>)_vtbl[36])(Unsafe.AsPointer(ref this), initialValue, flags, riid, ppFence);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetDeviceRemovedReason()
        => ((delegate* unmanaged[Stdcall]<void*, HRESULT>)_vtbl[37])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void GetCopyableFootprints(D3D12_RESOURCE_DESC* pResourceDesc, uint FirstSubresource, uint NumSubresources, ulong BaseOffset, D3D12_PLACED_SUBRESOURCE_FOOTPRINT* pLayouts, uint* pNumRows, ulong* pRowSizeInBytes, ulong* pTotalBytes)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_RESOURCE_DESC*, uint, uint, ulong, D3D12_PLACED_SUBRESOURCE_FOOTPRINT*, uint*, ulong*, ulong*, void>)_vtbl[38])(Unsafe.AsPointer(ref this), pResourceDesc, FirstSubresource, NumSubresources, BaseOffset, pLayouts, pNumRows, pRowSizeInBytes, pTotalBytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateQueryHeap(D3D12_QUERY_HEAP_DESC* pDesc, Guid* riid, void** ppvHeap)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_QUERY_HEAP_DESC*, Guid*, void**, HRESULT>)_vtbl[39])(Unsafe.AsPointer(ref this), pDesc, riid, ppvHeap);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetStablePowerState(int Enable)
        => ((delegate* unmanaged[Stdcall]<void*, int, HRESULT>)_vtbl[40])(Unsafe.AsPointer(ref this), Enable);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommandSignature(D3D12_COMMAND_SIGNATURE_DESC* pDesc, ID3D12RootSignature* pRootSignature, Guid* riid, void** ppvCommandSignature)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_COMMAND_SIGNATURE_DESC*, ID3D12RootSignature*, Guid*, void**, HRESULT>)_vtbl[41])(Unsafe.AsPointer(ref this), pDesc, pRootSignature, riid, ppvCommandSignature);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreatePipelineState(D3D12_PIPELINE_STATE_STREAM_DESC* pDesc, Guid* riid, void** ppPipelineState)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_PIPELINE_STATE_STREAM_DESC*, Guid*, void**, HRESULT>)_vtbl[47])(Unsafe.AsPointer(ref this), pDesc, riid, ppPipelineState);

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT EnqueueMakeResident(D3D12_RESIDENCY_FLAGS Flags, uint NumObjects, ID3D12Pageable** ppObjects, ID3D12Fence* pFenceToSignal, ulong FenceValueToSignal)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_RESIDENCY_FLAGS, uint, ID3D12Pageable**, ID3D12Fence*, ulong, HRESULT>)_vtbl[50])(Unsafe.AsPointer(ref this), Flags, NumObjects, ppObjects, pFenceToSignal, FenceValueToSignal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommandList1(uint nodeMask, D3D12_COMMAND_LIST_TYPE type, D3D12_COMMAND_LIST_FLAGS flags, Guid* riid, void** ppCommandList)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_COMMAND_LIST_TYPE, D3D12_COMMAND_LIST_FLAGS, Guid*, void**, HRESULT>)_vtbl[51])(Unsafe.AsPointer(ref this), nodeMask, type, flags, riid, ppCommandList);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateProtectedResourceSession(D3D12_PROTECTED_RESOURCE_SESSION_DESC* pDesc, Guid* riid, void** ppSession)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_PROTECTED_RESOURCE_SESSION_DESC*, Guid*, void**, HRESULT>)_vtbl[52])(Unsafe.AsPointer(ref this), pDesc, riid, ppSession);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommittedResource1(D3D12_HEAP_PROPERTIES* pHeapProperties, D3D12_HEAP_FLAGS HeapFlags, D3D12_RESOURCE_DESC* pDesc, D3D12_RESOURCE_STATES InitialResourceState, D3D12_CLEAR_VALUE* pOptimizedClearValue, ID3D12ProtectedResourceSession* pProtectedSession, Guid* riidResource, void** ppvResource)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_HEAP_PROPERTIES*, D3D12_HEAP_FLAGS, D3D12_RESOURCE_DESC*, D3D12_RESOURCE_STATES, D3D12_CLEAR_VALUE*, ID3D12ProtectedResourceSession*, Guid*, void**, HRESULT>)_vtbl[53])(Unsafe.AsPointer(ref this), pHeapProperties, HeapFlags, pDesc, InitialResourceState, pOptimizedClearValue, pProtectedSession, riidResource, ppvResource);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateHeap1(D3D12_HEAP_DESC* pDesc, ID3D12ProtectedResourceSession* pProtectedSession, Guid* riid, void** ppvHeap)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_HEAP_DESC*, ID3D12ProtectedResourceSession*, Guid*, void**, HRESULT>)_vtbl[54])(Unsafe.AsPointer(ref this), pDesc, pProtectedSession, riid, ppvHeap);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateReservedResource1(D3D12_RESOURCE_DESC* pDesc, D3D12_RESOURCE_STATES InitialState, D3D12_CLEAR_VALUE* pOptimizedClearValue, ID3D12ProtectedResourceSession* pProtectedSession, Guid* riid, void** ppvResource)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_RESOURCE_DESC*, D3D12_RESOURCE_STATES, D3D12_CLEAR_VALUE*, ID3D12ProtectedResourceSession*, Guid*, void**, HRESULT>)_vtbl[55])(Unsafe.AsPointer(ref this), pDesc, InitialState, pOptimizedClearValue, pProtectedSession, riid, ppvResource);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public D3D12_RESOURCE_ALLOCATION_INFO GetResourceAllocationInfo1(D3D12_RESOURCE_ALLOCATION_INFO* RetVal, uint visibleMask, uint numResourceDescs, D3D12_RESOURCE_DESC* pResourceDescs, D3D12_RESOURCE_ALLOCATION_INFO1* pResourceAllocationInfo1)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_RESOURCE_ALLOCATION_INFO*, uint, uint, D3D12_RESOURCE_DESC*, D3D12_RESOURCE_ALLOCATION_INFO1*, D3D12_RESOURCE_ALLOCATION_INFO>)_vtbl[54])(Unsafe.AsPointer(ref this), RetVal, visibleMask, numResourceDescs, pResourceDescs, pResourceAllocationInfo1);
}
