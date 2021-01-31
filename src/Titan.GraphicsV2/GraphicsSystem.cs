using System;
using Titan.GraphicsV2.D3D11;

namespace Titan.GraphicsV2
{
    public class GraphicsSystem : IDisposable
    {
        private Device _device = new();
        
        public void Initialize(DeviceConfiguration configuration)
        {
            _device.Initialize(configuration);
        }

        public void Dispose()
        {
            _device.Dispose();
        }
    }
}
