using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Memory;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.Graphics.Resources
{
    internal unsafe class RenderTargetViewManager : IRenderTargetViewManager
    {
        private ComPtr<ID3D11Device> _device;
        private ComPtr<ID3D11RenderTargetView> _backbuffer;
        private readonly RenderTargetView* _views;
        private readonly uint _maxViews;
        private int _numberOfViews;
        public RenderTargetViewHandle BackbufferHandle { get; } = 0;

        public RenderTargetViewManager(ID3D11Device* device, ID3D11RenderTargetView* backbuffer, IMemoryManager memoryManager)
        {
            _device = new ComPtr<ID3D11Device>(device);
            _backbuffer = new ComPtr<ID3D11RenderTargetView>(backbuffer);
            var memory = memoryManager.GetMemoryChunkValidated<RenderTargetView>("RenderTargetView");
            _views = memory.Pointer;
            _maxViews = memory.Count;

            _numberOfViews = 1;
            _views[0] = new RenderTargetView {Pointer = backbuffer};
        }

        public RenderTargetViewHandle Create(ID3D11Resource* resource, DXGI_FORMAT format)
        {
            var desc = new D3D11_RENDER_TARGET_VIEW_DESC
            {
                Format = format,
                Texture2D = new D3D11_TEX2D_RTV {MipSlice = 0},
                ViewDimension = D3D11_RTV_DIMENSION.D3D11_RTV_DIMENSION_TEXTURE2D
            };

            var handle = Interlocked.Increment(ref _numberOfViews) - 1;

            Common.CheckAndThrow(_device.Get()->CreateRenderTargetView(resource, &desc, &_views[handle].Pointer), "CreateRenderTargetView");

            return handle;
        }

        public void Destroy(in RenderTargetViewHandle handle)
        {
            ref var view = ref _views[handle];
            if (view.Pointer != null)
            {
                view.Pointer->Release();
                view.Pointer = null;
            }
        }

        public ref readonly RenderTargetView this[in RenderTargetViewHandle handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _views[handle];
        }

        public void Dispose()
        {
            // Start at 1, don't free the backbuffer
            for (var i = 1; i < _numberOfViews; ++i)
            {
                ref var resource = ref _views[i];
                if (resource.Pointer != null)
                {
                    resource.Pointer->Release();
                    resource.Pointer = null;
                }
            }
            _numberOfViews = 0;
            _backbuffer.Dispose();
            _device.Dispose();
        }
    }
}
