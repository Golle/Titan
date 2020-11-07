using System.Numerics;
using System.Runtime.InteropServices;
using Titan.Graphics.D3D11;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Renderers
{
    internal class DefaultLightsRenderer : IRenderer
    {
        private readonly IShaderManager _shaderManager;
        
        private readonly IVertexBufferManager _vertexBufferManager;
        private readonly IIndexBufferManager _indexBufferManager;
        private readonly IConstantBufferManager _constantBufferManager;


        private readonly VertexBufferHandle _vertexBufferHandle;
        private readonly IndexBufferHandle _indexBufferHandle;
        private readonly ConstantBufferHandle _lightSourceHandle;

        public unsafe DefaultLightsRenderer(IGraphicsDevice device, IShaderManager shaderManager)
        {
            _shaderManager = shaderManager;
            _vertexBufferManager = device.VertexBufferManager;
            _indexBufferManager = device.IndexBufferManager;
            _constantBufferManager = device.ConstantBufferManager;

            var vertices = stackalloc FullscreenVertex[4];
            vertices[0] = new FullscreenVertex { Position = new Vector2(-1, -1), UV = new Vector2(0, 1) };
            vertices[1] = new FullscreenVertex { Position = new Vector2(-1, 1), UV = new Vector2(0, 0) };
            vertices[2] = new FullscreenVertex { Position = new Vector2(1, 1), UV = new Vector2(1, 0) };
            vertices[3] = new FullscreenVertex { Position = new Vector2(1, -1), UV = new Vector2(1, 1) };
            _vertexBufferHandle = _vertexBufferManager.CreateVertexBuffer(4u, (uint)sizeof(FullscreenVertex), vertices);
            
            var indices = stackalloc ushort[6];
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;
            indices[3] = 0;
            indices[4] = 2;
            indices[5] = 3;
            _indexBufferHandle = _indexBufferManager.CreateIndexBuffer<ushort>(6, indices);
            _lightSourceHandle = _constantBufferManager.CreateConstantBuffer(new LightSource
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


            ref readonly var lightSource = ref _constantBufferManager[_lightSourceHandle];
            unsafe
            {
                context.MapResource(lightSource.Resource, new LightSource {Position = _lightPosition});
            }
            
            context.SetPixelShaderConstantBuffer(lightSource);

            context.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetVertexBuffer(_vertexBufferManager[_vertexBufferHandle]);
            context.SetIndexBuffer(_indexBufferManager[_indexBufferHandle]);

            _shaderManager.Get(_shaderManager.GetHandle("DeferredShadingDefault")).Bind(context);

            context.DrawIndexed(6);

        }

        public void Dispose()
        {
            _constantBufferManager.DestroyBuffer(_lightSourceHandle);
            _vertexBufferManager.DestroyBuffer(_vertexBufferHandle);
            _indexBufferManager.DestroyBuffer(_indexBufferHandle);
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
