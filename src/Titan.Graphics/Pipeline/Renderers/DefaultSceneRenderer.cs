using System.Numerics;
using Titan.Core.Common;
using Titan.Graphics.D3D11;
using Titan.Graphics.D3D11.Buffers;
using Titan.Graphics.Pipeline.Graph;
using Titan.Windows;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Pipeline.Renderers
{
    public class DefaultSceneRenderer : IRenderer
    {
        private readonly IMeshRenderQueue _renderQueue;
        private readonly IWindow _window;
        private readonly ConstantBuffer<Matrix4x4> _perObjectBuffer;
        private readonly ConstantBuffer<CameraBuffer> _camera;

        public DefaultSceneRenderer(IGraphicsDevice device, IMeshRenderQueue renderQueue, IWindow window)
        {
            _renderQueue = renderQueue;
            _window = window;

            _perObjectBuffer = new ConstantBuffer<Matrix4x4>(device, D3D11_USAGE.D3D11_USAGE_DYNAMIC, D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE);
            _camera = new ConstantBuffer<CameraBuffer>(device, D3D11_USAGE.D3D11_USAGE_DYNAMIC, D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE);
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
                modelRot.X += 0.03f;
                modelRot.Y -= 0.02f;
            }
            var modelRotation = Quaternion.CreateFromYawPitchRoll(modelRot.X, modelRot.Y, 0);
            var modelMatrix = Matrix4x4.Transpose(Matrix4x4.CreateScale(new Vector3(1, 1, 1)) *
                                                  Matrix4x4.CreateFromQuaternion(modelRotation) *
                                                  Matrix4x4.CreateTranslation(modelPosition));
            #endregion

            context.SetPritimiveTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.MapResource(_camera.AsResourcePointer(), camera); // only needs to be done when the camera changes 
            
            context.SetVertexShaderConstantBuffer(_camera);
            foreach (ref readonly var renderable in _renderQueue.GetRenderables())
            {
                //context.MapResource(_perObjectBuffer.AsResourcePointer(), renderable.World);
                context.MapResource(_perObjectBuffer.AsResourcePointer(), modelMatrix);
                context.SetVertexShaderConstantBuffer(_perObjectBuffer, 1u);

                var mesh = renderable.Mesh;
                
                mesh.Bind(context);
                
                var subsets = mesh.SubSets;
                if (subsets.Length > 0)
                {
                    for (var i = 0; i < subsets.Length; ++i)
                    {
                        ref readonly var subset = ref subsets[i];
                        context.DrawIndexed(subset.Count, subset.StartIndex);
                    }
                }
                else
                {
                    context.DrawIndexed(mesh.NumberOfIndices);
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
            _perObjectBuffer?.Dispose();
            _camera?.Dispose();
        }
    }
}
