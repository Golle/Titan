using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using Titan.Core.Common;
using Titan.Core.Memory;
using Titan.Graphics.D3D11;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D11_DSV_DIMENSION;

namespace Titan.Graphics.Resources
{
    internal unsafe class DepthStencilViewManager : IDepthStencilViewManager
    {
        private readonly IMemoryManager _memoryManager;
        private ComPtr<ID3D11Device> _device;
        private DepthStencilView* _views;
        private uint _maxViews; // TODO: add implementation for this
        private int _numberOfViews;

        public DepthStencilViewManager(IMemoryManager memoryManager)
        {
            _memoryManager = memoryManager;
        }

        public void Initialize(IGraphicsDevice graphicsDevice)
        {
            if (_views != null)
            {
                throw new InvalidOperationException($"{nameof(DepthStencilViewManager)} has already been initialized.");
            }
            _device = graphicsDevice is GraphicsDevice device ? new ComPtr<ID3D11Device>(device.Ptr) : throw new ArgumentException($"Trying to initialize a D3D11 {nameof(DepthStencilViewManager)} with the wrong device.", nameof(graphicsDevice));
            var memory = _memoryManager.GetMemoryChunkValidated<DepthStencilView>("DepthStencilView");
            _views = memory.Pointer;
            _maxViews = memory.Count;
        }

        public Handle<DepthStencilView> Create(ID3D11Resource* resource, DXGI_FORMAT format)
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

        public void Destroy(in Handle<DepthStencilView> handle)
        {
            Debug.Assert(_views != null, $"{nameof(DepthStencilViewManager)} has not been initialized.");
            ref var view = ref _views[handle];
            if (view.Pointer != null)
            {
                view.Pointer->Release();
                view.Pointer = null;
            }
        }

        public ref readonly DepthStencilView this[in Handle<DepthStencilView> handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _views[handle];
        }

        public void Dispose()
        {
            if (_views != null)
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
                _views = null;
                _device.Dispose();
            }
        }
    }
}
