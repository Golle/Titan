using System;
using System.Diagnostics;
using System.Numerics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Windows.D3D11;

namespace Titan.Graphics
{
    public class GraphicsSystem : IDisposable
    {

        private GraphicsSystem()
        {
            
        }

        public static GraphicsSystem Create()
        {
            Debug.Assert(GraphicsDevice.IsInitialized, $"{nameof(GraphicsDevice)} must be initialized before the {nameof(GraphicsSystem)} is created.");

            unsafe
            {
                var handle = GraphicsDevice.BufferManager.Create(new BufferCreation
                {
                    Type = BufferTypes.VertexBuffer,
                    Stride = (uint) sizeof(Matrix4x4),
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                    Count = 1000,
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                });
            }
            

            return new GraphicsSystem();

        }

        public void Dispose()
        {

        }
    }
}
