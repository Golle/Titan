// ReSharper disable InconsistentNaming

using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.D3D11;

public unsafe struct ID3D11CommandList
{
    private void** _vtbl;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
}
