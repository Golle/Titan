using System.Runtime.CompilerServices;

namespace Titan.Platform.Win32.DXGI;

public unsafe struct IDXGIAdapter
{
    private void** _vtbl;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
}
