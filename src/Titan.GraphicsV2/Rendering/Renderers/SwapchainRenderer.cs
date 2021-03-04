using System.Numerics;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Buffers;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Rendering.Renderers
{
    internal unsafe class SwapchainRenderer : IRenderer
    {
        private readonly Device _device;
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private ViewPort _viewPort;

        public SwapchainRenderer(Device device)
        {
            _device = device;
        }

        public void Init()
        {
            {
                var vertices = stackalloc FullscreenVertex[4];
                vertices[0] = new FullscreenVertex { Position = new Vector2(-1, -1), UV = new Vector2(0, 1) };
                vertices[1] = new FullscreenVertex { Position = new Vector2(-1, 1), UV = new Vector2(0, 0) };
                vertices[2] = new FullscreenVertex { Position = new Vector2(1, 1), UV = new Vector2(1, 0) };
                vertices[3] = new FullscreenVertex { Position = new Vector2(1, -1), UV = new Vector2(1, 1) };
                var handle = _device.BufferManager.Create(new BufferCreation
                {
                    Count = 4,
                    Type = BufferTypes.VertexBuffer,
                    InitialData = vertices,
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                    Stride = (uint)sizeof(FullscreenVertex),
                    Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
                });
                _vertexBuffer = _device.BufferManager.Access(handle);
            }
            {
                var indices = stackalloc ushort[6];
                indices[0] = 0;
                indices[1] = 1;
                indices[2] = 2;
                indices[3] = 0;
                indices[4] = 2;
                indices[5] = 3;
                var handle = _device.BufferManager.Create(new BufferCreation
                {
                    Count = 6,
                    Type = BufferTypes.IndexBuffer,
                    InitialData = indices,
                    CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                    Stride = sizeof(ushort),
                    Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE
                });
                _indexBuffer = _device.BufferManager.Access(handle);
            }
            var swapchain = _device.Swapchain;
            _viewPort = new ViewPort((int)swapchain.Width, (int)swapchain.Height);
        }

        public void Render(Context context)
        {
            context.SetTopology(D3D_PRIMITIVE_TOPOLOGY.D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);
            context.SetViewPort(_viewPort);
            context.SetVertexBuffer(_vertexBuffer);
            context.SetIndexBuffer(_indexBuffer);

            context.DrawIndexed(6);
        }

        public void Dispose()
        {
            _device.BufferManager.Release(_indexBuffer.Handle);
            _device.BufferManager.Release(_vertexBuffer.Handle);
            _indexBuffer = default;
            _vertexBuffer = default;
            _viewPort = default;
        }
    }
}
