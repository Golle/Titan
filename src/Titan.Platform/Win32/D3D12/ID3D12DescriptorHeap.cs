using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

[Guid("8efb471d-616c-4f49-90f7-127bb763fa51")]
public unsafe struct ID3D12DescriptorHeap : INativeGuid
{
    public static Guid* Guid => IID.IID_ID3D12DescriptorHeap;
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    //ID3D12DescriptorHeap* This,
    //_In_ REFGUID guid,
    //_Inout_  UINT* pDataSize,
    //_Out_writes_bytes_opt_( *pDataSize )  void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    //ID3D12DescriptorHeap* This,
    //_In_ REFGUID guid,
    //_In_  UINT DataSize,
    //_In_reads_bytes_opt_( DataSize )  const void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    //ID3D12DescriptorHeap* This,
    //_In_ REFGUID guid,
    //_In_opt_  const IUnknown* pData);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT SetName(char* Name)
        => ((delegate* unmanaged[Stdcall]<void*, char*, HRESULT>)_vtbl[6])(Unsafe.AsPointer(ref this), Name);

    //HRESULT(STDMETHODCALLTYPE* GetDevice)(
    //ID3D12DescriptorHeap* This,
    //REFIID riid,
    //_COM_Outptr_opt_  void** ppvDevice);

    //D3D12_DESCRIPTOR_HEAP_DESC(STDMETHODCALLTYPE* GetDesc)(
    //    ID3D12DescriptorHeap* This);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public D3D12_CPU_DESCRIPTOR_HANDLE* GetCPUDescriptorHandleForHeapStart(D3D12_CPU_DESCRIPTOR_HANDLE* RetVal)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_CPU_DESCRIPTOR_HANDLE*, D3D12_CPU_DESCRIPTOR_HANDLE*>)_vtbl[9])(Unsafe.AsPointer(ref this), RetVal);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public D3D12_GPU_DESCRIPTOR_HANDLE* GetGPUDescriptorHandleForHeapStart(D3D12_GPU_DESCRIPTOR_HANDLE* RetVal)
        => ((delegate* unmanaged[Stdcall]<void*, D3D12_GPU_DESCRIPTOR_HANDLE*, D3D12_GPU_DESCRIPTOR_HANDLE*>)_vtbl[10])(Unsafe.AsPointer(ref this), RetVal);
}
