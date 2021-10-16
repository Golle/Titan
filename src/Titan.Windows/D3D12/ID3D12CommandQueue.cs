using System;
using System.Runtime.CompilerServices;
// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D12;
public unsafe struct ID3D12CommandQueue
{
    public static readonly Guid UUID = new("0ec870a6-5d7e-4c22-8cfc-5baae07616ed");
    private void** _vtbl;

    //HRESULT(STDMETHODCALLTYPE* QueryInterface)(
    //     ID3D12CommandQueue* This,
    //     REFIID riid,
    //     _COM_Outptr_  void** ppvObject);

    //ULONG(STDMETHODCALLTYPE* AddRef)(
    // ID3D12CommandQueue* This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //    HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    //     ID3D12CommandQueue* This,
    //     _In_ REFGUID guid,
    //     _Inout_  UINT* pDataSize,
    //     _Out_writes_bytes_opt_( *pDataSize )  void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    // ID3D12CommandQueue* This,
    // _In_ REFGUID guid,
    // _In_  UINT DataSize,
    // _In_reads_bytes_opt_( DataSize )  const void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    // ID3D12CommandQueue* This,
    // _In_ REFGUID guid,
    // _In_opt_  const IUnknown* pData);

    //HRESULT(STDMETHODCALLTYPE* SetName)(
    // ID3D12CommandQueue* This,
    // _In_z_ LPCWSTR Name);

    //HRESULT(STDMETHODCALLTYPE* GetDevice)(
    // ID3D12CommandQueue* This,
    // REFIID riid,
    // _COM_Outptr_opt_  void** ppvDevice);

    //void (STDMETHODCALLTYPE* UpdateTileMappings ) (
    //    ID3D12CommandQueue* This,
    //    _In_ ID3D12Resource * pResource,
    //    UINT NumResourceRegions,
    //        _In_reads_opt_(NumResourceRegions)  const D3D12_TILED_RESOURCE_COORDINATE* pResourceRegionStartCoordinates,
    //        _In_reads_opt_(NumResourceRegions)  const D3D12_TILE_REGION_SIZE* pResourceRegionSizes,
    //        _In_opt_  ID3D12Heap* pHeap,
    //        UINT NumRanges,
    //        _In_reads_opt_(NumRanges)  const D3D12_TILE_RANGE_FLAGS* pRangeFlags,
    //        _In_reads_opt_(NumRanges)  const UINT* pHeapRangeStartOffsets,
    //        _In_reads_opt_(NumRanges)  const UINT* pRangeTileCounts,
    //        D3D12_TILE_MAPPING_FLAGS Flags);

    //    void (STDMETHODCALLTYPE* CopyTileMappings ) (
    //        ID3D12CommandQueue* This,
    //        _In_ ID3D12Resource * pDstResource,
    //        _In_  const D3D12_TILED_RESOURCE_COORDINATE* pDstRegionStartCoordinate,
    //        _In_  ID3D12Resource* pSrcResource,
    //        _In_  const D3D12_TILED_RESOURCE_COORDINATE* pSrcRegionStartCoordinate,
    //        _In_  const D3D12_TILE_REGION_SIZE* pRegionSize,
    //        D3D12_TILE_MAPPING_FLAGS Flags);

    //    void (STDMETHODCALLTYPE* ExecuteCommandLists ) (
    //        ID3D12CommandQueue* This,
    //        _In_ UINT NumCommandLists,
    //        _In_reads_(NumCommandLists)  ID3D12CommandList* const * ppCommandLists);

    //void (STDMETHODCALLTYPE* SetMarker ) (
    //    ID3D12CommandQueue* This,
    //    UINT Metadata,
    //    _In_reads_bytes_opt_(Size) const void* pData,
    //   UINT Size);

    //    void (STDMETHODCALLTYPE* BeginEvent ) (
    //        ID3D12CommandQueue* This,
    //        UINT Metadata,
    //        _In_reads_bytes_opt_(Size) const void* pData,
    //       UINT Size);

    //    void (STDMETHODCALLTYPE* EndEvent ) (
    //        ID3D12CommandQueue* This);

    //    HRESULT(STDMETHODCALLTYPE* Signal)(
    //     ID3D12CommandQueue* This,
    //     ID3D12Fence* pFence,
    //     UINT64 Value);

    //    HRESULT(STDMETHODCALLTYPE* Wait)(
    //     ID3D12CommandQueue* This,
    //     ID3D12Fence* pFence,
    //     UINT64 Value);

    //    HRESULT(STDMETHODCALLTYPE* GetTimestampFrequency)(
    //     ID3D12CommandQueue* This,
    //     _Out_ UINT64 * pFrequency);

    //HRESULT(STDMETHODCALLTYPE* GetClockCalibration)(
    // ID3D12CommandQueue* This,
    // _Out_ UINT64 * pGpuTimestamp,
    // _Out_  UINT64* pCpuTimestamp);

    //D3D12_COMMAND_QUEUE_DESC(STDMETHODCALLTYPE* GetDesc)(
    // ID3D12CommandQueue* This);
}
