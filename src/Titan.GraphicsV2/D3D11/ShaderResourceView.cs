using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal readonly unsafe struct ShaderResourceView
    {
        internal readonly ID3D11ShaderResourceView* View;
        internal ShaderResourceView(ID3D11ShaderResourceView* view)
        {
            View = view;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11ShaderResourceView* AsPointer() => View;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ID3D11ShaderResourceView*(in ShaderResourceView view) => view.View;
    }
}
