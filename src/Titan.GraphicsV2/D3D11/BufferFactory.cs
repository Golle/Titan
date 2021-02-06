using System.Runtime.CompilerServices;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe class BufferFactory
    {
        private readonly Device _device;
        public BufferFactory(Device device)
        {
            _device = device;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Buffer Create<T>(uint count, void* initialData, D3D11_USAGE usage, D3D11_CPU_ACCESS_FLAG cpuAccessFlags, D3D11_BIND_FLAG bindFlags, D3D11_RESOURCE_MISC_FLAG miscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED) where T : unmanaged 
            => Create(count, (uint) sizeof(T), initialData, usage, cpuAccessFlags, bindFlags, miscFlags);

        public Buffer Create(uint count, uint stride, void* initialData, D3D11_USAGE usage, D3D11_CPU_ACCESS_FLAG cpuAccessFlags, D3D11_BIND_FLAG bindFlags, D3D11_RESOURCE_MISC_FLAG miscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED)
        {
            var desc = new D3D11_BUFFER_DESC
            {
                BindFlags = bindFlags,
                Usage = usage,
                CpuAccessFlags = cpuAccessFlags,
                MiscFlags = miscFlags,
                ByteWidth = count * stride,
                StructureByteStride = stride
            };

            ID3D11Buffer* buffer;
            if (initialData != null)
            {
                var subResource = new D3D11_SUBRESOURCE_DATA
                {
                    pSysMem = initialData
                };
                Common.CheckAndThrow(_device.Get()->CreateBuffer(&desc, &subResource, &buffer), nameof(ID3D11Device.CreateBuffer));
            }
            else
            {
                Common.CheckAndThrow(_device.Get()->CreateBuffer(&desc, null, &buffer), nameof(ID3D11Device.CreateBuffer));
            }
            return new Buffer(buffer);
        }
    }
}
