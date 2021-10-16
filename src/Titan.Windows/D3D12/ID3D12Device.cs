using System;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D12;

public unsafe struct ID3D12Device
{
    public static readonly Guid UUID = new(0x189819f1, 0x1db6, 0x4b57, 0xbe, 0x54, 0x18, 0x21, 0x33, 0x9b, 0x85, 0xf7);
    private void** _vtbl;

    //HRESULT(STDMETHODCALLTYPE* QueryInterface)(
    //     ID3D12Device* This,
    //     REFIID riid,
    //     _COM_Outptr_  void** ppvObject);

    //ULONG(STDMETHODCALLTYPE* AddRef)(
    // ID3D12Device* This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

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

    //HRESULT(STDMETHODCALLTYPE* SetName)(
    // ID3D12Device* This,
    // _In_z_ LPCWSTR Name);

    //UINT(STDMETHODCALLTYPE* GetNodeCount)(
    // ID3D12Device* This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)] 
    public HRESULT CreateCommandQueue(D3D12_COMMAND_QUEUE_DESC* pDesc, in Guid riid, void** ppCommandQueue) => ((delegate* unmanaged[Stdcall]<void*, D3D12_COMMAND_QUEUE_DESC*, in Guid, void**, HRESULT>)_vtbl[8])(Unsafe.AsPointer(ref this), pDesc, riid, ppCommandQueue);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE type, in Guid riid, void** ppCommandAllocator) => ((delegate* unmanaged[Stdcall]<void*, D3D12_COMMAND_LIST_TYPE, in Guid, void**, HRESULT>)_vtbl[9])(Unsafe.AsPointer(ref this), type, riid, ppCommandAllocator);
    
    //HRESULT(STDMETHODCALLTYPE* CreateGraphicsPipelineState)(
    // ID3D12Device* This,
    // _In_  const D3D12_GRAPHICS_PIPELINE_STATE_DESC* pDesc,
    // REFIID riid,
    //        _COM_Outptr_ void** ppPipelineState);

    //HRESULT(STDMETHODCALLTYPE* CreateComputePipelineState)(
    // ID3D12Device* This,
    // _In_  const D3D12_COMPUTE_PIPELINE_STATE_DESC* pDesc,
    // REFIID riid,
    //        _COM_Outptr_ void** ppPipelineState);

    //HRESULT(STDMETHODCALLTYPE* CreateCommandList)(
    // ID3D12Device* This,
    // _In_ UINT nodeMask,
    // _In_  D3D12_COMMAND_LIST_TYPE type,
    // _In_  ID3D12CommandAllocator* pCommandAllocator,
    // _In_opt_  ID3D12PipelineState* pInitialState,
    // REFIID riid,
    //        _COM_Outptr_ void** ppCommandList);

    //HRESULT(STDMETHODCALLTYPE* CheckFeatureSupport)(
    // ID3D12Device* This,
    // D3D12_FEATURE Feature,
    // _Inout_updates_bytes_(FeatureSupportDataSize) void* pFeatureSupportData,
    //UINT FeatureSupportDataSize);

    //    HRESULT(STDMETHODCALLTYPE* CreateDescriptorHeap)(
    //     ID3D12Device* This,
    //     _In_  const D3D12_DESCRIPTOR_HEAP_DESC* pDescriptorHeapDesc,
    //     REFIID riid,
    //        _COM_Outptr_ void** ppvHeap);

    //UINT(STDMETHODCALLTYPE* GetDescriptorHandleIncrementSize)(
    // ID3D12Device* This,
    // _In_ D3D12_DESCRIPTOR_HEAP_TYPE DescriptorHeapType);

    //HRESULT(STDMETHODCALLTYPE* CreateRootSignature)(
    // ID3D12Device* This,
    // _In_ UINT nodeMask,
    // _In_reads_(blobLengthInBytes)  const void* pBlobWithRootSignature,
    // _In_  SIZE_T blobLengthInBytes,
    // REFIID riid,
    //        _COM_Outptr_ void** ppvRootSignature);

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

    //void (STDMETHODCALLTYPE* CreateRenderTargetView ) (
    //    ID3D12Device* This,
    //    _In_opt_ ID3D12Resource * pResource,
    //    _In_opt_  const D3D12_RENDER_TARGET_VIEW_DESC* pDesc,
    //    _In_  D3D12_CPU_DESCRIPTOR_HANDLE DestDescriptor);

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

    //D3D12_HEAP_PROPERTIES(STDMETHODCALLTYPE* GetCustomHeapProperties)(
    // ID3D12Device* This,
    // _In_ UINT nodeMask,
    // D3D12_HEAP_TYPE heapType);

    //    HRESULT(STDMETHODCALLTYPE* CreateCommittedResource)(
    //     ID3D12Device* This,
    //     _In_  const D3D12_HEAP_PROPERTIES* pHeapProperties,
    //     D3D12_HEAP_FLAGS HeapFlags,
    //        _In_ const D3D12_RESOURCE_DESC* pDesc,
    //       D3D12_RESOURCE_STATES InitialResourceState,
    //        _In_opt_ const D3D12_CLEAR_VALUE* pOptimizedClearValue,
    //       REFIID riidResource,
    //        _COM_Outptr_opt_ void** ppvResource);

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
    public HRESULT CreateFence(ulong initialValue, D3D12_FENCE_FLAGS flags, in Guid riid, void** ppFence) => ((delegate* unmanaged[Stdcall]<void*, ulong, D3D12_FENCE_FLAGS, in Guid, void**, HRESULT>)_vtbl[36])(Unsafe.AsPointer(ref this), initialValue, flags, riid, ppFence);

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
