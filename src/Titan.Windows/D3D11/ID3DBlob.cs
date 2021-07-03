using System;
using System.Runtime.CompilerServices;

// ReSharper disable InconsistentNaming

namespace Titan.Windows.D3D11
{
    public unsafe struct ID3DBlob
    {
        private void** _vtbl;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int QueryInterface(Guid* riid, void** ppvObject) => ((delegate* unmanaged<void*, Guid*, void**, int>)_vtbl[0])(Unsafe.AsPointer(ref this), riid, ppvObject);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint AddRef() => ((delegate* unmanaged<void*, uint>)_vtbl[1])(Unsafe.AsPointer(ref this));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Release() => ((delegate* unmanaged<void*, uint>)_vtbl[2])(Unsafe.AsPointer(ref this));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void* GetBufferPointer() => ((delegate* unmanaged<void*, void*>)_vtbl[3])(Unsafe.AsPointer(ref this));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public nuint GetBufferSize() => ((delegate* unmanaged<void*, nuint>)_vtbl[4])(Unsafe.AsPointer(ref this));

    }
}
