using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Memory;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D11_DSV_DIMENSION;

namespace Titan.Graphics.Resources
{
    internal unsafe class DepthStencilViewManager : IDepthStencilViewManager
    {
        private ComPtr<ID3D11Device> _device;
        private readonly DepthStencilView* _views;
        private readonly uint _maxViews;
        private int _numberOfViews;

        public DepthStencilViewManager(ID3D11Device* device, IMemoryManager memoryManager)
        {
            _device = new ComPtr<ID3D11Device>(device);
            
            var memory = memoryManager.GetMemoryChunkValidated<DepthStencilView>("DepthStencilView");
            _views = memory.Pointer;
            _maxViews = memory.Count;
        }
        public DepthStencilViewHandle Create(ID3D11Resource* resource, DXGI_FORMAT format)
        {
            var desc = new D3D11_DEPTH_STENCIL_VIEW_DESC
            {
                Flags = 0,
                Format = format,
                Texture2D = new D3D11_TEX2D_DSV{MipSlice = 0},
                ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D
            };

            var handle = Interlocked.Increment(ref _numberOfViews) - 1;
            ref var view = ref _views[handle];
            view.Format = desc.Format;
            CheckAndThrow(_device.Get()->CreateDepthStencilView(resource, &desc, &_views[handle].Pointer), "CreateDepthStencilView");
            return handle;
        }

        public void Destroy(in DepthStencilViewHandle handle)
        {
            ref var view = ref _views[handle];
            if (view.Pointer != null)
            {
                view.Pointer->Release();
                view.Pointer = null;
            }
        }

        public ref readonly DepthStencilView this[in DepthStencilViewHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _views[handle];
        }

        public void Dispose()
        {
            for (var i = 0; i < _numberOfViews; ++i)
            {
                ref var view = ref _views[i];
                if (view.Pointer != null)
                {
                    view.Pointer->Release();
                    view.Pointer = null;
                }
            }
            _numberOfViews = 0;
            _device.Dispose();
        }
    }
}
