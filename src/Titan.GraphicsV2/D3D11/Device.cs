using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Titan.Core.Common;
using Titan.Windows.Win32;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.D3D11
{
    internal unsafe class Device : IDisposable
    {
        private ComPtr<ID3D11Device> _device;
        private ComPtr<ID3D11DeviceContext> _context;

        private ResourcePool<Texture> _textures;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11Device* Get() => _device.Get();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ID3D11DeviceContext* GetContext() => _context.Get();
        
        public Device(ID3D11Device * device, ID3D11DeviceContext* context)
        {
            _device = new ComPtr<ID3D11Device>(device);
            _context = new ComPtr<ID3D11DeviceContext>(context);
        }
        
        internal void Init()
        {
            _textures.Init(1000);
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
                Common.CheckAndThrow(_device.Get()->CreateTexture2D(&desc, &subresourceData, &textureP->D3DTexture), nameof(ID3D11Device.CreateTexture2D));
            }
            else
            {
                Common.CheckAndThrow(_device.Get()->CreateTexture2D(&desc, null, &texture->D3DTexture), nameof(ID3D11Device.CreateTexture2D));
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
                Common.CheckAndThrow(_device.Get()->CreateRenderTargetView((ID3D11Resource*) texture->D3DTexture, &renderTargetDesc, &texture->D3DTarget), nameof(ID3D11Device.CreateRenderTargetView));
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
                Common.CheckAndThrow(_device.Get()->CreateShaderResourceView((ID3D11Resource*) texture->D3DTexture, &shadedResourceViewDesc, &texture->D3DResource), nameof(ID3D11Device.CreateShaderResourceView));
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
            _device.Dispose();

            // TODO: how do we track allocated textures? they needs to be released at shutdown
            _textures.Terminate();
        }
    }



    [Flags]
    internal enum TextureBindFlags
    {
        None = 0,
        ShaderResource = 1,
        RenderTarget = 2,
        DepthBuffer = 4
    }

    internal unsafe struct TextureCreation
    {
        // TODO: create new enums for these
        internal DXGI_FORMAT Format;
        internal D3D11_USAGE Usage;

        internal TextureBindFlags Binding;
        internal uint Width;
        internal uint Height;
        internal void* InitialData;
        internal uint DataStride;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal unsafe struct Texture
    {
        internal ID3D11Texture2D* D3DTexture;
        internal ID3D11RenderTargetView* D3DTarget;
        internal ID3D11ShaderResourceView* D3DResource;

        internal uint Width;
        internal uint Height;

        internal DXGI_FORMAT Format;
        internal D3D11_BIND_FLAG BindFlags;
        internal D3D11_USAGE Usage;
    }
}
