using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.AssetConverter.Exporter
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct VertexTangentBiNormal
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
        public Vector3 Tangent;
        public Vector3 BiNormal;
    }
}
