using System.Numerics;
using Titan.Core;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Windows.D3D11;

namespace Titan.Graphics.Extensions
{
    internal struct FullscreenVertex
    {
        public Vector2 Position;
        public Vector2 Texture;
    }

    public static class BufferManagerExtensions
    {

        public static unsafe Handle<Buffer> CreateFullscreenVertexBuffer(this BufferManager manager)
        {
            var vertices = stackalloc FullscreenVertex[4];
            vertices[0] = new FullscreenVertex { Position = new Vector2(-1, -1), Texture = new Vector2(0, 1) };
            vertices[1] = new FullscreenVertex { Position = new Vector2(-1, 1), Texture = new Vector2(0, 0) };
            vertices[2] = new FullscreenVertex { Position = new Vector2(1, 1), Texture = new Vector2(1, 0) };
            vertices[3] = new FullscreenVertex { Position = new Vector2(1, -1), Texture = new Vector2(1, 1) };
            return GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Count = 4,
                Type = BufferTypes.VertexBuffer,
                InitialData = vertices,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                Stride = (uint)sizeof(FullscreenVertex),
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
            });
        }

        public static unsafe Handle<Buffer> CreateFullscreenIndexBuffer(this BufferManager manager)
        {
            var indices = stackalloc ushort[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;
            return GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Count = 6,
                Type = BufferTypes.IndexBuffer,
                InitialData = indices,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                Stride = sizeof(ushort),
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
            });
        }
    }
}
