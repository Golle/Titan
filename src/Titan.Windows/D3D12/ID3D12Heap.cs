using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Windows.D3D12;

[Guid("6b3b2502-6e51-45b3-90ee-9884265e8df3")]
public unsafe struct ID3D12Heap
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

    //DECLSPEC_XFGVIRT(ID3D12DeviceChild, GetDevice)
    //HRESULT(STDMETHODCALLTYPE* GetDevice)(
    //ID3D12Heap* This,
    //REFIID riid,
    //_COM_Outptr_opt_  void** ppvDevice);

    //DECLSPEC_XFGVIRT(ID3D12Heap, GetDesc)
    //D3D12_HEAP_DESC* (STDMETHODCALLTYPE* GetDesc ) (
    //    ID3D12Heap* This,
    //    D3D12_HEAP_DESC* RetVal);
}
