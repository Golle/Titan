using System.Runtime.CompilerServices;
#pragma warning disable 169

namespace Titan.D3D11
{
    public unsafe struct ID3D11Device
    {
        private void** _vtbl;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Release() => ((delegate* unmanaged[Stdcall]<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));
    }
}
