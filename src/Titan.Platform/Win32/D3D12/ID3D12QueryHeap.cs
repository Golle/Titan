using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Titan.Platform.Win32.D3D12;

[Guid("0d9658ae-ed45-469e-a61d-970ec583cab4")]
public unsafe struct ID3D12QueryHeap : INativeGuid
{
    public static Guid* Guid => IID.IID_ID3D12QueryHeap;
    private void** _vtbl;
    public HRESULT QueryInterface(Guid* riid, void** ppvObject)
        => ((delegate* unmanaged[Stdcall]<void*, Guid*, void**, HRESULT>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

    //HRESULT(STDMETHODCALLTYPE* GetPrivateData)(
    //ID3D12QueryHeap* This,
    //_In_ REFGUID guid,
    //_Inout_  UINT* pDataSize,
    //_Out_writes_bytes_opt_( *pDataSize )  void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateData)(
    //ID3D12QueryHeap* This,
    //_In_ REFGUID guid,
    //_In_  UINT DataSize,
    //_In_reads_bytes_opt_( DataSize )  const void* pData);

    //HRESULT(STDMETHODCALLTYPE* SetPrivateDataInterface)(
    //ID3D12QueryHeap* This,
    //_In_ REFGUID guid,
    //_In_opt_  const IUnknown* pData);

    //HRESULT(STDMETHODCALLTYPE* SetName)(
    //ID3D12QueryHeap* This,
    //_In_z_ LPCWSTR Name);

    //HRESULT(STDMETHODCALLTYPE* GetDevice)(
    //ID3D12QueryHeap* This,
    //REFIID riid,
    //_COM_Outptr_opt_  void** ppvDevice);
}
