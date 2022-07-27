using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Windows.D3D12;

[Guid("6102dee4-af59-4b09-b999-b44d73f09b24")]
public unsafe struct ID3D12CommandAllocator
{
    private void** _vtbl;

    //HRESULT(STDMETHODCALLTYPE* QueryInterface)(
    //ID3D12CommandAllocator* This,
    //REFIID riid,
    //_COM_Outptr_  void** ppvObject);

    //ULONG(STDMETHODCALLTYPE* AddRef)(
    //    ID3D12CommandAllocator* This);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    //ID3D12CommandAllocator* This,
    //_In_ REFGUID guid,
    //_Inout_  UINT* pDataSize,
    //_Out_writes_bytes_opt_( *pDataSize )  void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    //ID3D12CommandAllocator* This,
    //_In_ REFGUID guid,
    //_In_  UINT DataSize,
    //_In_reads_bytes_opt_( DataSize )  const void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    //ID3D12CommandAllocator* This,
    //_In_ REFGUID guid,
    //_In_opt_  const IUnknown* pData);

    //HRESULT(STDMETHODCALLTYPE* SetName)(
    //ID3D12CommandAllocator* This,
    //_In_z_ LPCWSTR Name);

    //HRESULT(STDMETHODCALLTYPE* GetDevice)(
    //ID3D12CommandAllocator* This,
    //REFIID riid,
    //_COM_Outptr_opt_  void** ppvDevice);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT Reset()
        => ((delegate* unmanaged[Stdcall]<void*, HRESULT>)_vtbl[8])(Unsafe.AsPointer(ref this));

}
