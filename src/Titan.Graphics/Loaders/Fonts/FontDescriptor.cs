using System.Runtime.InteropServices;

namespace Titan.Graphics.Loaders.Fonts
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct FontDescriptor
    {
        public ushort LineHeight;
        public ushort Base;
        public ushort CharactersCount;
        public ushort KerningsCount;
    }
}
