using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Texture
    {
        [FieldOffset(0)]
        public ID3D11Texture2D* Pointer;
        [FieldOffset(0)]
        public ID3D11Resource* Resource;
        [FieldOffset(8)]
        public DXGI_FORMAT Format;
        [FieldOffset(12)]
        public D3D11_BIND_FLAG BindFlags;
        [FieldOffset(16)]
        public uint Width;
        [FieldOffset(20)]
        public uint Height;
    }
}
