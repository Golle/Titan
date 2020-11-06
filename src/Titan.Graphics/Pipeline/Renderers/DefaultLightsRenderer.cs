using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Renderers
{
    internal class DefaultLightsRenderer : IRenderer
    {
        private readonly ConstantBuffer<LightSource> _lightSource;
        private readonly IShaderManager _shaderManager;
        private readonly IVertexBufferManager _vertexBufferManager;
        private readonly IndexBuffer<ushort> _indexBuffer;

        private VertexBufferHandle _handle;

        public unsafe DefaultLightsRenderer(IGraphicsDevice device, IShaderManager shaderManager, IVertexBufferManager vertexBufferManager)
        {
            _shaderManager = shaderManager;
            _vertexBufferManager = vertexBufferManager;
            
            var vertices = stackalloc FullscreenVertex[4];
            vertices[0] = new FullscreenVertex { Position = new Vector2(-1, -1), UV = new Vector2(0, 1) };
            vertices[1] = new FullscreenVertex { Position = new Vector2(-1, 1), UV = new Vector2(0, 0) };
            vertices[2] = new FullscreenVertex { Position = new Vector2(1, 1), UV = new Vector2(1, 0) };
            vertices[3] = new FullscreenVertex { Position = new Vector2(1, -1), UV = new Vector2(1, 1) };
            _handle = _vertexBufferManager.CreateVertexBuffer(4u, (uint)sizeof(FullscreenVertex), vertices);

            
            _indexBuffer = new IndexBuffer<ushort>(device, new ushort[] { 0, 1, 2, 0, 2, 3 });
            _lightSource = new ConstantBuffer<LightSource>(device, new LightSource
            {
                Position = new Vector3(0, 0, -1)
            }, D3D11_USAGE.D3D11_USAGE_DYNAMIC, D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE);
        }


        private Vector3 _lightPosition = new Vector3(0,0,-1);
        private float _lightVelocity = 0.02f;
        

        public void Render(IRenderContext context)
        {
            #region TEMP LIGHT CALCULATIONS
            if (_lightPosition.X > 3.0f)
            {
                _lightPosition.X = 2.99f;
                _lightVelocity = -0.02f;
            }
            else if (_lightPosition.X < -3.0f)
            {
                _lightPosition.X = -2.99f;
                _lightVelocity = 0.02f;
            }

            _lightPosition.X += _lightVelocity;


            #endregion

            unsafe
            {
                context.MapResource(_lightSource.AsResourcePointer(), new LightSource {Position = _lightPosition});
            }

            context.SetPixelShaderConstantBuffer(_lightSource);

            context.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexBuffer(_vertexBufferManager[_handle]);
            context.SetIndexBuffer(_indexBuffer);
            _shaderManager.Get(_shaderManager.GetHandle("DeferredShadingDefault")).Bind(context);

            context.DrawIndexed(6);

        }

        public void Dispose()
        {
            _lightSource.Dispose();
            _vertexBufferManager.DestroyBuffer(_handle);
            _indexBuffer.Dispose();
        }


        [StructLayout(LayoutKind.Sequential, Size = 48)]
        private struct LightSource
        {
            public Vector3 Position;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FullscreenVertex
        {
            public Vector2 Position;
            public Vector2 UV;
        }
    }
}
