using System.Runtime.InteropServices;
using Titan.Graphics.D3D11;

namespace Titan.Graphics.Pipeline
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SetShaderResourceCommand
    {
        public ShaderResourceView View;
        public uint Slot;
    }
}
