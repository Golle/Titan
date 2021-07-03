using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
namespace Titan.Windows.D3D11
{
    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11_SUBRESOURCE_DATA
    {
        public unsafe void* pSysMem;
        public uint SysMemPitch;
        public uint SysMemSlicePitch;
    }
}
