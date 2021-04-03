using System;
using Titan.Core.Common;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Textures;
using Titan.GraphicsV2.Resources.Textures;
using Titan.Windows.Win32.D3D11;

namespace Titan.Assets
{
    internal class TextureAssetLoader : IAssetLoader<Handle<Texture>>
    {
        private readonly Device _device;
        private readonly IImageLoader _imageLoader;

        public TextureAssetLoader(Device device, IImageLoader imageLoader)
        {
            _device = device;
            _imageLoader = imageLoader;
        }

        public unsafe Handle<Texture> LoadFromData(ReadOnlySpan<byte> data)
        {
            fixed (byte* pData = data)
            {
                var image = _imageLoader.Load(pData, (uint) data.Length);
                var textureHandle = _device.TextureManager.Create(new TextureCreation
                {
                    Binding = TextureBindFlags.ShaderResource,
                    DataStride = image.Stride,
                    Format = image.Format,
                    Height = image.Height,
                    Width = image.Width,
                    Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE,
                    InitialData = new DataBlob(image.GetBuffer(), image.GetBufferSize())
                });
                return textureHandle;
            }
        }
    }
}
