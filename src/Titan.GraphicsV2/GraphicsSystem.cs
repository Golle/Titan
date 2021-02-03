using System;
using Titan.GraphicsV2.D3D11;
using Titan.IOC;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2
{
    public class GraphicsSystem : IDisposable
    {
        private readonly IContainer _container;
        private Device _device;
        private Swapchain _swapchain;
        private RenderTargetView _backbuffer;

        public GraphicsSystem(IContainer container)
        {
            _container = container;
        }

        public void Initialize(DeviceConfiguration configuration)
        {
            (_device, _swapchain) = _container
                .CreateInstance<DeviceFactory>() // Use create instance so that the factory till be GCed when the device has been created.
                .Create(configuration);

            _container
                .RegisterSingleton(_device, dispose: true)
                .RegisterSingleton(_swapchain, dispose: true);

            var renderTargetFactory = _container.CreateInstance<RenderTargetViewFactory>();
            
            _backbuffer = renderTargetFactory
                .CreateBackbuffer();


            unsafe
            {
                var texture = _container
                    .CreateInstance<Texture2DFactory>()
                    .Create(100, 100, DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM, bindFlag: D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET | D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE);

                var rtv = renderTargetFactory
                    .Create(texture);
                var rtv1 = renderTargetFactory.Create(texture.AsResource(), DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM);

                rtv1.View->Release();
                rtv.View->Release();

                texture.AsPtr()->Release();
            }
            
        }


        public void RenderFrame()
        {
            unsafe
            {
                var color = new Color(1, 0, 1, 1);
                _device.GetContext()->ClearRenderTargetView(_backbuffer.View, (float*) &color);
            }
            _swapchain.Present();
        }

        public void Dispose()
        {
            unsafe
            {
                _backbuffer.View->Release();
            }

            _swapchain.Dispose();
            _device.Dispose();
            _container.Dispose();
        }
    }
}
