using System.Runtime.InteropServices;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct ConstantBuffer
    {
        [FieldOffset(0)]
        public ID3D11Buffer* Pointer;
        [FieldOffset(0)]
        public ID3D11Resource* Resource;
    }
}
