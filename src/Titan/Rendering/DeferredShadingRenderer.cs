using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Core;
using Titan.Graphics;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Extensions;
using Titan.Windows.D3D11;

namespace Titan.Rendering
{
    [StructLayout(LayoutKind.Explicit, Size = 1024)]
    internal unsafe struct LightBuffer
    {
        [FieldOffset(0)]
        public fixed float Lights[32 * 4];

        [FieldOffset(sizeof(float) * 4 * 32)]
        public int NumberOfLights;

        public void SetLight(int index, in Vector3 position)
        {
            fixed (float* pLights = Lights)
            {
                ((Vector4*) pLights)[index] = new Vector4(position, 1);
            }
        }
    }

    internal sealed class DeferredShadingRenderer : Renderer
    {
        private readonly Handle<Buffer> _vertexBuffer;
        private readonly Handle<Buffer> _indexBuffer;
        private readonly Handle<Buffer> _lightBuffer;

        public unsafe DeferredShadingRenderer()
        {
            _vertexBuffer = GraphicsDevice.BufferManager.CreateFullscreenVertexBuffer();
            _indexBuffer = GraphicsDevice.BufferManager.CreateFullscreenIndexBuffer();

            _lightBuffer = GraphicsDevice.BufferManager.Create(new BufferCreation
            {
                Count = 1,
                Type = BufferTypes.ConstantBuffer,
                Stride = (uint) sizeof(LightBuffer),
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
            });
        }

        public override void Render(Context context)
        {
            var lights = new LightBuffer
            {
                NumberOfLights = 1
            };
            lights.SetLight(0, new Vector3(3, -10, 30));
            context.SetPixelShaderConstantBuffer(_lightBuffer, 2);
            context.Map(_lightBuffer, lights);
            context.SetPrimitiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

            context.SetVertexBuffer(_vertexBuffer);
            context.SetIndexBuffer(_indexBuffer);
            context.DrawIndexed(6);
        }

        public override void Dispose()
        {
            GraphicsDevice.BufferManager.Release(_vertexBuffer);
            GraphicsDevice.BufferManager.Release(_indexBuffer);
            GraphicsDevice.BufferManager.Release(_lightBuffer);
        }
    }
}
