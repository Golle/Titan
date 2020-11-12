using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.AssetConverter.Exporter
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SubMesh
    {
        public int StartIndex;
        public int Count;
        public int MaterialIndex;
        public Vector3 Min;
        public Vector3 Max;
    }
}
