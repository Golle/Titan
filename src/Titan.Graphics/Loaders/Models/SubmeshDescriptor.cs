using System.Runtime.InteropServices;

namespace Titan.Graphics.Loaders.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SubmeshDescriptor
    {
        public int MaterialIndex;
        public uint Count;
        public uint StartIndex;
    }
}
