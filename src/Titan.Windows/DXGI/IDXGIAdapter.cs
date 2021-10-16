using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming
namespace Titan.Windows.DXGI
{
    public unsafe struct IDXGIAdapter
    {
        private void** _vtbl;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
    }
}
