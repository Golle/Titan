using System.Runtime.InteropServices;
using Titan.Graphics.Resources;

namespace Titan.Graphics.Pipeline
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SetShaderResourceCommand
    {
        public Handle<ShaderResourceView> Handle;
        public uint Slot;
    }
}
