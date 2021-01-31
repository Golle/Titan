using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Common;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D_SRV_DIMENSION;

namespace Titan.Graphics.Resources
{
    internal unsafe class ShaderResourceViewManager : IShaderResourceViewManager
    {
        private readonly IMemoryManager _memoryManager;
        private ComPtr<ID3D11Device> _device;
        private ShaderResourceView* _resources;

        private int _numberOfResources;
        private uint _maxResources;

        public ShaderResourceViewManager(IMemoryManager memoryManager)
        {
            _memoryManager = memoryManager;
        }

        public void Initialize(IGraphicsDevice graphicsDevice)
        {
            if (_resources != null)
            {
                throw new InvalidOperationException($"{nameof(ShaderResourceViewManager)} has already been initialized.");
            }
            _device = graphicsDevice is D3D11GraphicsDevice device ? new ComPtr<ID3D11Device>(device.Ptr) : throw new ArgumentException($"Trying to initialize a D3D11 {nameof(ShaderResourceViewManager)} with the wrong device.", nameof(graphicsDevice));
            var memory = _memoryManager.GetMemoryChunkValidated<ShaderResourceView>("ShaderResourceView");
            _resources = memory.Pointer;
            _maxResources = memory.Count;
        }

        public Handle<ShaderResourceView> Create(ID3D11Resource* resource, DXGI_FORMAT format)
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

        public void Destroy(in Handle<ShaderResourceView> handle)
        {
            ref var resource = ref _resources[handle];
            if (resource.Pointer != null)
            {
                resource.Pointer->Release();
                resource.Pointer = null;
            }
        }

        public ref readonly ShaderResourceView this[in Handle<ShaderResourceView> handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _resources[handle];
        }

        public void Dispose()
        {
            if (_resources != null)
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
                _resources = null;
                _device.Dispose();
            }
        }
    }
}
