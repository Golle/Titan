using System.Runtime.InteropServices;

namespace Titan.Assets.Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct SubMesh
    {
        public int MaterialIndex;
        public uint Count;
        public uint StartIndex;
    }
}
