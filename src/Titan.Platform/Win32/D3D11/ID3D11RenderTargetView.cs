using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Titan.Platform.Win32.D3D11;

public unsafe struct ID3D11RenderTargetView
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint AddRef() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
}
