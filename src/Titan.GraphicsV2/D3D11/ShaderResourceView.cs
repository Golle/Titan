using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal readonly unsafe struct ShaderResourceView
    {
        private readonly ID3D11ShaderResourceView* _view;
        internal ShaderResourceView(ID3D11ShaderResourceView* view) => _view = view;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ID3D11ShaderResourceView*(in ShaderResourceView view) => view._view;
        public void Release() => _view->Release();
    }
}
