using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{

    // TODO: Should the Context be a class or a struct? will deferred context be created per frame or per render pass?
    internal unsafe class ContextFactory
    {
        private readonly Device _device;

        public ContextFactory(Device device)
        {
            _device = device;
        }

        internal Context CreateImmediateContext() => new(_device.GetContext());

        internal Context CreateDeferredContext(uint flags = 0u)
        {
            using var deferredContext = new ComPtr<ID3D11DeviceContext>();
            Common.CheckAndThrow(_device.Get()->CreateDeferredContext(flags, deferredContext.GetAddressOf()), nameof(ID3D11Device.CreateDeferredContext));
            return new(deferredContext.Get());
        }

    }
}
