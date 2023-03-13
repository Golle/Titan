using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Platform.Win32;

namespace Titan.Platform.DXC;

[Guid("73EFFE2A-70DC-45F8-9690-EFF64C02429D")]
internal unsafe struct IDXCCompilerArgs
{
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


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char** GetArguments()
        => ((delegate* unmanaged[Stdcall]<void*, char**>)_vtbl[3])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint GetCount()
        => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[4])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT AddArguments(char** pArguments, uint argCount)
        => ((delegate* unmanaged[Stdcall]<void*, char**, uint, HRESULT>)_vtbl[5])(Unsafe.AsPointer(ref this), pArguments, argCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT AddArgumentsUTF8(char** pArguments, uint argCount)
        => ((delegate* unmanaged[Stdcall]<void*, char**, uint, HRESULT>)_vtbl[6])(Unsafe.AsPointer(ref this), pArguments, argCount);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public HRESULT AddDefines(DXCDefine* pDefines, uint defineCount)
        => ((delegate* unmanaged[Stdcall]<void*, DXCDefine*, uint, HRESULT>)_vtbl[7])(Unsafe.AsPointer(ref this), pDefines, defineCount);
}
