using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Titan.Core;
using Titan.Core.Logging;
using Titan.Core.Memory;
using Titan.Windows.D3D11;
using static Titan.Windows.Common;


namespace Titan.Graphics.D3D11.Textures
{
    public unsafe class TextureManager : IDisposable
    {
        private const int MaxNumberOfTextures = 1000;

        private readonly ID3D11Device* _device;
        private readonly SwapChain _swapChain;
        private ResourcePool<Texture> _resourcePool;

        private readonly List<Handle<Texture>> _usedHandles = new();

        internal TextureManager(ID3D11Device* device, SwapChain swapChain)
        {
            Logger.Trace<TextureManager>($"Init with {MaxNumberOfTextures} slots");
            _device = device;
            _swapChain = swapChain;
            _resourcePool.Init(MaxNumberOfTextures);
        }

        public Handle<Texture> Create(TextureCreation args)
        {
            var handle = _resourcePool.CreateResource();
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

            var height = args.Height == 0 ? _swapChain.Height : args.Height;
            var width = args.Width == 0 ? _swapChain.Width : args.Width;
            Logger.Trace<TextureManager>($"Creating texture with format {args.Format} and size {width} x {height}");

            var desc = new D3D11_TEXTURE2D_DESC
            {
                Format = args.Format,
                ArraySize = 1,
                MipLevels = 1,
                BindFlags = bindflags,
                CpuAccessFlags = D3D11_CPU_ACCESS_FLAG.UNSPECIFIED,
                Height = height,
                Width = width,
                Usage = args.Usage,
                MiscFlags = D3D11_RESOURCE_MISC_FLAG.UNSPECIFIED,
                SampleDesc = new DXGI_SAMPLE_DESC
                {
                    Count = 1,
                    Quality = 0
                }
            };

            var texture = _resourcePool.GetResourcePointer(handle);
            texture->Handle = handle;
            texture->BindFlags = bindflags;
            texture->Format = args.Format;
            texture->Height = height;
            texture->Width = width;
            texture->Usage = args.Usage;


            if (args.InitialData.HasValue() && args.DataStride > 0)
            {
                var subresourceData = new D3D11_SUBRESOURCE_DATA
                {
                    pSysMem = args.InitialData,
                    SysMemPitch = args.DataStride
                };
                CheckAndThrow(_device->CreateTexture2D(&desc, &subresourceData, &texture->D3DTexture), nameof(ID3D11Device.CreateTexture2D));
            }
            else
            {
                CheckAndThrow(_device->CreateTexture2D(&desc, null, &texture->D3DTexture), nameof(ID3D11Device.CreateTexture2D));
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
                CheckAndThrow(_device->CreateRenderTargetView((ID3D11Resource*)texture->D3DTexture, &renderTargetDesc, &texture->D3DTarget), nameof(ID3D11Device.CreateRenderTargetView));
            }
            else
            {
                texture->D3DTarget = null;
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
                    ViewDimension = D3D_SRV_DIMENSION.D3D11_SRV_DIMENSION_TEXTURE2D
                };
                CheckAndThrow(_device->CreateShaderResourceView((ID3D11Resource*)texture->D3DTexture, &shadedResourceViewDesc, &texture->D3DResource), nameof(ID3D11Device.CreateShaderResourceView));
            }
            else
            {
                texture->D3DResource = null;
            }
            
            _usedHandles.Add(handle);
            return handle;
        }

        public void Release(in Handle<Texture> handle)
        {
            ReleaseInternal(handle);
            _usedHandles.Remove(handle);
            _resourcePool.ReleaseResource(handle);
        }

        private void ReleaseInternal(Handle<Texture> handle)
        {
            var texture = _resourcePool.GetResourcePointer(handle);
            if (texture->D3DTarget != null)
            {
                texture->D3DTarget->Release();
            }
            if (texture->D3DResource != null)
            {
                texture->D3DResource->Release();
            }
            if (texture->D3DTexture != null)
            {
                texture->D3DTexture->Release();
            }
            *texture = default; // TODO: is not really needed, but we can do it to "clean" up the pointers so they can't be used.
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref readonly Texture Access(in Handle<Texture> handle) => ref _resourcePool.GetResourceReference(handle);

        public void Dispose()
        {
            if (_usedHandles.Count > 0)
            {
                Logger.Warning<TextureManager>($"{_usedHandles.Count} unreleased resources when disposing the manager");
                Logger.Trace<TextureManager>($"Releasing {_usedHandles.Count} textures");
            }
            foreach (var handle in _usedHandles)
            {
                ReleaseInternal(handle);
            }
            Logger.Trace<TextureManager>("Terminate resource pool");
            _resourcePool.Terminate();
        }
    }
}
