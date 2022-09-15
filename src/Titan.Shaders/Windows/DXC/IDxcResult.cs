using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32;

namespace Titan.Shaders.Windows.DXC;

[Guid("58346CDA-DDE7-4497-9461-6F87AF5E0659")]
public unsafe struct IDxcResult
{
    // IUnknown
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

    //IDxcOperationResult
    //virtual HRESULT STDMETHODCALLTYPE GetStatus(_Out_ HRESULT *pStatus) = 0;

    //// GetResult returns the main result of the operation.
    //// This corresponds to:
    //// DXC_OUT_OBJECT - Compile() with shader or library target
    //// DXC_OUT_DISASSEMBLY - Disassemble()
    //// DXC_OUT_HLSL - Compile() with -P
    //// DXC_OUT_ROOT_SIGNATURE - Compile() with rootsig_* target
    //virtual HRESULT STDMETHODCALLTYPE GetResult(_COM_Outptr_result_maybenull_ IDxcBlob **ppResult) = 0;

    //// GetErrorBuffer Corresponds to DXC_OUT_ERRORS.
    //virtual HRESULT STDMETHODCALLTYPE GetErrorBuffer(_COM_Outptr_result_maybenull_ IDxcBlobEncoding **ppErrors) = 0;


    //IDxcResult
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int HasOut(DXC_OUT_KIND dxcOutKind)
        => ((delegate* unmanaged[Stdcall]<void*, DXC_OUT_KIND, int>)_vtbl[6])(Unsafe.AsPointer(ref this), dxcOutKind);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT GetOutput(DXC_OUT_KIND dxcOutKind, in Guid iid, void** ppvObject, IDxcBlobWide** ppOutputName)
        => ((delegate* unmanaged[Stdcall]<void*, DXC_OUT_KIND, in Guid, void**, IDxcBlobWide**, HRESULT>)_vtbl[7])(Unsafe.AsPointer(ref this), dxcOutKind, iid, ppvObject, ppOutputName);


    //virtual UINT32 GetNumOutputs() = 0;
    //virtual DXC_OUT_KIND GetOutputByIndex(UINT32 Index) = 0;
    //virtual DXC_OUT_KIND PrimaryOutput() = 0;
};
