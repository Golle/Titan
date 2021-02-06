using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal readonly unsafe struct Buffer
    {
        private readonly ID3D11Buffer* _buffer;
        internal Buffer(ID3D11Buffer* buffer)
        {
            _buffer = buffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11Buffer* AsPointer() => _buffer;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ID3D11Buffer*(in Buffer buffer) => buffer._buffer;
    }
}
