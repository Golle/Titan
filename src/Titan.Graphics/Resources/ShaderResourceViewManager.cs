using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Memory;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D_SRV_DIMENSION;

namespace Titan.Graphics.Resources
{
    internal unsafe class ShaderResourceViewManager : IShaderResourceViewManager
    {
        private ComPtr<ID3D11Device> _device;
        private readonly ShaderResourceView* _resources;

        private int _numberOfResources;
        private uint _maxResources;

        public ShaderResourceViewManager(ID3D11Device* device, IMemoryManager memoryManager)
        {
            _device = new ComPtr<ID3D11Device>(device);
            var memory = memoryManager.GetMemoryChunk("ShaderResourceView");
            Debug.Assert(memory.Stride == sizeof(ShaderResourceView), "The stride of the memory chunk is not matching the expected size");
            _resources = (ShaderResourceView*)memory.Pointer;
            _maxResources = memory.Count;
        }


        public ShaderResourceViewHandle Create(ID3D11Resource* resource, DXGI_FORMAT format)
        {
            var desc = new D3D11_SHADER_RESOURCE_VIEW_DESC
            {
                Format = format,
                Texture2D = new D3D11_TEX2D_SRV {MipLevels = 1, MostDetailedMip = 0},
                ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D
            };

            var handle = Interlocked.Increment(ref _numberOfResources) - 1;
            ref var resourceView = ref _resources[handle];
            resourceView.Format = desc.Format;

            CheckAndThrow(_device.Get()->CreateShaderResourceView(resource, &desc, &_resources[handle].Pointer), "CreateShaderResourceView");

            return handle;
        }

        public void Destroy(in ShaderResourceViewHandle handle)
        {
            ref var resource = ref _resources[handle];
            if (resource.Pointer != null)
            {
                resource.Pointer->Release();
                resource.Pointer = null;
            }
        }

        public ref readonly ShaderResourceView this[in ShaderResourceViewHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _resources[handle];
        }

        public void Dispose()
        {
            for (var i = 0; i < _numberOfResources; ++i)
            {
                ref var resource = ref _resources[i];
                if (resource.Pointer != null)
                {
                    resource.Pointer->Release();
                    resource.Pointer = null;
                }
            }
            _numberOfResources = 0;
            _device.Dispose();
        }
    }
}
