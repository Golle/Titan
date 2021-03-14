using Titan.Core.Common;
using Titan.GraphicsV2.D3D11;
using Titan.GraphicsV2.D3D11.Textures;
using Titan.Windows.Win32.D3D11;

namespace Titan.GraphicsV2.Resources.Textures
{
    internal class TextureLoader
    {
        private readonly Device _device;
        private readonly IImageLoader _imageLoader;

        internal TextureLoader(Device device, IImageLoader imageLoader)
        {
            _device = device;
            _imageLoader = imageLoader;
        }
        
        public unsafe Handle<Texture> Load(string identifier)
        {
            var image = _imageLoader.Load(identifier);

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
