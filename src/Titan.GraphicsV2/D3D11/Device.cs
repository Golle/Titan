using System;
using System.Runtime.CompilerServices;
using Titan.Core.Common;
using Titan.Core.Logging;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;
using static Titan.Windows.Win32.Common;
using static Titan.Windows.Win32.D3D11.D3D11Common;

namespace Titan.GraphicsV2.D3D11
{
    public record DeviceConfiguration(HWND WindowHandle, uint Width, uint Height, uint RefreshRate, bool Windowed, bool VSync, bool Debug = false);

    internal unsafe class Device : IDisposable
    {
        // D3d pointers
        private ComPtr<ID3D11Device> _device;
        private ComPtr<ID3D11DeviceContext> _context;
        private ComPtr<IDXGISwapChain> _swapChain;


        // Swapchain and Context
        public Swapchain Swapchain { get; }
        public Context Context { get; }


        // Resources
        private ResourcePool<Texture> _textures;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11Device* Get() => _device.Get();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11DeviceContext* GetContext() => _context.Get();
        
        internal Device(DeviceConfiguration configuration)
        {
            var flags = configuration.Debug ? 2u : 0u;
            var featureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1;
            var desc = new DXGI_SWAP_CHAIN_DESC
            {
                BufferCount = 2,
                BufferDesc = new DXGI_MODE_DESC
                {
                    Width = configuration.Width,
                    Height = configuration.Height,
                    RefreshRate = new DXGI_RATIONAL { Denominator = configuration.RefreshRate },
                    Scaling = DXGI_MODE_SCALING.DXGI_MODE_SCALING_UNSPECIFIED,
                    ScanlineOrdering = DXGI_MODE_SCANLINE_ORDER.DXGI_MODE_SCANLINE_ORDER_UNSPECIFIED,
                    Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM
                },
                SampleDesc = new DXGI_SAMPLE_DESC
                {
                    Count = 1,
                    Quality = 0
                },
                BufferUsage = DXGI_USAGE.DXGI_USAGE_RENDER_TARGET_OUTPUT,
                OutputWindow = configuration.WindowHandle,
                SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD,
                Flags = DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH,
                Windowed = configuration.Windowed
            };

            LOGGER.Debug("Creating D3D11 Device");
            CheckAndThrow(D3D11CreateDeviceAndSwapChain(null, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_HARDWARE, 0, flags, null, 0, D3D11_SDK_VERSION, &desc, _swapChain.GetAddressOf(), _device.GetAddressOf(), null, _context.GetAddressOf()), nameof(D3D11CreateDeviceAndSwapChain));
            LOGGER.Debug("D3D11 Device Created");

            Context = new Context(_context.Get());
            Swapchain = new Swapchain(_swapChain.Get(), configuration.VSync, configuration.Width, configuration.Height);

            LOGGER.Debug("Initialize resource pools");
            
            _textures.Init(1000);

            LOGGER.Debug("Resource pools initialized");
        }

        
        internal Handle<Texture> CreateTexture(in TextureCreation args)
        {
            var handle = _textures.CreateResource();
            if (!handle.IsValid())
            {
                throw new InvalidOperationException("Failed to Create Texture Handle");
            }

            D3D11_BIND_FLAG bindflags = 0;
            if (args.Binding.HasFlag(TextureBindFlags.RenderTarget))
            {
                bindflags |= D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET;
            }

            if (args.Binding.HasFlag(TextureBindFlags.ShaderResource))
            {
                bindflags |= D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE;
            }

            if (bindflags == 0)
            {
                throw new InvalidOperationException("Can't create a Texture without a binding");
            }
            
            var desc = new D3D11_TEXTURE2D_DESC
            {
                Format = args.Format,
                ArraySize = 1,
                MipLevels = 1,
                BindFlags = bindflags,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                Height = args.Height,
                Width = args.Width,
                Usage = args.Usage,
                MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFICED,
                SampleDesc = new DXGI_SAMPLE_DESC
                {
                    Count = 1,
                    Quality = 0
                }
            };

            var texture = _textures.GetResourcePointer(handle);

            texture->Handle = handle;
            texture->BindFlags = bindflags;
            texture->Format = args.Format;
            texture->Height = args.Height;
            texture->Width = args.Width;
            texture->Usage = args.Usage;

            var textureP = _textures.GetResourcePointer(handle);
            if (args.InitialData != null && args.DataStride > 0)
            {
                var subresourceData = new D3D11_SUBRESOURCE_DATA
                {
                    pSysMem = args.InitialData
                };
                CheckAndThrow(_device.Get()->CreateTexture2D(&desc, &subresourceData, &textureP->D3DTexture), nameof(ID3D11Device.CreateTexture2D));
            }
            else
            {
                CheckAndThrow(_device.Get()->CreateTexture2D(&desc, null, &texture->D3DTexture), nameof(ID3D11Device.CreateTexture2D));
            }

            if ((bindflags & D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET) != 0)
            {
                var renderTargetDesc = new D3D11_RENDER_TARGET_VIEW_DESC
                {
                    Format = args.Format,
                    Texture2D = new D3D11_TEX2D_RTV
                    {
                        MipSlice = 0
                    },
                    ViewDimension = D3D11_RTV_DIMENSION.D3D11_RTV_DIMENSION_TEXTURE2D
                };
                CheckAndThrow(_device.Get()->CreateRenderTargetView((ID3D11Resource*) texture->D3DTexture, &renderTargetDesc, &texture->D3DTarget), nameof(ID3D11Device.CreateRenderTargetView));
            }

            if ((bindflags & D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE) != 0)
            {
                var shadedResourceViewDesc = new D3D11_SHADER_RESOURCE_VIEW_DESC
                {
                    Format = args.Format,
                    Texture2D = new D3D11_TEX2D_SRV
                    {
                        MipLevels = 1,
                        MostDetailedMip = 0
                    },
                    ViewDimension = D3D_SRV_DIMENSION.D3D10_1_SRV_DIMENSION_TEXTURE2D
                };
                CheckAndThrow(_device.Get()->CreateShaderResourceView((ID3D11Resource*) texture->D3DTexture, &shadedResourceViewDesc, &texture->D3DResource), nameof(ID3D11Device.CreateShaderResourceView));
            }
            
            return handle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly Texture AccessTexture(in Handle<Texture> handle) => ref _textures.GetResourceReference(handle);

        internal void DestroyTexture(in Handle<Texture> handle)
        {
            var texture = _textures.GetResourcePointer(handle);
            if (texture->D3DResource != null)
            {
                texture->D3DResource->Release();
            }
            if (texture->D3DTexture != null)
            {
                texture->D3DTexture->Release();
            }
            if (texture->D3DTarget != null)
            {
                texture->D3DTarget->Release();
            }
            *texture = default; // TODO: is not really needed, but we can do it to "clean" up the pointers so they can't be used.
            _textures.ReleaseResource(handle);
        }

        public void Dispose()
        {
            _context.Dispose();
            _swapChain.Dispose();
            _device.Dispose();

            // TODO: how do we track allocated textures? they needs to be released at shutdown
            _textures.Terminate();
        }
    }
}
