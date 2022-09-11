using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Titan.Core;
using Titan.Core.Memory;
using Titan.Core.Services;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Platform.Win32.D3D11;

namespace Titan.UI.Debugging
{
    public class BoundingBoxRenderQueue : IServicePreUpdate, IServicePostUpdate
    {
        public Handle<ResourceBuffer> VertexBuffer { get; }
        public int NumberOfVertices => _count;
        private readonly MemoryChunk<Line> _lines;
        private int _count;


        [StructLayout(LayoutKind.Sequential, Size = 32)]
        [SkipLocalsInit]
        private struct Line
        {
            public Color Color;
            public Vector3 Position;
            private float Nothing;
        }
        public unsafe BoundingBoxRenderQueue()
        {
            var maxVertices = 10_000u;
            _lines = MemoryUtilsOld.AllocateBlock<Line>(maxVertices);

            VertexBuffer = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Type = BufferTypes.VertexBuffer,
                Count = maxVertices,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                Stride = (uint)sizeof(Line),
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            });
        }

        public void PreUpdate()
        {
            _count = 0;
        }
        public void Add(ReadOnlySpan<Vector3> positions, in Color color)
        {
            for (var i = 0; i < positions.Length; ++i)
            {
                var next = Interlocked.Add(ref _count, 2);
                _lines[next-2] = new Line { Position = positions[i], Color = color };
                _lines[next - 1] = new Line { Position = positions[(i + 1) % positions.Length], Color = color };
            }
        }

        public unsafe void PostUpdate()
        {
            GraphicsDevice.ImmediateContext.Map(VertexBuffer, _lines.AsPointer(), (uint)(_count * sizeof(Line)));
        }
    }
}
