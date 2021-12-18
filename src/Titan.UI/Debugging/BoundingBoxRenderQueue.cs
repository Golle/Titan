using System;
using System.Numerics;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Core.Services;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Windows.D3D11;

namespace Titan.UI.Debugging
{
    public class BoundingBoxRenderQueue : IServicePreUpdate, IServicePostUpdate
    {
        public Handle<ResourceBuffer> VertexBuffer { get; }
        public int NumberOfVertices => _count;
        private readonly MemoryChunk<Vector3> _lines;
        private int _count;


        public unsafe BoundingBoxRenderQueue()
        {
            var maxVertices = 10_000u;
            _lines = MemoryUtils.AllocateBlock<Vector3>(maxVertices);

            VertexBuffer = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Type = BufferTypes.VertexBuffer,
                Count = maxVertices,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                Stride = (uint)sizeof(Vector3),
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            });
        }

        public void PreUpdate()
        {
            _count = 0;
        }
        public void Add(ReadOnlySpan<Vector3> positions)
        {
            for (var i = 0; i < positions.Length; ++i)
            {
                _lines[_count++] = positions[i];
                _lines[_count++] = positions[(i + 1) % positions.Length];
            }
        }

        public unsafe void PostUpdate()
        {
            GraphicsDevice.ImmediateContext.Map(VertexBuffer, _lines.AsPointer(), (uint)(_count * sizeof(Vector3)));
        }
    }
}
