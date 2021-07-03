using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming
namespace Titan.Windows.D3D11
{
    public unsafe struct ID3D11Resource
    {
        private void** _vtbl;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
    }
}
