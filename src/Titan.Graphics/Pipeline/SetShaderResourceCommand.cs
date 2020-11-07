using System.Runtime.InteropServices;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Pipeline
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SetShaderResourceCommand
    {
        public ShaderResourceViewHandle Handle;
        public uint Slot;
    }
}
