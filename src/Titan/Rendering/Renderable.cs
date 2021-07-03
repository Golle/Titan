using System.Numerics;
using System.Runtime.CompilerServices;
using Titan.Assets.Materials;
using Titan.Core;
using Titan.Graphics.D3D11.Buffers;

namespace Titan.Rendering
{
    [SkipLocalsInit]
    internal struct Renderable
    {
        public Matrix4x4 Transform;

        public Handle<Buffer> VertexBuffer;
        public Handle<Buffer> IndexBuffer;
        public Handle<Material> Material;
        public uint StartIndex;
        public uint Count;
    }
}
