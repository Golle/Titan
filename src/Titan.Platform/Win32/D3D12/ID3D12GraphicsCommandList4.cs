using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32.D3D;
using Titan.Platform.Win32.DXGI;

namespace Titan.Platform.Win32.D3D12;

[Guid("8754318e-d3a9-4541-98cf-645b50dc4874")]
public unsafe struct ID3D12GraphicsCommandList4 : INativeGuid
{
    public static Guid* Guid => IID.IID_ID3D12GraphicsCommandList;
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetPrivateData(Guid* guid, uint* pDataSize, void* pData)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, uint*, void*, HRESULT>)_vtbl[3])(Unsafe.AsPointer(ref this), guid, pDataSize, pData);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetPrivateData(Guid* guid, uint DataSize, void* pData)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, uint, void*, HRESULT>)_vtbl[4])(Unsafe.AsPointer(ref this), guid, DataSize, pData);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetPrivateDataInterface(Guid* guid, IUnknown* pData)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, IUnknown*, HRESULT>)_vtbl[5])(Unsafe.AsPointer(ref this), guid, pData);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetName(char* Name)
        => ((delegate* unmanaged[Stdcall]<void*, char*, HRESULT>)_vtbl[6])(Unsafe.AsPointer(ref this), Name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetDevice(Guid* riid, void** ppvDevice)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[7])(Unsafe.AsPointer(ref this), riid, ppvDevice);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new D3D12_COMMAND_LIST_TYPE GetType()
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_COMMAND_LIST_TYPE>)_vtbl[8])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Close()
        => ((delegate* unmanaged[Stdcall]<void*, HRESULT>)_vtbl[9])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Reset(ID3D12CommandAllocator* pAllocator, ID3D12PipelineState* pInitialState)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12CommandAllocator*, ID3D12PipelineState*, HRESULT>)_vtbl[10])(Unsafe.AsPointer(ref this), pAllocator, pInitialState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearState(ID3D12PipelineState* pPipelineState)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12PipelineState*, void>)_vtbl[11])(Unsafe.AsPointer(ref this), pPipelineState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawInstanced(uint VertexCountPerInstance, uint InstanceCount, uint StartVertexLocation, uint StartInstanceLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, uint, void>)_vtbl[12])(Unsafe.AsPointer(ref this), VertexCountPerInstance, InstanceCount, StartVertexLocation, StartInstanceLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawIndexedInstanced(uint IndexCountPerInstance, uint InstanceCount, uint StartIndexLocation, int BaseVertexLocation, uint StartInstanceLocation)
    => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, int, uint, void>)_vtbl[13])(Unsafe.AsPointer(ref this), IndexCountPerInstance, InstanceCount, StartIndexLocation, BaseVertexLocation, StartInstanceLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispatch(uint ThreadGroupCountX, uint ThreadGroupCountY, uint ThreadGroupCountZ)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, void>)_vtbl[14])(Unsafe.AsPointer(ref this), ThreadGroupCountX, ThreadGroupCountY, ThreadGroupCountZ);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyBufferRegion(ID3D12Resource* pDstBuffer, ulong DstOffset, ID3D12Resource* pSrcBuffer, ulong SrcOffset, ulong NumBytes)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, ulong, ID3D12Resource*, ulong, ulong, void>)_vtbl[15])(Unsafe.AsPointer(ref this), pDstBuffer, DstOffset, pSrcBuffer, SrcOffset, NumBytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTextureRegion(D3D12_TEXTURE_COPY_LOCATION* pDst, uint DstX, uint DstY, uint DstZ, D3D12_TEXTURE_COPY_LOCATION* pSrc, D3D12_BOX* pSrcBox)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_TEXTURE_COPY_LOCATION*, uint, uint, uint, D3D12_TEXTURE_COPY_LOCATION*, D3D12_BOX*, void>)_vtbl[16])(Unsafe.AsPointer(ref this), pDst, DstX, DstY, DstZ, pSrc, pSrcBox);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyResource(ID3D12Resource* pDstResource, ID3D12Resource* pSrcResource)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, ID3D12Resource*, void>)_vtbl[17])(Unsafe.AsPointer(ref this), pDstResource, pSrcResource);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTiles(ID3D12Resource* pTiledResource, D3D12_TILED_RESOURCE_COORDINATE* pTileRegionStartCoordinate, D3D12_TILE_REGION_SIZE* pTileRegionSize, ID3D12Resource* pBuffer, ulong BufferStartOffsetInBytes, D3D12_TILE_COPY_FLAGS Flags)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, D3D12_TILED_RESOURCE_COORDINATE*, D3D12_TILE_REGION_SIZE*, ID3D12Resource*, ulong, D3D12_TILE_COPY_FLAGS, void>)_vtbl[18])(Unsafe.AsPointer(ref this), pTiledResource, pTileRegionStartCoordinate, pTileRegionSize, pBuffer, BufferStartOffsetInBytes, Flags);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResolveSubresource(ID3D12Resource* pDstResource, uint DstSubresource, ID3D12Resource* pSrcResource, uint SrcSubresource, DXGI_FORMAT Format)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, uint, ID3D12Resource*, uint, DXGI_FORMAT, void>)_vtbl[19])(Unsafe.AsPointer(ref this), pDstResource, DstSubresource, pSrcResource, SrcSubresource, Format);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IASetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY PrimitiveTopology)
        => ((delegate* unmanaged[Stdcall]<void*, D3D_PRIMITIVE_TOPOLOGY, void>)_vtbl[20])(Unsafe.AsPointer(ref this), PrimitiveTopology);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RSSetViewports(uint NumViewports, D3D12_VIEWPORT* pViewports)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_VIEWPORT*, void>)_vtbl[21])(Unsafe.AsPointer(ref this), NumViewports, pViewports);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RSSetScissorRects(uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_RECT*, void>)_vtbl[22])(Unsafe.AsPointer(ref this), NumRects, pRects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OMSetBlendFactor(float* BlendFactor /*FLOAT[4]*/)
        => ((delegate* unmanaged[Stdcall]<void*, float*, void>)_vtbl[23])(Unsafe.AsPointer(ref this), BlendFactor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OMSetStencilRef(uint StencilRef)
        => ((delegate* unmanaged[Stdcall]<void*, uint, void>)_vtbl[24])(Unsafe.AsPointer(ref this), StencilRef);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPipelineState(ID3D12PipelineState* pPipelineState)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12PipelineState*, void>)_vtbl[25])(Unsafe.AsPointer(ref this), pPipelineState);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResourceBarrier(uint NumBarriers, D3D12_RESOURCE_BARRIER* pBarriers)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_RESOURCE_BARRIER*, void>)_vtbl[26])(Unsafe.AsPointer(ref this), NumBarriers, pBarriers);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExecuteBundle(ID3D12GraphicsCommandList* pCommandList)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12GraphicsCommandList*, void>)_vtbl[27])(Unsafe.AsPointer(ref this), pCommandList);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetDescriptorHeaps(uint NumDescriptorHeaps, ID3D12DescriptorHeap** ppDescriptorHeaps)
        => ((delegate* unmanaged[Stdcall]<void*, uint, ID3D12DescriptorHeap**, void>)_vtbl[28])(Unsafe.AsPointer(ref this), NumDescriptorHeaps, ppDescriptorHeaps);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRootSignature(ID3D12RootSignature* pRootSignature)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12RootSignature*, void>)_vtbl[29])(Unsafe.AsPointer(ref this), pRootSignature);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRootSignature(ID3D12RootSignature* pRootSignature)
     => ((delegate* unmanaged[Stdcall]<void*, ID3D12RootSignature*, void>)_vtbl[30])(Unsafe.AsPointer(ref this), pRootSignature);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRootDescriptorTable(uint RootParameterIndex, D3D12_GPU_DESCRIPTOR_HANDLE BaseDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_DESCRIPTOR_HANDLE, void>)_vtbl[31])(Unsafe.AsPointer(ref this), RootParameterIndex, BaseDescriptor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRootDescriptorTable(uint RootParameterIndex, D3D12_GPU_DESCRIPTOR_HANDLE BaseDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_DESCRIPTOR_HANDLE, void>)_vtbl[32])(Unsafe.AsPointer(ref this), RootParameterIndex, BaseDescriptor);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRoot32BitConstant(uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, void>)_vtbl[33])(Unsafe.AsPointer(ref this), RootParameterIndex, SrcData, DestOffsetIn32BitValues);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRoot32BitConstant(uint RootParameterIndex, uint SrcData, uint DestOffsetIn32BitValues)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, uint, void>)_vtbl[34])(Unsafe.AsPointer(ref this), RootParameterIndex, SrcData, DestOffsetIn32BitValues);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRoot32BitConstants(uint RootParameterIndex, uint Num32BitValuesToSet, void* pSrcData, uint DestOffsetIn32BitValues)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, void*, uint, void>)_vtbl[35])(Unsafe.AsPointer(ref this), RootParameterIndex, Num32BitValuesToSet, pSrcData, DestOffsetIn32BitValues);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRoot32BitConstants(uint RootParameterIndex, uint Num32BitValuesToSet, void* pSrcData, uint DestOffsetIn32BitValues)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, void*, uint, void>)_vtbl[36])(Unsafe.AsPointer(ref this), RootParameterIndex, Num32BitValuesToSet, pSrcData, DestOffsetIn32BitValues);

    public void SetComputeRootConstantBufferView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[37])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRootConstantBufferView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[38])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRootShaderResourceView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[39])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRootShaderResourceView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[40])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetComputeRootUnorderedAccessView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[41])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetGraphicsRootUnorderedAccessView(uint RootParameterIndex, D3D12_GPU_VIRTUAL_ADDRESS BufferLocation)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_GPU_VIRTUAL_ADDRESS, void>)_vtbl[42])(Unsafe.AsPointer(ref this), RootParameterIndex, BufferLocation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IASetIndexBuffer(D3D12_INDEX_BUFFER_VIEW* pView)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_INDEX_BUFFER_VIEW*, void>)_vtbl[43])(Unsafe.AsPointer(ref this), pView);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void IASetVertexBuffers(uint StartSlot, uint NumViews, D3D12_VERTEX_BUFFER_VIEW* pViews)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, D3D12_VERTEX_BUFFER_VIEW*, void>)_vtbl[44])(Unsafe.AsPointer(ref this), StartSlot, NumViews, pViews);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SOSetTargets(uint StartSlot, uint NumViews, D3D12_STREAM_OUTPUT_BUFFER_VIEW* pViews)
        => ((delegate* unmanaged[Stdcall]<void*, uint, uint, D3D12_STREAM_OUTPUT_BUFFER_VIEW*, void>)_vtbl[45])(Unsafe.AsPointer(ref this), StartSlot, NumViews, pViews);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OMSetRenderTargets(uint NumRenderTargetDescriptors, D3D12_CPU_DESCRIPTOR_HANDLE* pRenderTargetDescriptors, int RTsSingleHandleToDescriptorRange, D3D12_CPU_DESCRIPTOR_HANDLE* pDepthStencilDescriptor)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_CPU_DESCRIPTOR_HANDLE*, int, D3D12_CPU_DESCRIPTOR_HANDLE*, void>)_vtbl[46])(Unsafe.AsPointer(ref this), NumRenderTargetDescriptors, pRenderTargetDescriptors, RTsSingleHandleToDescriptorRange, pDepthStencilDescriptor);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearDepthStencilView(D3D12_CPU_DESCRIPTOR_HANDLE DepthStencilView, D3D12_CLEAR_FLAGS ClearFlags, float Depth, byte Stencil, uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_CPU_DESCRIPTOR_HANDLE, D3D12_CLEAR_FLAGS, float, byte, uint, D3D12_RECT*, void>)_vtbl[47])(Unsafe.AsPointer(ref this), DepthStencilView, ClearFlags, Depth, Stencil, NumRects, pRects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearRenderTargetView(D3D12_CPU_DESCRIPTOR_HANDLE RenderTargetView, float* ColorRGBA, uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_CPU_DESCRIPTOR_HANDLE, float*, uint, D3D12_RECT*, void>)_vtbl[48])(Unsafe.AsPointer(ref this), RenderTargetView, ColorRGBA, NumRects, pRects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearUnorderedAccessViewUint(D3D12_GPU_DESCRIPTOR_HANDLE ViewGPUHandleInCurrentHeap, D3D12_CPU_DESCRIPTOR_HANDLE ViewCPUHandle, ID3D12Resource* pResource, uint* Values, uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_GPU_DESCRIPTOR_HANDLE, D3D12_CPU_DESCRIPTOR_HANDLE, ID3D12Resource*, uint*, uint, D3D12_RECT*, void>)_vtbl[49])(Unsafe.AsPointer(ref this), ViewGPUHandleInCurrentHeap, ViewCPUHandle, pResource, Values, NumRects, pRects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ClearUnorderedAccessViewFloat(D3D12_GPU_DESCRIPTOR_HANDLE ViewGPUHandleInCurrentHeap, D3D12_CPU_DESCRIPTOR_HANDLE ViewCPUHandle, ID3D12Resource* pResource, float* Values, uint NumRects, D3D12_RECT* pRects)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_GPU_DESCRIPTOR_HANDLE, D3D12_CPU_DESCRIPTOR_HANDLE, ID3D12Resource*, float*, uint, D3D12_RECT*, void>)_vtbl[50])(Unsafe.AsPointer(ref this), ViewGPUHandleInCurrentHeap, ViewCPUHandle, pResource, Values, NumRects, pRects);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DiscardResource(ID3D12Resource* pResource, D3D12_DISCARD_REGION* pRegion)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, D3D12_DISCARD_REGION*, void>)_vtbl[51])(Unsafe.AsPointer(ref this), pResource, pRegion);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BeginQuery(ID3D12QueryHeap* pQueryHeap, D3D12_QUERY_TYPE Type, uint Index)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12QueryHeap*, D3D12_QUERY_TYPE, uint, void>)_vtbl[52])(Unsafe.AsPointer(ref this), pQueryHeap, Type, Index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndQuery(ID3D12QueryHeap* pQueryHeap, D3D12_QUERY_TYPE Type, uint Index)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12QueryHeap*, D3D12_QUERY_TYPE, uint, void>)_vtbl[53])(Unsafe.AsPointer(ref this), pQueryHeap, Type, Index);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResolveQueryData(ID3D12QueryHeap* pQueryHeap, D3D12_QUERY_TYPE Type, uint StartIndex, uint NumQueries, ID3D12Resource* pDestinationBuffer, ulong AlignedDestinationBufferOffset)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12QueryHeap*, D3D12_QUERY_TYPE, uint, uint, ID3D12Resource*, ulong, void>)_vtbl[54])(Unsafe.AsPointer(ref this), pQueryHeap, Type, StartIndex, NumQueries, pDestinationBuffer, AlignedDestinationBufferOffset);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPredication(ID3D12Resource* pBuffer, ulong AlignedBufferOffset, D3D12_PREDICATION_OP Operation)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12Resource*, ulong, D3D12_PREDICATION_OP, void>)_vtbl[55])(Unsafe.AsPointer(ref this), pBuffer, AlignedBufferOffset, Operation);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetMarker(uint Metadata, void* pData, uint Size)
        => ((delegate* unmanaged[Stdcall]<void*, uint, void*, uint, void>)_vtbl[56])(Unsafe.AsPointer(ref this), Metadata, pData, Size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BeginEvent(uint Metadata, void* pData, uint Size)
        => ((delegate* unmanaged[Stdcall]<void*, uint, void*, uint, void>)_vtbl[57])(Unsafe.AsPointer(ref this), Metadata, pData, Size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndEvent()
        => ((delegate* unmanaged[Stdcall]<void*, void>)_vtbl[58])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ExecuteIndirect(ID3D12CommandSignature* pCommandSignature, uint MaxCommandCount, ID3D12Resource* pArgumentBuffer, ulong ArgumentBufferOffset, ID3D12Resource* pCountBuffer, ulong CountBufferOffset)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12CommandSignature*, uint, ID3D12Resource*, ulong, ID3D12Resource*, ulong, void>)_vtbl[59])(Unsafe.AsPointer(ref this), pCommandSignature, MaxCommandCount, pArgumentBuffer, ArgumentBufferOffset, pCountBuffer, CountBufferOffset);

    //    void (STDMETHODCALLTYPE* AtomicCopyBufferUINT ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_ ID3D12Resource * pDstBuffer,
    //        UINT64 DstOffset,
    //        _In_ ID3D12Resource *pSrcBuffer,
    //        UINT64 SrcOffset,
    //        UINT Dependencies,
    //        _In_reads_(Dependencies)  ID3D12Resource* const * ppDependentResources,
    //        _In_reads_(Dependencies)  const D3D12_SUBRESOURCE_RANGE_UINT64* pDependentSubresourceRanges);

    //    void (STDMETHODCALLTYPE* AtomicCopyBufferUINT64 ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_ ID3D12Resource * pDstBuffer,
    //        UINT64 DstOffset,
    //        _In_ ID3D12Resource *pSrcBuffer,
    //        UINT64 SrcOffset,
    //        UINT Dependencies,
    //        _In_reads_(Dependencies)  ID3D12Resource* const * ppDependentResources,
    //        _In_reads_(Dependencies)  const D3D12_SUBRESOURCE_RANGE_UINT64* pDependentSubresourceRanges);

    //    void (STDMETHODCALLTYPE* OMSetDepthBounds ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_ FLOAT Min,
    //        _In_  FLOAT Max);

    //    void (STDMETHODCALLTYPE* SetSamplePositions ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_ UINT NumSamplesPerPixel,
    //        _In_  UINT NumPixels,
    //        _In_reads_(NumSamplesPerPixel * NumPixels)  D3D12_SAMPLE_POSITION* pSamplePositions);

    //    void (STDMETHODCALLTYPE* ResolveSubresourceRegion ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_ ID3D12Resource * pDstResource,
    //        _In_  UINT DstSubresource,
    //        _In_  UINT DstX,
    //        _In_  UINT DstY,
    //        _In_  ID3D12Resource* pSrcResource,
    //        _In_  UINT SrcSubresource,
    //        _In_opt_  D3D12_RECT* pSrcRect,
    //        _In_  DXGI_FORMAT Format,
    //        _In_  D3D12_RESOLVE_MODE ResolveMode);

    //    void (STDMETHODCALLTYPE* SetViewInstanceMask ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_ UINT Mask);

    //    void (STDMETHODCALLTYPE* WriteBufferImmediate ) (
    //        ID3D12GraphicsCommandList4* This,
    //        UINT Count,
    //        _In_reads_(Count) const D3D12_WRITEBUFFERIMMEDIATE_PARAMETER* pParams,
    //        _In_reads_opt_(Count)  const D3D12_WRITEBUFFERIMMEDIATE_MODE* pModes);

    //    void (STDMETHODCALLTYPE* SetProtectedResourceSession ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_opt_ ID3D12ProtectedResourceSession * pProtectedResourceSession);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BeginRenderPass(uint NumRenderTargets, D3D12_RENDER_PASS_RENDER_TARGET_DESC* pRenderTargets, D3D12_RENDER_PASS_DEPTH_STENCIL_DESC* pDepthStencil, D3D12_RENDER_PASS_FLAGS Flags)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_RENDER_PASS_RENDER_TARGET_DESC*, D3D12_RENDER_PASS_DEPTH_STENCIL_DESC*, D3D12_RENDER_PASS_FLAGS, void>)_vtbl[68])(Unsafe.AsPointer(ref this), NumRenderTargets, pRenderTargets, pDepthStencil, Flags);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndRenderPass()
        => ((delegate* unmanaged[Stdcall]<void*, void>)_vtbl[69])(Unsafe.AsPointer(ref this));

    //    void (STDMETHODCALLTYPE* InitializeMetaCommand ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_ ID3D12MetaCommand * pMetaCommand,
    //        _In_reads_bytes_opt_(InitializationParametersDataSizeInBytes)  const void* pInitializationParametersData,
    //        _In_  SIZE_T InitializationParametersDataSizeInBytes);

    //    void (STDMETHODCALLTYPE* ExecuteMetaCommand ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_ ID3D12MetaCommand * pMetaCommand,
    //        _In_reads_bytes_opt_(ExecutionParametersDataSizeInBytes)  const void* pExecutionParametersData,
    //        _In_  SIZE_T ExecutionParametersDataSizeInBytes);

    //    void (STDMETHODCALLTYPE* BuildRaytracingAccelerationStructure ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_  const D3D12_BUILD_RAYTRACING_ACCELERATION_STRUCTURE_DESC* pDesc,
    //        _In_  UINT NumPostbuildInfoDescs,
    //        _In_reads_opt_(NumPostbuildInfoDescs)  const D3D12_RAYTRACING_ACCELERATION_STRUCTURE_POSTBUILD_INFO_DESC* pPostbuildInfoDescs);

    //    void (STDMETHODCALLTYPE* EmitRaytracingAccelerationStructurePostbuildInfo ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_  const D3D12_RAYTRACING_ACCELERATION_STRUCTURE_POSTBUILD_INFO_DESC* pDesc,
    //        _In_  UINT NumSourceAccelerationStructures,
    //        _In_reads_( NumSourceAccelerationStructures )  const D3D12_GPU_VIRTUAL_ADDRESS* pSourceAccelerationStructureData);

    //    void (STDMETHODCALLTYPE* CopyRaytracingAccelerationStructure ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_ D3D12_GPU_VIRTUAL_ADDRESS DestAccelerationStructureData,
    //        _In_  D3D12_GPU_VIRTUAL_ADDRESS SourceAccelerationStructureData,
    //        _In_  D3D12_RAYTRACING_ACCELERATION_STRUCTURE_COPY_MODE Mode);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPipelineState1(ID3D12StateObject* pStateObject)
        => ((delegate* unmanaged[Stdcall]<void*, ID3D12StateObject*, void>)_vtbl[75])(Unsafe.AsPointer(ref this), pStateObject);

    //    void (STDMETHODCALLTYPE* DispatchRays ) (
    //        ID3D12GraphicsCommandList4* This,
    //        _In_  const D3D12_DISPATCH_RAYS_DESC* pDesc);
}
