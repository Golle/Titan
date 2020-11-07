using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct ShaderResourceView
    {
        public ID3D11ShaderResourceView* Pointer;
        public DXGI_FORMAT Format;
    }
}
