using System.Numerics;
using Titan.Core.Common;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.State;
using Titan.Graphics.Pipeline.Graph;
using Titan.Graphics.Resources;
using Titan.Graphics.Shaders;
using Titan.Windows;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Renderers
{
    internal class DefaultSceneRenderer : IRenderer
    {
        private readonly IMeshRenderQueue _renderQueue;
        private readonly IWindow _window;
        private readonly IShaderManager _shaderManager;
        private readonly IVertexBufferManager _vertexBufferManager;
        private readonly IIndexBufferManager _indexBufferManager;
        private readonly IConstantBufferManager _constantBufferManager;
        private readonly IShaderResourceViewManager _shaderResourceViewManager;


        private readonly ConstantBufferHandle _perObjectHandle;
        private readonly ConstantBufferHandle _cameraHandle;

        private readonly SamplerState _sampler;

        public DefaultSceneRenderer(IGraphicsDevice device, IMeshRenderQueue renderQueue, IWindow window, IShaderManager shaderManager)
        {
            _renderQueue = renderQueue;
            _window = window;
            _shaderManager = shaderManager;
            _vertexBufferManager = device.VertexBufferManager;
            _indexBufferManager = device.IndexBufferManager;
            _constantBufferManager = device.ConstantBufferManager;
            _shaderResourceViewManager = device.ShaderResourceViewManager;

            _perObjectHandle = _constantBufferManager.CreateConstantBuffer<Matrix4x4>(usage: D3D11_USAGE.D3D11_USAGE_DYNAMIC, cpuAccess: D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE);
            _cameraHandle = _constantBufferManager.CreateConstantBuffer<CameraBuffer>(usage: D3D11_USAGE.D3D11_USAGE_DYNAMIC, cpuAccess: D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE);

            _sampler = new SamplerState(device);
        }

        private Vector3 modelPosition = new Vector3(0, 0, 0);
        private Vector2 modelRot = new Vector2(0, 0);
        


        public unsafe void Render(IRenderContext context)
        {
            // update camera

            #region TEMP_CAMERA IMPL

            var position = new Vector3(0, 0, -5);
            var projectionMatrix = MatrixExtensions.CreatePerspectiveLH(1f, _window.Height / (float)_window.Width, 0.5f, 10000f);
            var rotation = Quaternion.CreateFromYawPitchRoll(3, 0, 0);
            var forward = Vector3.Transform(new Vector3(0, 0, 1f), rotation);
            var up = Vector3.Transform(new Vector3(0, 1, 0), rotation);
            //position += Vector3.Transform(distance, rotation);
            var viewMatrix = Matrix4x4.CreateLookAt(position, position + forward, up);
            var viewProjectionMatrix = viewMatrix * projectionMatrix;
            //var viewProjectionMatrix = new Matrix4x4(-1, 0, 0, 0, 0, 1.77777779f, 0, 0, 0, 0, -1.00005f, -1, 0, 0, -0.5f, 0);
            var camera = new CameraBuffer
            {
                View = viewMatrix,
                ViewProjection = Matrix4x4.Transpose(viewProjectionMatrix)
            };

            #endregion

            #region TEMP MODEL SPIN

            {
                modelRot.X += 0.006f;
                modelRot.Y -= 0.004f;
            }
            var modelRotation = Quaternion.CreateFromYawPitchRoll(modelRot.X, modelRot.Y, 0);
            
            #endregion

            _shaderManager.Get(_shaderManager.GetHandle("GBufferDefault")).Bind(context);

            context.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            ref readonly var cameraBuffer = ref _constantBufferManager[_cameraHandle];
            context.MapResource(cameraBuffer.Resource, camera); // only needs to be done when the camera changes 
            
            context.SetVertexShaderConstantBuffer(cameraBuffer);
            context.SetPixelShaderSampler(_sampler);

            foreach (ref readonly var renderable in _renderQueue.GetRenderables())
            {
                //context.MapResource(_perObjectBuffer.AsResourcePointer(), renderable.World);
                var modelMatrix = Matrix4x4.Transpose(Matrix4x4.CreateScale(new Vector3(1, 1, 1)) *
                                                      Matrix4x4.CreateFromQuaternion(modelRotation) *
                                                      renderable.World);

                ref readonly var objectBuffer = ref _constantBufferManager[_perObjectHandle];
                context.MapResource(objectBuffer.Resource, modelMatrix);
                context.SetVertexShaderConstantBuffer(objectBuffer, 1u);
                context.SetPixelShaderResource(_shaderResourceViewManager[renderable.Texture.ResourceViewHandle]);

                var mesh = renderable.Mesh;

                ref readonly var indexBuffer = ref _indexBufferManager[mesh.IndexBufferHandle];
                context.SetIndexBuffer(indexBuffer);
                context.SetVertexBuffer(_vertexBufferManager[mesh.VertexBufferHandle]);

                var subsets = mesh.SubSets;
                if (subsets.Length > 1)
                {
                    for (var i = 0; i < subsets.Length; ++i)
                    {
                        ref readonly var subset = ref subsets[i];
                        context.DrawIndexed(subset.Count, subset.StartIndex);
                    }
                }
                else
                {
                    context.DrawIndexed(indexBuffer.NumberOfIndices);
                }
            }
        }

        private struct CameraBuffer
        {
            public Matrix4x4 View;
            public Matrix4x4 ViewProjection;
        }

        public void Dispose()
        {
            _sampler.Dispose();
            _constantBufferManager.DestroyBuffer(_cameraHandle);
            _constantBufferManager.DestroyBuffer(_perObjectHandle);
        }
    }
}
