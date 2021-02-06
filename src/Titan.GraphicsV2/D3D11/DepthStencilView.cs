using System.Runtime.CompilerServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal readonly unsafe struct DepthStencilView
    {
        private readonly ID3D11DepthStencilView* _view;
        public DepthStencilView(ID3D11DepthStencilView* view)
        {
            _view = view;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11DepthStencilView* AsPointer() => _view;
    }
}
