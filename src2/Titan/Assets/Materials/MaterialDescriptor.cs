using System.Runtime.InteropServices;

namespace Titan.Assets.Materials
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct MaterialDescriptor
    {
        public uint Count;
    }
}
