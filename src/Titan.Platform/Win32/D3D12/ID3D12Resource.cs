using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

[Guid("696442be-a72e-4059-bc79-5b5c98040fad")]
public unsafe struct ID3D12Resource : INativeGuid
{
    public static Guid* Guid => IID.IID_ID3D12Resource;

    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //    HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    //     ID3D12Resource* This,
    //     _In_ REFGUID guid,
    //     _Inout_  UINT* pDataSize,
    //     _Out_writes_bytes_opt_( *pDataSize )  void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    // ID3D12Resource* This,
    // _In_ REFGUID guid,
    // _In_  UINT DataSize,
    // _In_reads_bytes_opt_( DataSize )  const void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    // ID3D12Resource* This,
    // _In_ REFGUID guid,
    // _In_opt_  const IUnknown* pData);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetName(char* Name)
        => ((delegate* unmanaged[Stdcall]<void*, char*, HRESULT>)_vtbl[6])(Unsafe.AsPointer(ref this), Name);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetDevice(Guid* riid, void** ppvDevice)
    => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[7])(Unsafe.AsPointer(ref this), riid, ppvDevice);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Map(uint Subresource, D3D12_RANGE* pReadRange, void** ppData)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_RANGE*, void**, HRESULT>)_vtbl[8])(Unsafe.AsPointer(ref this), Subresource, pReadRange, ppData);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Unmap(uint Subresource, D3D12_RANGE* pWrittenRange)
        => ((delegate* unmanaged[Stdcall]<void*, uint, D3D12_RANGE*, void>)_vtbl[9])(Unsafe.AsPointer(ref this), Subresource, pWrittenRange);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public D3D12_RESOURCE_DESC* GetDesc(D3D12_RESOURCE_DESC* RetVal)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_RESOURCE_DESC*, D3D12_RESOURCE_DESC*>)_vtbl[10])(Unsafe.AsPointer(ref this), RetVal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public D3D12_GPU_VIRTUAL_ADDRESS GetGPUVirtualAddress()
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_GPU_VIRTUAL_ADDRESS>)_vtbl[11])(Unsafe.AsPointer(ref this));

    //    HRESULT(STDMETHODCALLTYPE* WriteToSubresource)(
    //     ID3D12Resource* This,
    //     UINT DstSubresource,
    //     _In_opt_  const D3D12_BOX* pDstBox,
    //     _In_  const void* pSrcData,
    //     UINT SrcRowPitch,
    //        UINT SrcDepthPitch);

    //HRESULT(STDMETHODCALLTYPE* ReadFromSubresource)(
    // ID3D12Resource* This,
    // _Out_  void* pDstData,
    // UINT DstRowPitch,
    //        UINT DstDepthPitch,
    //        UINT SrcSubresource,
    //        _In_opt_ const D3D12_BOX* pSrcBox);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetHeapProperties(D3D12_HEAP_PROPERTIES* pHeapProperties, D3D12_HEAP_FLAGS* pHeapFlags)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_HEAP_PROPERTIES*, D3D12_HEAP_FLAGS*, HRESULT>)_vtbl[14])(Unsafe.AsPointer(ref this), pHeapProperties, pHeapFlags);


}
