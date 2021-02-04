using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe class Device : IDisposable
    {
        private ComPtr<ID3D11Device> _device;
        private ComPtr<ID3D11DeviceContext> _context;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11Device* Get() => _device.Get();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11DeviceContext* GetContext() => _context.Get();
        
        public Device(ID3D11Device * device, ID3D11DeviceContext* context)
        {
            _device = new ComPtr<ID3D11Device>(device);
            _context = new ComPtr<ID3D11DeviceContext>(context);
        }

        public void Dispose()
        {
            _context.Dispose();
            _device.Dispose();
        }
    }
}
