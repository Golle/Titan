using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Buffers;
using Titan.GraphicsV2.Resources;
using Titan.Windows;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering.Renderers
{

    internal struct CameraBuffer
    {
        public Matrix4x4 View;
        public Matrix4x4 ViewProjection;
    }

    internal class GeometryRenderer : IRenderer
    {
        private readonly Device _device;
        private readonly ModelLoader _loader;
        private ViewPort _viewPort;
        private Model3D _model;
        
        private Buffer _cameraBuffer;
        private Buffer _worldBuffer;

        private Camera _camera;
        private Matrix4x4 _worldMatrix;

        public GeometryRenderer(Device device, ModelLoader loader, IWindow window)
        {
            _device = device;
            _loader = loader;
            _camera = new Camera(window);
            _camera.MoveForward(10f);
            _camera.Update();
            
        }

        public void Init()
        {
            _viewPort = new ViewPort((int) _device.Swapchain.Width, (int) _device.Swapchain.Height);

            _model = _loader.Load("models1/table.dat");
            
            unsafe
            {
                var handle = _device.BufferManager.Create(new BufferCreation
                {
                    Count = 1,
                    Stride = (uint) sizeof(CameraBuffer),
                    Type = BufferTypes.ConstantBuffer,
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                    Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
                });
                _cameraBuffer = _device.BufferManager.Access(handle);
            }


            unsafe
            {
                var handle = _device.BufferManager.Create(new BufferCreation
                {
                    Count = 1,
                    Stride = (uint)sizeof(Matrix4x4),
                    Type = BufferTypes.ConstantBuffer,
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                    Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC
                });
                _worldBuffer = _device.BufferManager.Access(handle);
            }

            _worldMatrix = Matrix4x4.CreateScale(1f) *
                Matrix4x4.CreateFromQuaternion(Quaternion.Identity) *
                Matrix4x4.CreateTranslation(new Vector3(0, 0, 1f));

        }


        private Vector3 _position = new Vector3(0,0, 10f);
        public unsafe void Render(Context context)
        {
            //_camera.Rotate(new Vector3(1, 0.5f, 2f));
            _camera.MoveForward(0.2f);
            
            //_camera.MoveForward(-1.1f);
            _camera.Update();
            //_position -= Vector3.UnitZ * 0.01f;

            _worldMatrix = Matrix4x4.CreateScale(1f) *
                           Matrix4x4.CreateFromQuaternion(Quaternion.Identity) *
                           Matrix4x4.CreateTranslation(_position);

            /// Temp camera code
            var context1 = _device.GetContext();
            {

                D3D11_MAPPED_SUBRESOURCE mappedResource;
                context1->Map((ID3D11Resource*) _cameraBuffer.Resource, 0, D3D11_MAP.D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
                var buff = new CameraBuffer {View = _camera.View, ViewProjection = Matrix4x4.Transpose(_camera.ViewProjection)};
                var size = sizeof(CameraBuffer);
                System.Buffer.MemoryCopy(&buff, mappedResource.pData, size, size);
                context1->Unmap((ID3D11Resource*) _cameraBuffer.Resource, 0);
            }
            {

                D3D11_MAPPED_SUBRESOURCE mappedResource;
                context1->Map((ID3D11Resource*)_worldBuffer.Resource, 0, D3D11_MAP.D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
                fixed (Matrix4x4* pWorld = &_worldMatrix)
                {
                    var size = sizeof(Matrix4x4);
                    System.Buffer.MemoryCopy(&pWorld, mappedResource.pData, size, size);
                }
                context1->Unmap((ID3D11Resource*)_worldBuffer.Resource, 0);
            }

            var buffers = stackalloc ID3D11Buffer*[2];
            buffers[0] = _cameraBuffer.Resource;
            buffers[1] = _worldBuffer.Resource;
            context1->VSSetConstantBuffers(0, 2, buffers);
            /// Temp camera code


            context.SetViewPort(_viewPort);

            context.SetTopology(D3D_PRIMITIVE_TOPOLOGY.D3D_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

            context.SetVertexBuffer(_device.BufferManager.Access(_model.VertexBuffer));
            context.SetIndexBuffer(_device.BufferManager.Access(_model.IndexBuffer));

            var a = _device.BufferManager.Access(_model.IndexBuffer).Count;
            
            context.DrawIndexed(a);
        }

        public void Dispose()
        {

        }
    }
}
