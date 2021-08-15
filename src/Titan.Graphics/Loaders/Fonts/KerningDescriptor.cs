using System.Runtime.InteropServices;

namespace Titan.Graphics.Loaders.Fonts
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct KerningDescriptor
    {
        public char First;
        public char Second;
        public short Amount;
    }
}
