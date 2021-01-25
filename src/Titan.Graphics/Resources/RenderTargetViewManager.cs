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
using static Titan.Windows.Win32.D3D11.D3D11Common;

namespace Titan.Graphics.Resources
{
    internal unsafe class RenderTargetViewManager : IRenderTargetViewManager
    {
        private readonly IMemoryManager _memoryManager;
        private ComPtr<ID3D11Device> _device;
        private ComPtr<ID3D11RenderTargetView> _backbuffer;
        private RenderTargetView* _views;
        private uint _maxViews;
        private int _numberOfViews;
        public Handle<RenderTargetView> BackbufferHandle { get; } = 0;

        public RenderTargetViewManager(IMemoryManager memoryManager)
        {
            _memoryManager = memoryManager;
        }
      
        public void Initialize(IGraphicsDevice graphicsDevice)
        {
            if (_views != null)
            {
                throw new InvalidOperationException($"{nameof(RenderTargetViewManager)} has already been initialized.");
            }

            if (graphicsDevice is not D3D11GraphicsDevice device)
            {
                throw new ArgumentException($"Trying to initialize a D3D11 {nameof(RenderTargetViewManager)} with the wrong device.", nameof(graphicsDevice));
            }
            
            var memory = _memoryManager.GetMemoryChunkValidated<RenderTargetView>("RenderTargetView");
            _views = memory.Pointer;
            _maxViews = memory.Count;
            _numberOfViews = 1;

            // Get the backbuffer
            using var backbufferResource = new ComPtr<ID3D11Resource>();
            fixed (Guid* resourcePointer = &D3D11Resource)
            {
                CheckAndThrow(device.SwapChainPtr->GetBuffer(0, resourcePointer, (void**)backbufferResource.GetAddressOf()), "GetBuffer");
            }
            CheckAndThrow(device.Ptr->CreateRenderTargetView(backbufferResource.Get(), null, _backbuffer.GetAddressOf()), "CreateRenderTargetView");
            
            _views[BackbufferHandle] = new RenderTargetView { Pointer = _backbuffer.Get() };
            _device = new ComPtr<ID3D11Device>(device.Ptr);
        }

        public Handle<RenderTargetView> Create(ID3D11Resource* resource, DXGI_FORMAT format)
        {
            var desc = new D3D11_RENDER_TARGET_VIEW_DESC
            {
                Format = format,
                Texture2D = new D3D11_TEX2D_RTV {MipSlice = 0},
                ViewDimension = D3D11_RTV_DIMENSION.D3D11_RTV_DIMENSION_TEXTURE2D
            };

            var handle = Interlocked.Increment(ref _numberOfViews) - 1;

            CheckAndThrow(_device.Get()->CreateRenderTargetView(resource, &desc, &_views[handle].Pointer), "CreateRenderTargetView");

            return handle;
        }

        public void Destroy(in Handle<RenderTargetView> handle)
        {
            Debug.Assert(handle != 0, "Trying to destroy the backbuffer");
            ref var view = ref _views[handle];
            if (view.Pointer != null)
            {
                view.Pointer->Release();
                view.Pointer = null;
            }
        }

        public ref readonly RenderTargetView this[in Handle<RenderTargetView> handle]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _views[handle];
        }

        public void Dispose()
        {
            if (_views != null)
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
                _backbuffer.Dispose(); // TOdo: Hmm?
                _device.Dispose();
                _views = null;
            }
            
        }
    }
}
