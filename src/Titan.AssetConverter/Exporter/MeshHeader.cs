using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.AssetConverter.Exporter
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MeshHeader
    {
        public Vector3 Min;
        public Vector3 Max;
        public int VerticeCount;
        public int IndexCount;
    }
}
