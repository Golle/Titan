using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal readonly unsafe struct InputLayout
    {
        private readonly ID3D11InputLayout* _layout;
        internal InputLayout(ID3D11InputLayout* layout)
        {
            _layout = layout;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ID3D11InputLayout*(in InputLayout layout) => layout._layout;
        public void Release() => _layout->Release();
    }
}
