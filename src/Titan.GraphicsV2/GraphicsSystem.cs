using System;
using Titan.GraphicsV2.D3D11;

namespace Titan.GraphicsV2
{
    public class GraphicsSystem : IDisposable
    {
        private Device _device;
        private Swapchain _swapchain;

        public void Initialize(DeviceConfiguration configuration)
        {
            (_device, _swapchain) = new DeviceFactory().Create(configuration);
   
        }

        public void Dispose()
        {
            _swapchain.Dispose();
            _device.Dispose();
        }
    }
}
