using System.Numerics;
using System.Runtime.InteropServices;

namespace Titan.AssetConverter.Exporter
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 Texture;
    }
}
